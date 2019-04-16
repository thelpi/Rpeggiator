using System.Collections.Generic;
using RpeggiatorLib.Render;

namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents an enemy.
    /// </summary>
    /// <seealso cref="LifeSprite"/>
    public class Enemy : LifeSprite
    {
        // Movement time manager.
        private Elapser _movementTimeManager;
        // Movement path.
        private Path _path;
        // Ensures non-authorized path redefinition.
        private bool _pathInitialized = false;

        /// <summary>
        /// Loot <see cref="Enums.ItemType"/>; <c>Null</c> for coin.
        /// </summary>
        public Enums.ItemType? LootItemType { get; private set; }
        /// <summary>
        /// Loot quantity.
        /// </summary>
        public int LootQuantity { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="maximalLifePoints"><see cref="DamageableSprite.CurrentLifePoints"/></param>
        /// <param name="hitLifePointCost"><see cref="LifeSprite.HitLifePointCost"/></param>
        /// <param name="speed"><see cref="LifeSprite.Speed"/></param>
        /// <param name="recoveryTime"><see cref="LifeSprite._recoveryTime"/></param>
        /// <param name="renderFilename"><see cref="Sprite._render"/></param>
        /// <param name="renderRecoveryFilename"><see cref="LifeSprite._renderRecovery"/></param>
        /// <param name="defaultDirection">Initial <see cref="LifeSprite.Direction"/>.</param>
        /// <param name="lootItemType"><see cref="LootItemType"/></param>
        /// <param name="lootQuantity"><see cref="LootQuantity"/></param>
        internal Enemy(double x, double y, double width, double height, double maximalLifePoints, double hitLifePointCost,
            double speed, double recoveryTime, string renderFilename, string renderRecoveryFilename,
            Enums.Direction defaultDirection, Enums.ItemType? lootItemType, int lootQuantity)
            : base(x, y, width, height, maximalLifePoints, hitLifePointCost, speed, recoveryTime, renderFilename,
                  renderRecoveryFilename, defaultDirection)
        {
            _movementTimeManager = new Elapser();
            LootItemType = lootItemType;
            LootQuantity = lootQuantity;
        }

        /// <summary>
        /// Initializes <see cref="_path"/>.
        /// </summary>
        /// <remarks>Duplicate calls will be ignored.</remarks>
        /// <param name="pathSteps">Steps of <see cref="_path"/>; except the current position.</param>
        internal void SetPath(IEnumerable<Point> pathSteps)
        {
            if (!_pathInitialized)
            {
                List<Point> points = new List<Point> { TopLeftCorner };
                points.AddRange(pathSteps);
                _path = new Path(points.ToArray());
                _pathInitialized = true;
            }
        }

        /// <inheritdoc />
        internal override void BehaviorAtNewFrame()
        {
            Point newPositionPoint = _path.ComputeNextPosition(this, _movementTimeManager.Distance(Speed));
            if (!TryToOverlapPlayerWithShield(newPositionPoint))
            {
                SetCoordinatesDirection(newPositionPoint);
            }
        }

        // Detects if the next motion tries to overlap the player while he's holding his shield in that direction.
        private bool TryToOverlapPlayerWithShield(Point newPositionPoint)
        {
            // Shortcut.
            Player p = Engine.Default.Player;

            return CopyToPosition(newPositionPoint).Overlap(p)
                && Engine.Default.KeyPress.PressShield
                && Tools.AreCloseDirections(p.Direction, p.DirectionSourceOfOverlap(this, newPositionPoint).Value);
        }

        /// <summary>
        /// Freeze the instance movements by reseting the <see cref="Elapser"/>.
        /// </summary>
        internal void Freeze()
        {
            _movementTimeManager.Reset();
        }

        /// <inheritdoc />
        internal override bool CheckIfHasBeenHit()
        {
            if (base.CheckIfHasBeenHit())
            {
                _path.ReversePath();
                return true;
            }
            return false;
        }

        // Sets the position from the new position compared to the current position.
        private void SetCoordinatesDirection(Point newPoint)
        {
            bool? fromLeft = newPoint.X.Equal(X) ? (bool?)null : newPoint.X.Greater(X);
            bool? fromTop = newPoint.Y.Equal(Y) ? (bool?)null : newPoint.Y.Greater(Y);

            if (fromLeft == true)
            {
                if (fromTop == true)
                {
                    Direction = Enums.Direction.BottomRight;
                }
                else if (fromTop == false)
                {
                    Direction = Enums.Direction.TopRight;
                }
                else
                {
                    Direction = Enums.Direction.Right;
                }
            }
            else if (fromLeft == false)
            {
                if (fromTop == true)
                {
                    Direction = Enums.Direction.BottomLeft;
                }
                else if (fromTop == false)
                {
                    Direction = Enums.Direction.TopLeft;
                }
                else
                {
                    Direction = Enums.Direction.Left;
                }
            }
            else
            {
                if (fromTop == true)
                {
                    Direction = Enums.Direction.Bottom;
                }
                else if (fromTop == false)
                {
                    Direction = Enums.Direction.Top;
                }
                else
                {
                    // No change.
                }
            }

            Move(newPoint.X, newPoint.Y);
        }

        /// <inheritdoc />
        protected override double ComputeLifePointCostFromEnemies()
        {
            if (Engine.Default.Player.IsHitting && Overlap(Engine.Default.Player.SwordHitSprite))
            {
                return Engine.Default.Player.HitLifePointCost;
            }

            return 0;
        }
    }
}
