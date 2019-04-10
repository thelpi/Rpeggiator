using System.Collections.Generic;
using System.Windows;

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
        public Enums.ItemType? LootItemId { get; private set; }
        /// <summary>
        /// Loot quantity.
        /// </summary>
        public int LootQuantity { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="enemyJson">The json dynamic object.</param>
        public Enemy(dynamic enemyJson) : base((object)enemyJson)
        {
            _movementTimeManager = new Elapser();
            List<Point> points = new List<Point> { TopLeftCorner };
            foreach (dynamic jsonPath in enemyJson.Path)
            {
                points.Add(new Point((double)jsonPath.X, (double)jsonPath.Y));
            }
            _path = new Path(points.ToArray());
            LootItemId = enemyJson.LootItemId;
            LootQuantity = enemyJson.LootQuantity;
        }

        /// <inheritdoc />
        internal override void BehaviorAtNewFrame()
        {
            Point pt = _path.ComputeNextPosition(this, _movementTimeManager.Distance(Speed));
            X = pt.X;
            Y = pt.Y;
        }

        /// <summary>
        /// Freeze the instance movements by reseting the <see cref="Elapser"/>.
        /// </summary>
        public void Freeze()
        {
            _movementTimeManager.Reset();
        }

        /// <inheritdoc />
        public override bool CheckIfHasBeenHit()
        {
            if (base.CheckIfHasBeenHit())
            {
                _path.ReversePath();
                return true;
            }
            return false;
        }
    }
}
