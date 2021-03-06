﻿using System.Collections.Generic;
using System.Linq;
using RpeggiatorLib.Enums;
using RpeggiatorLib.Exceptions;
using RpeggiatorLib.Renders;

namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents the player.
    /// </summary>
    /// <seealso cref="LifeSprite"/>
    public class Player : LifeSprite
    {
        // History of movements.
        private Queue<Point> _moveHistory = new Queue<Point>(Constants.MOVE_HISTORY_COUNT);
        // Delay, in milliseconds, between two hits with the current weapon.
        private readonly double _currentWeaponHitDelay;
        // Movement time manager.
        private Elapser _movementTimeManager;
        // Lifetime manager for the current hit with the current weapon.
        private Elapser _hitElapser;
        // Render while holding a shield.
        private readonly IRender _renderShield;
        // Render while recovering and holding a shield.
        private readonly IRender _renderRecoveryShield;
        // Render while holding a shield.
        private readonly IRender _renderSword;
        // Render while recovering and holding a shield.
        private readonly IRender _renderRecoverySword;

        /// <summary>
        /// When coming into a new screen, indicates the direction relative to the former screen.
        /// </summary>
        public Direction? NewScreenEntrance { get; private set; }
        /// <summary>
        /// Inventory.
        /// </summary>
        public Inventory Inventory { get; private set; }
        /// <summary>
        /// Indicates if the player is currently hitting.
        /// </summary>
        public bool IsHitting { get { return _hitElapser != null; } }
        /// <summary>
        /// Hit <see cref="Sprite"/>.
        /// </summary>
        public SwordHit SwordHitSprite { get; private set; }
        /// <inheritdoc />
        public override IRender Render
        {
            get
            {
                return IsHitting ? (IsRecovering ? _renderRecoverySword : _renderSword) : (Engine.Default.KeyPress.PressShield ?
                    (IsRecovering ? _renderRecoveryShield : _renderShield) : base.Render);
            }
        }
        /// <summary>
        /// Gets the latest move.
        /// </summary>
        internal Point? LatestMove { get { return _moveHistory.Count > 0 ? _moveHistory.Last() : (Point?)null; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>Every initial values come from <see cref="Constants.Player"/>.</remarks>
        internal Player() : base(
            0,
            Constants.Player.INITIAL_X,
            Constants.Player.INITIAL_Y,
            Constants.Player.SPRITE_WIDTH,
            Constants.Player.SPRITE_HEIGHT,
            Constants.Player.MAXIMAL_LIFE_POINTS,
            Constants.Player.HIT_LIFE_POINT_COST,
            Constants.Player.INITIAL_SPEED,
            Constants.Player.RECOVERY_TIME,
            nameof(Filename.Player),
            nameof(Filename.PlayerRecovery),
            Direction.Right)
        {
            NewScreenEntrance = null;
            Inventory = new Inventory();
            SwordHitSprite = null;
            _hitElapser = null;
            _currentWeaponHitDelay = Constants.Player.SWORD_HIT_DELAY;
            _movementTimeManager = new Elapser(this, ElapserUse.PlayerMovement);
            _renderShield = DefaultRender.BasicImage(this, nameof(Filename.PlayerShield));
            _renderRecoveryShield = DefaultRender.AnimatedBasicImage(this,
                nameof(Filename.PlayerRecoveryShield), ElapserUse.LifeSpriteRecovery, Constants.RECOVERY_BLINK_DELAY);
            _renderSword = DefaultRender.BasicImage(this, nameof(Filename.PlayerSword));
            _renderRecoverySword = DefaultRender.AnimatedBasicImage(this,
                nameof(Filename.PlayerRecoverySword), ElapserUse.LifeSpriteRecovery, Constants.RECOVERY_BLINK_DELAY);
        }

        /// <inheritdoc />
        internal override void BehaviorAtNewFrame()
        {
            NewScreenEntrance = null;
            Point newPosition = Tools.ComputeMovementNextPointInDirection(X, Y, _movementTimeManager.Distance(Speed), Engine.Default.KeyPress.Direction);

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
                foreach (Chest chest in Engine.Default.CurrentScreen.ClosedChests)
                {
                    if (chest.Overlap(ResizeToRatio(Constants.Player.ACTION_RANGE)) && chest.PlayerIsLookingTo())
                    {
                        chest.TryOpen();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Update the player lifepoints after drinking a potion.
        /// </summary>
        /// <param name="recoveryPoints">Lifepoints gained.</param>
        internal void DrinkLifePotion(double recoveryPoints)
        {
            RegenerateLifePoints(recoveryPoints);
        }

        /// <summary>
        /// Resets player position when he goes through a door and changes screen.
        /// </summary>
        /// <param name="doorId">The door identifier.</param>
        internal void SetPositionRelativeToDoorGoThrough(int doorId)
        {
            Door doorInNewScreen = Engine.Default.CurrentScreen.Doors.First(d => d.Id == doorId);
            Move(doorInNewScreen.PlayerGoThroughX, doorInNewScreen.PlayerGoThroughY);
        }

        #region Position management private methods

        /// <summary>
        /// Checks, for a theoretical new position, if its avoid every solid structures of the screen; the position might be edited.
        /// </summary>
        /// <param name="newPosition">The position to check; might be edited inside the function.</param>
        /// <exception cref="InfiniteOverlapCheckException"><see cref="Messages.InfiniteOverlapCheckExceptionMessage"/></exception>
        private void CheckPotentialOverlapAndAdjustPosition(ref Point newPosition)
        {
            List<Point> forbiddens = new List<Point>();
            bool findGoodSpot = false;
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
                    NewScreenEntrance = Direction.TopLeft;
                    newPosition.Y = Engine.Default.CurrentScreen.Height - Height;
                }
                else if (goDown)
                {
                    NewScreenEntrance = Direction.BottomLeft;
                    newPosition.Y = 0;
                }
                else
                {
                    NewScreenEntrance = Direction.Left;
                }
            }
            else if (goRight)
            {
                newPosition.X = 0;
                if (goUp)
                {
                    NewScreenEntrance = Direction.TopRight;
                    newPosition.Y = Engine.Default.CurrentScreen.Height - Height;
                }
                else if (goDown)
                {
                    NewScreenEntrance = Direction.BottomRight;
                    newPosition.Y = 0;
                }
                else
                {
                    NewScreenEntrance = Direction.Right;
                }
            }
            else if (goUp)
            {
                NewScreenEntrance = Direction.Top;
                newPosition.Y = Engine.Default.CurrentScreen.Height - Height;
            }
            else
            {
                NewScreenEntrance = Direction.Bottom;
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
            _moveHistory.Enqueue(new Point(X, Y));
            if (_moveHistory.Count > Constants.MOVE_HISTORY_COUNT)
            {
                _moveHistory.Dequeue();
            }
            Move(newPosition.X, newPosition.Y);
        }

        #endregion Position management private methods

        // Manages the main weapon hit.
        private void ManageHit()
        {
            if (Engine.Default.KeyPress.PressHit && _hitElapser == null)
            {
                _hitElapser = new Elapser(this, ElapserUse.PlayerSwordManagement, _currentWeaponHitDelay);
                // Sprite coordinates are not important here, as we compute them below.
                SwordHitSprite = new SwordHit(Id, 0, 0, 0, 0);
            }
            else if (_hitElapser?.Elapsed == true)
            {
                _hitElapser = null;
            }
            if (_hitElapser == null)
            {
                SwordHitSprite = null;
            }

            SwordHitSprite?.AdjustToPlayer();
        }

        /// <inheritdoc />
        protected override double ComputeLifePointCostFromEnemies()
        {
            return Engine.Default.CheckHitByEnemiesOnPlayer();
        }
    }
}
