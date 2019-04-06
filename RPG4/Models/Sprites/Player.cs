﻿using RPG4.Models.Enums;
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
    public class Player : LifeSprite
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
        /// When coming into a new screen, indicates the direction relative to the former screen.
        /// </summary>
        public Direction? NewScreenEntrance { get; private set; }
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
        public Direction Direction { get; private set; }
        /// <summary>
        /// Graphic rendering when recovering.
        /// </summary>
        public ISpriteGraphic RecoveryGraphic { get { return Constants.Player.RECOVERY_GRAPHIC; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>Every initial values come from <see cref="Constants.Player"/>.</remarks>
        public Player() : base(
            Constants.Player.INITIAL_PLAYER_X,
            Constants.Player.INITIAL_PLAYER_Y,
            Constants.Player.SPRITE_SIZE_X,
            Constants.Player.SPRITE_SIZE_Y,
            Constants.Player.GRAPHIC,
            Constants.Player.MAXIMAL_LIFE_POINTS,
            Constants.Player.HIT_LIFE_POINT_COST,
            Constants.Player.INITIAL_PLAYER_SPEED)
        {
            _creationHashcode = DateTime.Now.ToString(Constants.UNIQUE_TIMESTAMP_PATTERN).GetHashCode();

            NewScreenEntrance = null;
            Inventory = new Inventory(_creationHashcode);
            HitSprite = null;
            _recoveryManager = null;
            _hitElapser = null;
            _currentWeaponHitDelay = Constants.Player.SWORD_HIT_DELAY;
            Direction = Direction.Right;
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
                    if (chest.Overlap(ResizeToRatio(Constants.Player.ACTION_RANGE)) && chest.PlayerIsLookingTo())
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
                    _recoveryManager = new Elapser(Constants.Player.RECOVERY_TIME);
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
                case Direction.Bottom:
                    newTop += distance;
                    break;
                case Direction.BottomLeft:
                    newTop += Tools.FrameDiagonalDistance(distance);
                    newLeft -= Tools.FrameDiagonalDistance(distance);
                    break;
                case Direction.BottomRight:
                    newTop += Tools.FrameDiagonalDistance(distance);
                    newLeft += Tools.FrameDiagonalDistance(distance);
                    break;
                case Direction.Top:
                    newTop -= distance;
                    break;
                case Direction.TopLeft:
                    newTop -= Tools.FrameDiagonalDistance(distance);
                    newLeft -= Tools.FrameDiagonalDistance(distance);
                    break;
                case Direction.TopRight:
                    newTop -= Tools.FrameDiagonalDistance(distance);
                    newLeft += Tools.FrameDiagonalDistance(distance);
                    break;
                case Direction.Left:
                    newLeft -= distance;
                    break;
                case Direction.Right:
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
                    case Direction.Bottom:
                        hitY += Height;
                        break;
                    case Direction.BottomLeft:
                        hitY += Height;
                        hitX -= Width;
                        break;
                    case Direction.BottomRight:
                        hitY += Height;
                        hitX += Width;
                        break;
                    case Direction.Left:
                        hitX -= Width;
                        break;
                    case Direction.Right:
                        hitX += Width;
                        break;
                    case Direction.TopRight:
                        hitY -= Height;
                        hitX += Width;
                        break;
                    case Direction.TopLeft:
                        hitY -= Height;
                        hitX -= Width;
                        break;
                    case Direction.Top:
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
