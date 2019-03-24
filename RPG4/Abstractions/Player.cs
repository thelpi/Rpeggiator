using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents the player.
    /// </summary>
    /// <seealso cref="LifeSprite"/>
    public class Player : LifeSprite
    {
        // Current frames count while recovering.
        private int _currentRecoveryFrameCount;
        // History of movements.
        private Queue<Point> _moveHistory = new Queue<Point>(Constants.MOVE_HISTORY_COUNT);

        /// <summary>
        /// Speed (i.e. distance, in pixels, by frame)
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
        /// Inventory.
        /// </summary>
        public Inventory Inventory { get; private set; }
        /// <summary>
        /// Hit halo.
        /// </summary>
        public HaloSprite HitHalo { get; private set; }
        /// <summary>
        /// Indicates the player is currently recovering from an hit.
        /// </summary>
        public bool IsRecovering { get { return _currentRecoveryFrameCount >= 0 && _currentRecoveryFrameCount <= Constants.RECOVERY_FRAME_COUNT; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>Every initial values come from <see cref="InitialPlayerStatus"/>.</remarks>
        public Player() : base(
            InitialPlayerStatus.INITIAL_PLAYER_X,
            InitialPlayerStatus.INITIAL_PLAYER_Y,
            InitialPlayerStatus.SPRITE_SIZE_X,
            InitialPlayerStatus.SPRITE_SIZE_Y,
            InitialPlayerStatus.MAXIMAL_LIFE_POINTS,
            InitialPlayerStatus.HIT_LIFE_POINT_COST)
        {
            Speed = InitialPlayerStatus.INITIAL_PLAYER_SPEED;
            NewScreenEntrance = null;
            Inventory = new Inventory();
            HitHalo = new HaloSprite(X, Y, Width, Height, InitialPlayerStatus.INITIAL_HIT_HALO_SIZE_RATIO, InitialPlayerStatus.HIT_FRAME_MAX_COUNT);
            _currentRecoveryFrameCount = -1;
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame(AbstractEngine engine, params object[] args)
        {
            var keys = args[0] as KeyPress;

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
                        foreach (Sprite sprite in engine.SolidStructures)
                        {
                            Point pToMove = sprite.CheckOverlapAndAdjustPosition(currentPt, this,
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
            
            HitHalo.BehaviorAtNewFrame(engine, keys.PressHit, this);
        }

        /// <summary>
        /// Checks if the instance has been hit.
        /// </summary>
        /// <param name="engine"><see cref="AbstractEngine"/></param>
        public void CheckIfHasBeenHit(AbstractEngine engine)
        {
            // currently recovering ?
            if (IsRecovering)
            {
                _currentRecoveryFrameCount++;
                if (_currentRecoveryFrameCount > Constants.RECOVERY_FRAME_COUNT)
                {
                    // end of recovery
                    _currentRecoveryFrameCount = -1;
                }
            }

            if (_currentRecoveryFrameCount < 0)
            {
                // checks hit
                int cumuledLifePoints = engine.CheckHitByEnemiesOnPlayer();

                if (cumuledLifePoints > 0)
                {
                    // sets hit points
                    Hit(cumuledLifePoints);

                    // beginning of recovery
                    _currentRecoveryFrameCount = 0;
                }
            }
        }

        /// <summary>
        /// Drinks a life potion.
        /// </summary>
        /// <param name="potionType"><see cref="ItemIdEnum"/>; ignored if not a life potion.</param>
        public void DrinkLifePotion(ItemIdEnum potionType)
        {
            int recoveryPoints = 0;
            switch (potionType)
            {
                case ItemIdEnum.SmallLifePotion:
                    recoveryPoints = Constants.SMALL_LIFE_POTION_RECOVERY_LIFE_POINTS;
                    break;
                case ItemIdEnum.MediumLifePotion:
                    recoveryPoints = Constants.MEDIUM_LIFE_POTION_RECOVERY_LIFE_POINTS;
                    break;
                case ItemIdEnum.LargeLifePotion:
                    recoveryPoints = Constants.LARGE_LIFE_POTION_RECOVERY_LIFE_POINTS;
                    break;
            }
            RegenerateLifePoints(recoveryPoints);
        }
    }
}
