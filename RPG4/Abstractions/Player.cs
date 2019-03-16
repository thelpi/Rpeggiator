using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RPG4.Abstractions
{
    public class Player : SizedPoint
    {
        private const int MOVE_HISTORY_COUNT = 50;
        private Queue<Point> _moveHistory = new Queue<Point>(MOVE_HISTORY_COUNT);

        /// <summary>
        /// Speed (i.e. distance, in pixels, by tick)
        /// </summary>
        public double Speed { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public double KickReachRatio { get; private set; }
        // inverse de pythagore pour la distance parcourue dans chaque sens (top et left) lors d'un mouvement en diagonale
        public double DiagonaleMoveSideDistance { get { return Math.Sqrt((Speed * Speed) / 2); } }

        public Player(double x, double y, double width, double height, double distanceByTick, double kickReachRatio)
            : base(x, y, width, height)
        {
            Speed = distanceByTick;
            KickReachRatio = kickReachRatio;
        }

        public override void ComputeBehaviorAtTick(AbstractEngine engine, KeyPress keys)
        {
            double newTop = Y;
            double newLeft = X;

            if (keys.PressUp)
            {
                if (keys.PressLeft)
                {
                    newTop -= DiagonaleMoveSideDistance;
                    newLeft -= DiagonaleMoveSideDistance;
                }
                else if (keys.PressRight)
                {
                    newTop -= DiagonaleMoveSideDistance;
                    newLeft += DiagonaleMoveSideDistance;
                }
                else
                {
                    newTop -= Speed;
                }
            }
            else if (keys.PressDown)
            {
                if (keys.PressLeft)
                {

                    newTop += DiagonaleMoveSideDistance;
                    newLeft -= DiagonaleMoveSideDistance;
                }
                else if (keys.PressRight)
                {
                    newTop += DiagonaleMoveSideDistance;
                    newLeft += DiagonaleMoveSideDistance;
                }
                else
                {
                    newTop += Speed;
                }
            }
            else if (keys.PressLeft)
            {
                newLeft -= Speed;
            }
            else if (keys.PressRight)
            {
                newLeft += Speed;
            }

            // si mouvement pour moi
            if (newLeft != X || newTop != Y)
            {
                // correction des trajectoires au bord
                if (newLeft < 0)
                {
                    newLeft = 0;
                }
                if (newTop < 0)
                {
                    newTop = 0;
                }
                if (newLeft + Width > engine.AreaWidth)
                {
                    newLeft = (engine.AreaWidth - Width);
                }
                if (newTop + Height > engine.AreaHeight)
                {
                    newTop = (engine.AreaHeight - Height);
                }

                // correction des trajectoires au bord d'un obstacle
                var forbiddens = new List<SizedPoint>();
                bool loop = true;
                do
                {
                    var currentPt = Copy(newLeft, newTop);
                    loop = false;
                    foreach (SizedPoint rect in engine.ConcreteWalls)
                    {
                        Point pToMove = rect.CheckOverlapAndAdjustPosition(currentPt, this,
                            keys.PressLeft ? true : (keys.PressRight ? false : (bool?)null),
                            keys.PressUp ? true : (keys.PressDown ? false : (bool?)null));

                        if (pToMove.X >= 0 || pToMove.Y >= 0)
                        {
                            forbiddens.Add(currentPt);
                            newLeft = pToMove.X;
                            newTop = pToMove.Y;
                            if (forbiddens.Any(pt => pt.X == newLeft && pt.Y == newTop))
                            {
                                throw new InvalidProgramException("Collide check infinite loop !");
                            }
                            loop = true;
                        }
                    }
                }
                while (loop);

                _moveHistory.Enqueue(new Point(X, Y));
                if (_moveHistory.Count > MOVE_HISTORY_COUNT)
                {
                    _moveHistory.Dequeue();
                }
                X = newLeft;
                Y = newTop;
            }
        }

        public bool CheckKick(Enemy enemy)
        {
            var kickHaloX = X - (((KickReachRatio - 1) / 2) * Width);
            var kickHaloY = Y - (((KickReachRatio - 1) / 2) * Height);

            return enemy.Overlap(new SizedPoint(kickHaloX, kickHaloY, Width * KickReachRatio, Height * KickReachRatio));
        }
    }
}
