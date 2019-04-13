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
        /// Constructor.
        /// </summary>
        /// <param name="enemyJson">The json dynamic object.</param>
        internal Enemy(dynamic enemyJson) : base((object)enemyJson, "Enemy", "Enemy")
        {
            Direction = enemyJson.DefaultDirection;
            _movementTimeManager = new Elapser();
            List<Point> points = new List<Point> { TopLeftCorner };
            foreach (dynamic jsonPath in enemyJson.Path)
            {
                points.Add(new Point((double)jsonPath.X, (double)jsonPath.Y));
            }
            _path = new Path(points.ToArray());
            LootItemType = enemyJson.LootItemType;
            LootQuantity = enemyJson.LootQuantity;
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

            X = newPoint.X;
            Y = newPoint.Y;
        }
    }
}
