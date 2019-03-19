using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents the player.
    /// </summary>
    /// <seealso cref="SizedPoint"/>
    public class Player : SizedPoint
    {
        // ticks count while kicking
        private int _hitKickCount;
        // history of movements
        private Queue<Point> _moveHistory = new Queue<Point>(Constants.MOVE_HISTORY_COUNT);

        /// <summary>
        /// Speed (i.e. distance, in pixels, by tick)
        /// </summary>
        public double Speed { get; private set; }
        /// <summary>
        /// Ratio of hit reach depending to the player size.
        /// </summary>
        public double HitReachRatio { get; private set; }
        /// <summary>
        /// Inferred; Speed by side while moving in diagonal. (Pythagore reversal)
        /// </summary>
        /// <remarks>Assumes that <see cref="base.X"/> and <see cref="base.Y"/> have the same value.</remarks>
        public double DiagonalSpeedBySize { get { return Math.Sqrt((Speed * Speed) / 2); } }
        /// <summary>
        /// When coming into a new screen, indicates the direction relative to the former screen.
        /// </summary>
        public Directions? NewScreenEntrance { get; private set; }
        /// <summary>
        /// Inferred; indicates if currently kicking.
        /// </summary>
        public bool IsHitting { get { return _hitKickCount >= 0; } }
        /// <summary>
        /// When kicking, indicates the life-points cost on the enemy.
        /// </summary>
        public int HitLifePointCost { get; private set; }
        /// <summary>
        /// Inferred; the X-axis size of an hit reach depending on <see cref="base.Width"/>.
        /// </summary>
        public double HitWidth { get { return ((HitReachRatio - 1) / 2) * Width; } }
        /// <summary>
        /// Inferred; the Y-axis size of an hit reach depending on <see cref="base.Height"/>.
        /// </summary>
        public double HitHeight { get { return ((HitReachRatio - 1) / 2) * Height; } }
        /// <summary>
        /// Tick count before the effect of a hit ends.
        /// </summary>
        public int HitTickMaxCount { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>Every initial values come from <see cref="InitialPlayerStatus"/>.</remarks>
        public Player() : base(
            InitialPlayerStatus.INITIAL_PLAYER_X,
            InitialPlayerStatus.INITIAL_PLAYER_Y,
            InitialPlayerStatus.SPRITE_SIZE_X,
            InitialPlayerStatus.SPRITE_SIZE_Y)
        {
            Speed = InitialPlayerStatus.INITIAL_PLAYER_SPEED;
            HitReachRatio = InitialPlayerStatus.INITIAL_HIT_SIZE_RATIO;
            NewScreenEntrance = null;
            HitLifePointCost = InitialPlayerStatus.HIT_LIFE_POINT_COST;
            _hitKickCount = -1;
            HitTickMaxCount = InitialPlayerStatus.HIT_TICK_MAX_COUNT;
        }

        /// <summary>
        /// Behavior of the instance at tick.
        /// </summary>
        /// <param name="engine"><see cref="AbstractEngine"/></param>
        /// <param name="keys"><see cref="KeyPress"/></param>
        public override void ComputeBehaviorAtTick(AbstractEngine engine, KeyPress keys)
        {
            // gère le temps d'effet du kick
            if (_hitKickCount >= HitTickMaxCount)
            {
                _hitKickCount = -1;
            }
            else if (_hitKickCount >= 0)
            {
                _hitKickCount += 1;
            }
            else if (keys.PressHit)
            {
                _hitKickCount = 0;
            }

            NewScreenEntrance = null;
            double newTop = Y;
            double newLeft = X;

            if (keys.PressUp)
            {
                if (keys.PressLeft)
                {
                    newTop -= DiagonalSpeedBySize;
                    newLeft -= DiagonalSpeedBySize;
                }
                else if (keys.PressRight)
                {
                    newTop -= DiagonalSpeedBySize;
                    newLeft += DiagonalSpeedBySize;
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

                    newTop += DiagonalSpeedBySize;
                    newLeft -= DiagonalSpeedBySize;
                }
                else if (keys.PressRight)
                {
                    newTop += DiagonalSpeedBySize;
                    newLeft += DiagonalSpeedBySize;
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
                bool goLeft = newLeft < 0;
                bool goUp = newTop < 0;
                bool goRight = newLeft + Width > engine.AreaWidth;
                bool goDown = newTop + Height > engine.AreaHeight;
                if (goLeft || goUp || goRight || goDown)
                {
                    if (goLeft)
                    {
                        newLeft = engine.AreaWidth - Width;
                        if (goUp)
                        {
                            NewScreenEntrance = Directions.top_left;
                            newTop = engine.AreaHeight - Height;
                        }
                        else if (goDown)
                        {
                            NewScreenEntrance = Directions.bottom_left;
                            newTop = 0;
                        }
                        else
                        {
                            NewScreenEntrance = Directions.left;
                        }
                    }
                    else if (goRight)
                    {
                        newLeft = 0;
                        if (goUp)
                        {
                            NewScreenEntrance = Directions.top_right;
                            newTop = engine.AreaHeight - Height;
                        }
                        else if (goDown)
                        {
                            NewScreenEntrance = Directions.bottom_right;
                            newTop = 0;
                        }
                        else
                        {
                            NewScreenEntrance = Directions.right;
                        }
                    }
                    else if (goUp)
                    {
                        NewScreenEntrance = Directions.top;
                        newTop = engine.AreaHeight - Height;
                    }
                    else
                    {
                        NewScreenEntrance = Directions.bottom;
                        newTop = 0;
                    }
                }

                else
                {
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
                                    // TODO : logger to implement
                                    return;
                                }
                                loop = true;
                            }
                        }
                    }
                    while (loop);
                }

                _moveHistory.Enqueue(new Point(X, Y));
                if (_moveHistory.Count > Constants.MOVE_HISTORY_COUNT)
                {
                    _moveHistory.Dequeue();
                }
                X = newLeft;
                Y = newTop;
            }
        }

        /// <summary>
        /// Checks if the instance currently hit an enemy.
        /// </summary>
        /// <param name="enemy"><see cref="Enemy"/></param>
        /// <returns><c>True</c> if the enemy has been hit; otherwise <c>False</c>.</returns>
        public bool CheckHitReachEnemy(Enemy enemy)
        {
            if (IsHitting)
            {
                var kickHaloX = X - (((HitReachRatio - 1) / 2) * Width);
                var kickHaloY = Y - (((HitReachRatio - 1) / 2) * Height);

                return enemy.Overlap(new SizedPoint(kickHaloX, kickHaloY, Width * HitReachRatio, Height * HitReachRatio));
            }
            return false;
        }
    }
}
