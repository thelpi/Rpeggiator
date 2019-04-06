using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System;

namespace RPG4.Models.Sprites
{
    /// <summary>
    /// Represents an enemy.
    /// </summary>
    /// <seealso cref="LifeSprite"/>
    /// <see cref="IExplodable"/>
    public class Enemy : LifeSprite
    {
        // Indicates the life points cost when a bomb explodes nearby.
        private const double EXPLOSION_LIFE_POINT_COST = 2;
        // Movement time manager.
        private Elapser _movementTimeManager;
        // Movement path.
        private Path _path;
        
        /// <summary>
        /// Loot <see cref="ItemEnum"/>; <c>Null</c> for coin.
        /// </summary>
        public ItemEnum? LootItemId { get; private set; }
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
            foreach (var jsonPath in enemyJson.Path)
            {
                points.Add(new Point((double)jsonPath.X, (double)jsonPath.Y));
            }
            _path = new Path(points.ToArray());
            LootItemId = enemyJson.LootItemId;
            LootQuantity = enemyJson.LootQuantity;
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame()
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

        /// <summary>
        /// Checks if the instance has been hit.
        /// </summary>
        public void CheckIfHasBeenHit()
        {
            bool hasBeenHit = false;

            // hit by player ?
            if (Engine.Default.Player.IsHitting && Overlap(Engine.Default.Player.HitSprite))
            {
                Hit(Engine.Default.Player.HitLifePointCost);
                hasBeenHit = true;
            }

            // hit by a bomb ?
            double lifePointCostByBomb = Engine.Default.CurrentScreen.OverlapAnExplodingBomb(this);
            if (lifePointCostByBomb.Greater(0))
            {
                Hit(lifePointCostByBomb);
                hasBeenHit = true;
            }

            if (hasBeenHit)
            {
                _path.ReversePath();
            }
        }
    }
}
