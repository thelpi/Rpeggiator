using RPG4.Abstraction.Exceptions;
using RPG4.Abstraction.Graphic;
using System;
using System.Collections.Generic;
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
        // History of movements.
        private Queue<Point> _moveHistory = new Queue<Point>(Constants.MOVE_HISTORY_COUNT);
        // Delay, in milliseconds, between two hits with the current weapon.
        private double _currentWeaponHitDelay;
        // Movement time manager.
        private Elapser _movementTimeManager;
        // Lifetime manager for the current hit with the current weapon.
        private Elapser _hitElapser;
        // Recovery time manager.
        private Elapser _recoveryManager;
        // Hashcode associated to the instance timestamp.
        private int _creationHashcode;

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
        public bool IsRecovering { get { return _recoveryManager?.Elapsed == false; } }
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
            _creationHashcode = DateTime.Now.ToString(Constants.UNIQUE_TIMESTAMP_PATTERN).GetHashCode();

            Speed = InitialPlayerStatus.INITIAL_PLAYER_SPEED;
            NewScreenEntrance = null;
            Inventory = new Inventory(_creationHashcode);
            HitSprite = null;
            _recoveryManager = null;
            _hitElapser = null;
            _currentWeaponHitDelay = InitialPlayerStatus.SWORD_HIT_DELAY;
            LastDirection = Directions.right;
            _movementTimeManager = new Elapser();
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame()
        {
            NewScreenEntrance = null;
            Point newPosition = ComputeTheoreticalMove();

            // If any movement.
            if (newPosition.X != X || newPosition.Y != Y)
            {
                CheckPotentialOverlapAndAdjustPosition(ref newPosition);

                CheckNewScreenEntrance(ref newPosition, Engine.Default.CurrentScreen.Width, Engine.Default.CurrentScreen.Height);

                SetDirection(newPosition);

                AssigneNewPositionAndAddToHistory(newPosition);
            }

            ManageHit();

            // Opens proximity chest (one at a time).
            if (Engine.Default.KeyPress.PressAction)
            {
                foreach (var chest in Engine.Default.CurrentScreen.ClosedChests)
                {
                    if (chest.Overlap(ResizeToRatio(InitialPlayerStatus.ACTION_RANGE)))
                    {
                        chest.TryOpen();
                        break;
                    }
                }
            }
        }

        // Manages the main weapon hit.
        private void ManageHit()
        {
            if (Engine.Default.KeyPress.PressHit && _hitElapser == null)
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
        public void CheckIfHasBeenHit()
        {
            // currently recovering ?
            if (_recoveryManager?.Elapsed == true)
            {
                _recoveryManager = null;
            }

            if (_recoveryManager == null)
            {
                // checks hits by enemies or bombs
                var cumuledLifePoints = Engine.Default.CheckHitByEnemiesOnPlayer() + Engine.Default.CurrentScreen.OverlapAnExplodingBomb(this);

                if (cumuledLifePoints > 0)
                {
                    Hit(cumuledLifePoints);
                    _recoveryManager = new Elapser(InitialPlayerStatus.RECOVERY_TIME);
                }
            }
        }

        /// <summary>
        /// Update the player lifepoints after drinking a potion.
        /// </summary>
        /// <param name="creationHashcode">Ensure the call is made by the player <see cref="Inventory"/>.</param>
        /// <param name="recoveryPoints">Lifepoints gained.</param>
        public void DrinkLifePotion(int creationHashcode, double recoveryPoints)
        {
            if (_creationHashcode != creationHashcode)
            {
                return;
            }

            RegenerateLifePoints(recoveryPoints);
        }

        #region Position management private methods

        /// <summary>
        /// Computes the next theoretical position.
        /// </summary>
        /// <returns>The new position coordinates, which might be the same as current coordinates.</returns>
        private Point ComputeTheoreticalMove()
        {
            // Shortcut.
            KeyPress keys = Engine.Default.KeyPress;

            double newTop = Y;
            double newLeft = X;
            double distance = _movementTimeManager.Distance(Speed);

            if (keys.PressUp)
            {
                if (keys.PressLeft)
                {
                    newTop -= Tools.FrameDiagonalDistance(distance);
                    newLeft -= Tools.FrameDiagonalDistance(distance);
                }
                else if (keys.PressRight)
                {
                    newTop -= Tools.FrameDiagonalDistance(distance);
                    newLeft += Tools.FrameDiagonalDistance(distance);
                }
                else
                {
                    newTop -= distance;
                }
            }
            else if (keys.PressDown)
            {
                if (keys.PressLeft)
                {

                    newTop += Tools.FrameDiagonalDistance(distance);
                    newLeft -= Tools.FrameDiagonalDistance(distance);
                }
                else if (keys.PressRight)
                {
                    newTop += Tools.FrameDiagonalDistance(distance);
                    newLeft += Tools.FrameDiagonalDistance(distance);
                }
                else
                {
                    newTop += distance;
                }
            }
            else if (keys.PressLeft)
            {
                newLeft -= distance;
            }
            else if (keys.PressRight)
            {
                newLeft += distance;
            }

            return new Point(newLeft, newTop);
        }

        /// <summary>
        /// Checks, for a theoretical new position, if its avoid every solid structures of the screen; the position might be edited.
        /// </summary>
        /// <param name="newPosition">The position to check; might be edited inside the function.</param>
        /// <exception cref="InfiniteOverlapCheckException"><see cref="Messages.InfiniteOverlapCheckExceptionMessage"/></exception>
        private void CheckPotentialOverlapAndAdjustPosition(ref Point newPosition)
        {
            var forbiddens = new List<Point>();
            var findGoodSpot = false;
            while (!findGoodSpot)
            {
                Sprite currentPt = CopyToPosition(newPosition);
                findGoodSpot = true;
                foreach (Sprite sprite in Engine.Default.CurrentScreen.Structures)
                {
                    Point pToMove = sprite.CheckOverlapAndAdjustPosition(currentPt, this,
                        Engine.Default.KeyPress.PressLeft ? true : (Engine.Default.KeyPress.PressRight ? false : (bool?)null),
                        Engine.Default.KeyPress.PressUp ? true : (Engine.Default.KeyPress.PressDown ? false : (bool?)null));

                    if (pToMove.X >= 0 || pToMove.Y >= 0)
                    {
                        forbiddens.Add(new Point(currentPt.X, currentPt.Y));
                        newPosition.X = pToMove.X;
                        newPosition.Y = pToMove.Y;
                        if (forbiddens.Contains(newPosition))
                        {
                            throw new InfiniteOverlapCheckException(this, Engine.Default.CurrentScreen.Structures, newPosition);
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
