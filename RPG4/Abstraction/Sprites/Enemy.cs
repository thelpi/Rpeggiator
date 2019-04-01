﻿using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System;

namespace RPG4.Abstraction.Sprites
{
    /// <summary>
    /// Represents an enemy.
    /// </summary>
    /// <seealso cref="LifeSprite"/>
    /// <see cref="IExplodable"/>
    public class Enemy : LifeSprite, IExplodable
    {
        // Indicates the life points cost when a bomb explodes nearby.
        private const double EXPLOSION_LIFE_POINT_COST = 2;
        // Movement time manager.
        private Elapser _movementTimeManager;
        // Movement path.
        private Path _path;

        /// <summary>
        /// Speed, in pixels by second.
        /// </summary>
        public double Speed { get; private set; }
        /// <inheritdoc />
        public double ExplosionLifePointCost { get { return EXPLOSION_LIFE_POINT_COST; } }
        /// <summary>
        /// Loot <see cref="ItemIdEnum"/>; <c>Null</c> for coin.
        /// </summary>
        public ItemIdEnum? LootItemId { get; private set; }
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
            Speed = enemyJson.Speed;
            _movementTimeManager = new Elapser();
            List<Point> points = new List<Point> { TopLeftCorner };
            foreach (var jsonPath in enemyJson.Path)
            {
                points.Add(new Point((double)jsonPath.X, (double)jsonPath.Y));
            }
            _path = new Path(points.ToArray());
            LootItemId = enemyJson.LootItemId == null ? (ItemIdEnum?)null : (ItemIdEnum)Enum.Parse(typeof(ItemIdEnum), (string)enemyJson.LootItemId);
            LootQuantity = enemyJson.LootQuantity;
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame(Engine engine, params object[] args)
        {
            double distance = _movementTimeManager.Distance(Speed);

            double nextX = X;
            double nextY = Y;

            var pt = Tools.GetPointOnLine(TopLeftCorner, _path.GetCurrentStep(), distance);
            nextX = pt.X;
            nextY = pt.Y;

            if (_path.ComputeNextStep(this, nextX, nextY, engine.CurrentScreen))
            {
                X = nextX;
                Y = nextY;
            }
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
        public void CheckIfHasBeenHit(Engine engine)
        {
            bool hasBeenHit = false;

            // hit by player ?
            if (engine.Player.IsHitting && Overlap(engine.Player.HitSprite))
            {
                Hit(engine.Player.HitLifePointCost);
                hasBeenHit = true;
            }

            // hit by a bomb ?
            double lifePointCostByBomb = engine.CurrentScreen.OverlapAnExplodingBomb(this);
            if (lifePointCostByBomb > 0)
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
