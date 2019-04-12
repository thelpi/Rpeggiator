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
        internal Enemy(dynamic enemyJson) : base((object)enemyJson)
        {
            _movementTimeManager = new Elapser();
            List<Point> points = new List<Point> { TopLeftCorner };
            foreach (dynamic jsonPath in enemyJson.Path)
            {
                points.Add(new Point((double)jsonPath.X, (double)jsonPath.Y));
            }
            _path = new Path(points.ToArray());
            LootItemType = enemyJson.LootItemType;
            LootQuantity = enemyJson.LootQuantity;
            _render = new ImageRender("Enemy");
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
    }
}
