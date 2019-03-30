using RPG4.Abstraction.Exceptions;
using RPG4.Abstraction.Graphic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RPG4.Abstraction.Sprites
{
    /// <summary>
    /// Represents the player.
    /// </summary>
    /// <seealso cref="LifeSprite"/>
    /// <see cref="IExplodable"/>
    public class Player : LifeSprite, IExplodable
    {
        // Recovery beginning timestamp.
        private DateTime _recoveryBeginTimestamp;
        // Time in recovery
        private TimeSpan? _recoveryTime;
        // History of movements.
        private Queue<Point> _moveHistory = new Queue<Point>(Constants.MOVE_HISTORY_COUNT);
        // Timestamp of latest frame.
        private DateTime _latestFrameTimestamp;
        // Delay, in milliseconds, between two hits with the current weapon.
        private double _currentWeaponHitDelay;
        // Lifetime manager for the current hit with the current weapon.
        private Elapser _hitElapser;

        /// <summary>
        /// Speed (i.e. distance, in pixels, by second)
        /// </summary>
        public double Speed { get; private set; }
        /// <summary>
        /// When coming into a new screen, indicates the direction relative to the former screen.
        /// </summary>
        public Directions? NewScreenEntrance { get; private set; }
        /// <summary>
        /// Inventory.
        /// </summary>
        public Inventory Inventory { get; private set; }
        /// <summary>
        /// Indicates the player is currently recovering from an hit.
        /// </summary>
        public bool IsRecovering { get { return _recoveryTime.HasValue; } }
        /// <summary>
        /// Indicates if the player is currently hitting.
        /// </summary>
        public bool IsHitting { get { return _hitElapser != null; } }
        /// <summary>
        /// Hit <see cref="Sprite"/>.
        /// </summary>
        public Sprite HitSprite { get; private set; }
        /// <summary>
        /// Indicates the sprite direction.
        /// </summary>
        public Directions LastDirection { get; private set; }
        /// <inheritdoc />
        public double ExplosionLifePointCost { get { return InitialPlayerStatus.EXPLOSION_LIFE_POINT_COST; } }
        /// <summary>
        /// Graphic rendering when recovering.
        /// </summary>
        public ISpriteGraphic RecoveryGraphic { get { return InitialPlayerStatus.RECOVERY_GRAPHIC; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>Every initial values come from <see cref="InitialPlayerStatus"/>.</remarks>
        public Player() : base(
            InitialPlayerStatus.INITIAL_PLAYER_X,
            InitialPlayerStatus.INITIAL_PLAYER_Y,
            InitialPlayerStatus.SPRITE_SIZE_X,
            InitialPlayerStatus.SPRITE_SIZE_Y,
            InitialPlayerStatus.GRAPHIC,
            InitialPlayerStatus.MAXIMAL_LIFE_POINTS,
            InitialPlayerStatus.HIT_LIFE_POINT_COST)
        {
            Speed = InitialPlayerStatus.INITIAL_PLAYER_SPEED;
            NewScreenEntrance = null;
            Inventory = new Inventory();
            HitSprite = null;
            _recoveryTime = null;
            _hitElapser = null;
            _currentWeaponHitDelay = InitialPlayerStatus.SWORD_HIT_DELAY;
            LastDirection = Directions.right;
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame(AbstractEngine engine, params object[] args)
        {
            var keys = args[0] as KeyPress;

            NewScreenEntrance = null;
            Point newPosition = ComputeTheoreticalMove(keys);

            // If any movement.
            if (newPosition.X != X || newPosition.Y != Y)
            {
                CheckPotentialOverlapAndAdjustPosition(ref newPosition, keys, engine.SolidStructures);

                CheckNewScreenEntrance(ref newPosition, engine.AreaWidth, engine.AreaHeight);

                SetDirection(newPosition);

                AssigneNewPositionAndAddToHistory(newPosition);
            }

            ManageHit(keys.PressHit);
        }

        // Manages the sword hit.
        private void ManageHit(bool pressHit)
        {
            if (pressHit && _hitElapser == null)
            {
                _hitElapser = new Elapser(_currentWeaponHitDelay);
                double hitX = X;
                double hitY = Y;
                switch (LastDirection)
                {
                    case Directions.bottom:
                        hitY += Height;
                        break;
                    case Directions.bottom_left:
                        hitY += Height;
                        hitX -= Width;
                        break;
                    case Directions.bottom_right:
                        hitY += Height;
                        hitX += Width;
                        break;
                    case Directions.left:
                        hitX -= Width;
                        break;
                    case Directions.right:
                        hitX += Width;
                        break;
                    case Directions.top_right:
                        hitY -= Height;
                        hitX += Width;
                        break;
                    case Directions.top_left:
                        hitY -= Height;
                        hitX -= Width;
                        break;
                    case Directions.top:
                        hitY -= Height;
                        break;
                }
                HitSprite = new Sprite(hitX, hitY, Width, Height, InitialPlayerStatus.HIT_GRAPHIC);
            }
            else if (_hitElapser?.Elapsed == true)
            {
                _hitElapser = null;
            }
            if (_hitElapser == null)
            {
                HitSprite = null;
            }
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
                _recoveryTime = DateTime.Now - _recoveryBeginTimestamp;
                if (_recoveryTime.Value.TotalMilliseconds > Constants.PLAYER_RECOVERY_TIME)
                {
                    _recoveryTime = null;
                }
            }

            if (!_recoveryTime.HasValue)
            {
                // checks hits by enemies or bombs
                var cumuledLifePoints = engine.CheckHitByEnemiesOnPlayer() + engine.OverlapAnExplodingBomb(this);

                if (cumuledLifePoints > 0)
                {
                    Hit(cumuledLifePoints);
                    _recoveryBeginTimestamp = DateTime.Now;
                    _recoveryTime = DateTime.Now - _recoveryBeginTimestamp;
                }
            }
        }

        /// <summary>
        /// Drinks a life potion.
        /// </summary>
        /// <param name="potionType"><see cref="ItemIdEnum"/>; ignored if not a life potion.</param>
        public void DrinkLifePotion(ItemIdEnum potionType)
        {
            double recoveryPoints = 0;
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

        #region Position management private methods

        /// <summary>
        /// Computes the next theoretical position; also sets <see cref="_latestFrameTimestamp"/>.
        /// </summary>
        /// <param name="keys"><see cref="KeyPress"/></param>
        /// <returns>The new position coordinates, which might be the same as current coordinates.</returns>
        private Point ComputeTheoreticalMove(KeyPress keys)
        {
            double newTop = Y;
            double newLeft = X;
            DateTime currentFrameTimestamp = DateTime.Now;
            TimeSpan delay = currentFrameTimestamp - _latestFrameTimestamp;
            double frameDistance = (delay.TotalMilliseconds / 1000) * Speed;

            if (keys.PressUp)
            {
                if (keys.PressLeft)
                {
                    newTop -= Tools.FrameDiagonalDistance(frameDistance);
                    newLeft -= Tools.FrameDiagonalDistance(frameDistance);
                }
                else if (keys.PressRight)
                {
                    newTop -= Tools.FrameDiagonalDistance(frameDistance);
                    newLeft += Tools.FrameDiagonalDistance(frameDistance);
                }
                else
                {
                    newTop -= frameDistance;
                }
            }
            else if (keys.PressDown)
            {
                if (keys.PressLeft)
                {

                    newTop += Tools.FrameDiagonalDistance(frameDistance);
                    newLeft -= Tools.FrameDiagonalDistance(frameDistance);
                }
                else if (keys.PressRight)
                {
                    newTop += Tools.FrameDiagonalDistance(frameDistance);
                    newLeft += Tools.FrameDiagonalDistance(frameDistance);
                }
                else
                {
                    newTop += frameDistance;
                }
            }
            else if (keys.PressLeft)
            {
                newLeft -= frameDistance;
            }
            else if (keys.PressRight)
            {
                newLeft += frameDistance;
            }
            _latestFrameTimestamp = currentFrameTimestamp;

            return new Point(newLeft, newTop);
        }

        /// <summary>
        /// Checks, for a theoretical new position, if its avoid every solid structures of the screen; the position might be edited.
        /// </summary>
        /// <param name="newPosition">The position to check; might be edited inside the function.</param>
        /// <param name="keys"><see cref="KeyPress"/></param>
        /// <param name="structures">List of <see cref="Sprite"/> to avoid.</param>
        /// <exception cref="InfiniteOverlapCheckException"><see cref="Messages.InfiniteOverlapCheckExceptionMessage"/></exception>
        private void CheckPotentialOverlapAndAdjustPosition(ref Point newPosition, KeyPress keys, IReadOnlyCollection<Sprite> structures)
        {
            var forbiddens = new List<Point>();
            var findGoodSpot = false;
            while (!findGoodSpot)
            {
                Sprite currentPt = CopyToPosition(newPosition);
                findGoodSpot = true;
                foreach (Sprite sprite in structures)
                {
                    Point pToMove = sprite.CheckOverlapAndAdjustPosition(currentPt, this,
                        keys.PressLeft ? true : (keys.PressRight ? false : (bool?)null),
                        keys.PressUp ? true : (keys.PressDown ? false : (bool?)null));

                    if (pToMove.X >= 0 || pToMove.Y >= 0)
                    {
                        forbiddens.Add(new Point(currentPt.X, currentPt.Y));
                        newPosition.X = pToMove.X;
                        newPosition.Y = pToMove.Y;
                        if (forbiddens.Contains(newPosition))
                        {
                            throw new InfiniteOverlapCheckException(this, structures, newPosition);
                        }
                        findGoodSpot = false;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if <paramref name="newPosition"/> triggers a new screen entrance.
        /// </summary>
        /// <param name="newPosition">The new position; might be edited inside the function.</param>
        /// <param name="areaWidth">Current area width.</param>
        /// <param name="areaHeight">Current area height.</param>
        /// <returns><c>True</c> if enters a new screen; <c>False</c> otherwise.</returns>
        private bool CheckNewScreenEntrance(ref Point newPosition, double areaWidth, double areaHeight)
        {
            bool goLeft = newPosition.X < 0;
            bool goUp = newPosition.Y < 0;
            bool goRight = newPosition.X + Width > areaWidth;
            bool goDown = newPosition.Y + Height > areaHeight;

            if (!goLeft && !goUp && !goRight && !goDown)
            {
                return false;
            }

            if (goLeft)
            {
                newPosition.X = areaWidth - Width;
                if (goUp)
                {
                    NewScreenEntrance = Directions.top_left;
                    newPosition.Y = areaHeight - Height;
                }
                else if (goDown)
                {
                    NewScreenEntrance = Directions.bottom_left;
                    newPosition.Y = 0;
                }
                else
                {
                    NewScreenEntrance = Directions.left;
                }
            }
            else if (goRight)
            {
                newPosition.X = 0;
                if (goUp)
                {
                    NewScreenEntrance = Directions.top_right;
                    newPosition.Y = areaHeight - Height;
                }
                else if (goDown)
                {
                    NewScreenEntrance = Directions.bottom_right;
                    newPosition.Y = 0;
                }
                else
                {
                    NewScreenEntrance = Directions.right;
                }
            }
            else if (goUp)
            {
                NewScreenEntrance = Directions.top;
                newPosition.Y = areaHeight - Height;
            }
            else
            {
                NewScreenEntrance = Directions.bottom;
                newPosition.Y = 0;
            }

            return true;
        }

        /// <summary>
        /// Sets <see cref="LastDirection"/> property.
        /// </summary>
        /// <param name="newPosition">new position.</param>
        private void SetDirection(Point newPosition)
        {
            if (NewScreenEntrance.HasValue)
            {
                LastDirection = NewScreenEntrance.Value;
            }
            else if (newPosition.X < X)
            {
                if (newPosition.Y > Y)
                {
                    LastDirection = Directions.bottom_left;
                }
                else if (newPosition.Y < Y)
                {
                    LastDirection = Directions.top_left;
                }
                else
                {
                    LastDirection = Directions.left;
                }
            }
            else if (newPosition.X > X)
            {
                if (newPosition.Y > Y)
                {
                    LastDirection = Directions.bottom_right;
                }
                else if (newPosition.Y < Y)
                {
                    LastDirection = Directions.top_right;
                }
                else
                {
                    LastDirection = Directions.right;
                }
            }
            else
            {
                if (newPosition.Y > Y)
                {
                    LastDirection = Directions.bottom;
                }
                else if (newPosition.Y < Y)
                {
                    LastDirection = Directions.top;
                }
            }
        }

        /// <summary>
        /// Sets <see cref="Sprite.X"/> and <see cref="Sprite.Y"/> properties with the new position, and adds it to <see cref="_moveHistory"/>.
        /// </summary>
        /// <param name="newPosition">The new position.</param>
        private void AssigneNewPositionAndAddToHistory(Point newPosition)
        {
            _moveHistory.Enqueue(newPosition);
            if (_moveHistory.Count > Constants.MOVE_HISTORY_COUNT)
            {
                _moveHistory.Dequeue();
            }

            X = newPosition.X;
            Y = newPosition.Y;
        }

        #endregion Position management private methods
    }
}
