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

        /// <summary>
        /// Loot <see cref="Enums.ItemType"/>; <c>Null</c> for coin.
        /// </summary>
        public Enums.ItemType? LootItemType { get; private set; }
        /// <summary>
        /// Loot quantity.
        /// </summary>
        public int LootQuantity { get; private set; }

        /// <summary>
        /// Creats an instance from json datas.
        /// </summary>
        /// <param name="datas">Json datas.</param>
        /// <returns><see cref="Enemy"/></returns>
        internal static Enemy FromDynamic(dynamic datas)
        {
            List<Point> steps = new List<Point>();
            foreach (dynamic jsonPath in datas.Path)
            {
                steps.Add(new Point((double)jsonPath.X, (double)jsonPath.Y));
            }

            return new Enemy((double)datas.X, (double)datas.Y, (double)datas.Height, (double)datas.Width,
                (double)datas.MaximalLifePoints, (double)datas.HitLifePointCost, (double)datas.Speed, (double)datas.RecoveryTime,
                (string)datas.RenderFilename, (string)datas.RenderRecoveryFilename, (Enums.Direction)datas.DefaultDirection,
                (Enums.ItemType?)datas.LootItemType, (int)datas.LootQuantity, steps);
        }

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
        /// <param name="pathSteps">Steps of <see cref="_path"/>; except the current position.</param>
        internal Enemy(double x, double y, double width, double height, double maximalLifePoints, double hitLifePointCost,
            double speed, double recoveryTime, string renderFilename, string renderRecoveryFilename,
            Enums.Direction defaultDirection, Enums.ItemType? lootItemType, int lootQuantity, IEnumerable<Point> pathSteps)
            : base(x, y, width, height, maximalLifePoints, hitLifePointCost, speed, recoveryTime, renderFilename,
                  renderRecoveryFilename, defaultDirection)
        {
            _movementTimeManager = new Elapser();
            List<Point> points = new List<Point> { TopLeftCorner };
            points.AddRange(pathSteps);
            _path = new Path(points.ToArray());
            LootItemType = lootItemType;
            LootQuantity = lootQuantity;
        }

        /// <inheritdoc />
        internal override void BehaviorAtNewFrame()
        {
            SetCoordinatesDirection(_path.ComputeNextPosition(this, _movementTimeManager.Distance(Speed)));
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
    }
}
