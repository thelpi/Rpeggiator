using System.Collections.Generic;
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
        // Path.
        private List<Point> _path;
        // Next index on _path
        private int _nextPointIndex;
        // Reversed path.
        private bool _reversedPath;

        /// <summary>
        /// Speed, in pixels by second.
        /// </summary>
        public double Speed { get; private set; }
        /// <inheritdoc />
        public double ExplosionLifePointCost { get { return EXPLOSION_LIFE_POINT_COST; } }

        /// <summary>
        /// Freeze the instance movements by reseting the <see cref="Elapser"/>.
        /// </summary>
        public void Freeze()
        {
            _movementTimeManager.Reset();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="enemyJson">The json dynamic object.</param>
        public Enemy(dynamic enemyJson) : base((object)enemyJson)
        {
            Speed = enemyJson.Speed;
            _movementTimeManager = new Elapser();
            _path = new List<Point>();
            foreach (var jsonPath in enemyJson.Path)
            {
                _path.Add(new Point((double)jsonPath.X, (double)jsonPath.Y));
            }
            _path.Add(TopLeftCorner);
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame(Engine engine, params object[] args)
        {
            double distance = _movementTimeManager.Distance(Speed);

            double nextX = X;
            double nextY = Y;

            var pt = Tools.GetPointOnLine(TopLeftCorner, _path[_nextPointIndex], distance);
            nextX = pt.X;
            nextY = pt.Y;

            if (engine.CurrentScreen.SolidStructures.Any(s => s.Overlap(CopyToPosition(new Point(nextX, nextY)))))
            {
                _reversedPath = !_reversedPath;
                ComputeNextStepOnPath();
            }
            else
            {
                if (X < _path[_nextPointIndex].X && nextX >= _path[_nextPointIndex].X
                    || Y < _path[_nextPointIndex].Y && nextY >= _path[_nextPointIndex].Y
                    || X > _path[_nextPointIndex].X && nextX <= _path[_nextPointIndex].X
                    || Y > _path[_nextPointIndex].Y && nextY <= _path[_nextPointIndex].Y)
                {
                    ComputeNextStepOnPath();
                }
                X = nextX;
                Y = nextY;
            }
        }

        /// <summary>
        /// Computes the next step index on <see cref="_path"/>.
        /// </summary>
        private void ComputeNextStepOnPath()
        {
            _nextPointIndex = _reversedPath ?
                (_nextPointIndex - 1 < 0 ? _path.Count - 1 : _nextPointIndex - 1)
                : (_nextPointIndex + 1 == _path.Count ? 0 : _nextPointIndex + 1);
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
                _reversedPath = !_reversedPath;
            }
        }
    }
}
