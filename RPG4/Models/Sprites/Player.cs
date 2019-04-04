using RPG4.Models.Exceptions;
using RPG4.Models.Graphic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RPG4.Models.Sprites
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
        public DirectionEnum? NewScreenEntrance { get; private set; }
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
        public WeaponHit HitSprite { get; private set; }
        /// <summary>
        /// Indicates the sprite direction.
        /// </summary>
        public DirectionEnum Direction { get; private set; }
        /// <inheritdoc />
        public double ExplosionLifePointCost { get { return InitialPlayerStatus.EXPLOSION_LIFE_POINT_COST; } }
        /// <summary>
        /// Graphic rendering when recovering.
        /// </summary>
        public ISpriteGraphic RecoveryGraphic { get { return InitialPlayerStatus.RECOVERY_GRAPHIC; } }
        /// <summary>
        /// Inferred; current <see cref="FloorTypeEnum"/> the player is standing on.
        /// </summary>
        public FloorTypeEnum CurrentFloor
        {
            get
            {
                return Engine.Default.CurrentScreen.Floors.FirstOrDefault(f =>
                    Overlap(f, Constants.FLOOR_CHANGE_OVERLAP_RATIO)
                )?.FloorType ?? Engine.Default.CurrentScreen.FloorType;
            }
        }

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
            Direction = DirectionEnum.Right;
            _movementTimeManager = new Elapser();
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame()
        {
            NewScreenEntrance = null;
            Point newPosition = ComputeTheoreticalMove();

            // If any movement.
            if (!newPosition.X.Equal(X) || !newPosition.Y.Equal(Y))
            {
                CheckPotentialOverlapAndAdjustPosition(ref newPosition);

                CheckNewScreenEntrance(ref newPosition);

                Direction = Engine.Default.KeyPress.Direction ?? Direction;

                AssigneNewPositionAndAddToHistory(newPosition);
            }

            ManageHit();

            // Proceeds to proximity actions (open chests...)
            // Single action at a time.
            if (Engine.Default.KeyPress.PressAction)
            {
                foreach (var chest in Engine.Default.CurrentScreen.ClosedChests)
                {
                    if (chest.Overlap(ResizeToRatio(InitialPlayerStatus.ACTION_RANGE)) && chest.PlayerIsLookingTo())
                    {
                        chest.TryOpen();
                        break;
                    }
                }
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

                if (cumuledLifePoints.Greater(0))
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

        /// <summary>
        /// Resets player position when he goes through a door and changes screen.
        /// </summary>
        /// <param name="doorId">The door identifier.</param>
        public void SetPositionRelativeToDoorGoThrough(int doorId)
        {
            Door doorInNewScreen = Engine.Default.CurrentScreen.Doors.First(d => d.Id == doorId);
            X = doorInNewScreen.PlayerGoThroughX;
            Y = doorInNewScreen.PlayerGoThroughY;
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

            switch (keys.Direction)
            {
                case DirectionEnum.Bottom:
                    newTop += distance;
                    break;
                case DirectionEnum.BottomLeft:
                    newTop += Tools.FrameDiagonalDistance(distance);
                    newLeft -= Tools.FrameDiagonalDistance(distance);
                    break;
                case DirectionEnum.BottomRight:
                    newTop += Tools.FrameDiagonalDistance(distance);
                    newLeft += Tools.FrameDiagonalDistance(distance);
                    break;
                case DirectionEnum.Top:
                    newTop -= distance;
                    break;
                case DirectionEnum.TopLeft:
                    newTop -= Tools.FrameDiagonalDistance(distance);
                    newLeft -= Tools.FrameDiagonalDistance(distance);
                    break;
                case DirectionEnum.TopRight:
                    newTop -= Tools.FrameDiagonalDistance(distance);
                    newLeft += Tools.FrameDiagonalDistance(distance);
                    break;
                case DirectionEnum.Left:
                    newLeft -= distance;
                    break;
                case DirectionEnum.Right:
                    newLeft += distance;
                    break;
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
                        Engine.Default.KeyPress.GoLeft ? true : (Engine.Default.KeyPress.GoRight ? false : (bool?)null),
                        Engine.Default.KeyPress.GoUp ? true : (Engine.Default.KeyPress.GoBottom ? false : (bool?)null));

                    if (pToMove.X.GreaterEqual(0) || pToMove.Y.GreaterEqual(0))
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
        /// <returns><c>True</c> if enters a new screen; <c>False</c> otherwise.</returns>
        private bool CheckNewScreenEntrance(ref Point newPosition)
        {
            bool goLeft = newPosition.X.Lower(0);
            bool goUp = newPosition.Y.Lower(0);
            bool goRight = (newPosition.X + Width).Greater(Engine.Default.CurrentScreen.Width);
            bool goDown = (newPosition.Y + Height).Greater(Engine.Default.CurrentScreen.Height);

            if (!goLeft && !goUp && !goRight && !goDown)
            {
                return false;
            }

            if (goLeft)
            {
                newPosition.X = Engine.Default.CurrentScreen.Width - Width;
                if (goUp)
                {
                    NewScreenEntrance = DirectionEnum.TopLeft;
                    newPosition.Y = Engine.Default.CurrentScreen.Height - Height;
                }
                else if (goDown)
                {
                    NewScreenEntrance = DirectionEnum.BottomLeft;
                    newPosition.Y = 0;
                }
                else
                {
                    NewScreenEntrance = DirectionEnum.Left;
                }
            }
            else if (goRight)
            {
                newPosition.X = 0;
                if (goUp)
                {
                    NewScreenEntrance = DirectionEnum.TopRight;
                    newPosition.Y = Engine.Default.CurrentScreen.Height - Height;
                }
                else if (goDown)
                {
                    NewScreenEntrance = DirectionEnum.BottomRight;
                    newPosition.Y = 0;
                }
                else
                {
                    NewScreenEntrance = DirectionEnum.Right;
                }
            }
            else if (goUp)
            {
                NewScreenEntrance = DirectionEnum.Top;
                newPosition.Y = Engine.Default.CurrentScreen.Height - Height;
            }
            else
            {
                NewScreenEntrance = DirectionEnum.Bottom;
                newPosition.Y = 0;
            }

            return true;
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

        // Manages the main weapon hit.
        private void ManageHit()
        {
            if (Engine.Default.KeyPress.PressHit && _hitElapser == null)
            {
                _hitElapser = new Elapser(_currentWeaponHitDelay);
                double hitX = X;
                double hitY = Y;
                switch (Direction)
                {
                    case DirectionEnum.Bottom:
                        hitY += Height;
                        break;
                    case DirectionEnum.BottomLeft:
                        hitY += Height;
                        hitX -= Width;
                        break;
                    case DirectionEnum.BottomRight:
                        hitY += Height;
                        hitX += Width;
                        break;
                    case DirectionEnum.Left:
                        hitX -= Width;
                        break;
                    case DirectionEnum.Right:
                        hitX += Width;
                        break;
                    case DirectionEnum.TopRight:
                        hitY -= Height;
                        hitX += Width;
                        break;
                    case DirectionEnum.TopLeft:
                        hitY -= Height;
                        hitX -= Width;
                        break;
                    case DirectionEnum.Top:
                        hitY -= Height;
                        break;
                }
                HitSprite = new WeaponHit(hitX, hitY, Width, Height);
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
    }
}
