using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents the player.
    /// </summary>
    /// <seealso cref="HaloSizedPoint"/>
    public class Player : HaloSizedPoint
    {
        // history of movements
        private Queue<Point> _moveHistory = new Queue<Point>(Constants.MOVE_HISTORY_COUNT);

        /// <summary>
        /// Speed (i.e. distance, in pixels, by tick)
        /// </summary>
        public double Speed { get; private set; }
        /// <summary>
        /// Inferred; Speed by side while moving in diagonal. (Pythagore reversal)
        /// </summary>
        /// <remarks>Assumes that <see cref="Sprite.X"/> and <see cref="Sprite.Y"/> have the same value.</remarks>
        public double DiagonalSpeedBySize { get { return Math.Sqrt((Speed * Speed) / 2); } }
        /// <summary>
        /// When coming into a new screen, indicates the direction relative to the former screen.
        /// </summary>
        public Directions? NewScreenEntrance { get; private set; }
        /// <summary>
        /// When kicking, indicates the life-points cost on the enemy.
        /// </summary>
        public int HitLifePointCost { get; private set; }
        /// <summary>
        /// Inventory.
        /// </summary>
        public Inventory Inventory { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>Every initial values come from <see cref="InitialPlayerStatus"/>.</remarks>
        public Player() : base(
            InitialPlayerStatus.INITIAL_PLAYER_X,
            InitialPlayerStatus.INITIAL_PLAYER_Y,
            InitialPlayerStatus.SPRITE_SIZE_X,
            InitialPlayerStatus.SPRITE_SIZE_Y,
            InitialPlayerStatus.INITIAL_HIT_HALO_SIZE_RATIO,
            InitialPlayerStatus.HIT_TICK_MAX_COUNT)
        {
            Speed = InitialPlayerStatus.INITIAL_PLAYER_SPEED;
            NewScreenEntrance = null;
            HitLifePointCost = InitialPlayerStatus.HIT_LIFE_POINT_COST;
            Inventory = new Inventory();
        }

        /// <summary>
        /// Behavior of the instance at tick.
        /// </summary>
        /// <param name="engine"><see cref="AbstractEngine"/></param>
        /// <param name="args">Other arguments; in this case, the first one is <see cref="KeyPress"/>.</param>
        public override void ComputeBehaviorAtTick(AbstractEngine engine, params object[] args)
        {
            var keys = args[0] as KeyPress;

            base.ComputeBehaviorAtTick(engine, keys.PressHit);

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
                    var forbiddens = new List<Sprite>();
                    bool loop = true;
                    do
                    {
                        var currentPt = Copy(newLeft, newTop);
                        loop = false;
                        foreach (Sprite rect in engine.ConcreteWalls)
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
            return DisplayHalo && enemy.Overlap(Halo);
        }
    }
}
