using System.Linq;

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

        /// <summary>
        /// Speed, in pixels by second.
        /// </summary>
        public double Speed { get; private set; }
        /// <summary>
        /// Movement pattern.
        /// </summary>
        public Sprite Pattern { get; private set; }
        /// <summary>
        /// Indicates the current rotation on <see cref="Pattern"/>.
        /// </summary>
        public bool HourRotation { get; private set; }
        /// <inheritdoc />
        public double ExplosionLifePointCost { get { return EXPLOSION_LIFE_POINT_COST; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="enemyJson">The json dynamic object.</param>
        public Enemy(dynamic enemyJson) : base((object)enemyJson)
        {
            Speed = enemyJson.Speed;
            HourRotation = enemyJson.HourRotation;
            Pattern = new Sprite(enemyJson.Pattern);
            _movementTimeManager = new Elapser();
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame(AbstractEngine engine, params object[] args)
        {
            double distance = _movementTimeManager.Distance(Speed);

            double nextX = X;
            double nextY = Y;

            bool isRight = X + Width >= Pattern.BottomRightX;
            bool isDown = Y + Height >= Pattern.BottomRightY;
            bool isLeft = X <= Pattern.X;
            bool isUp = Y <= Pattern.Y;

            // si à droite
            if (isRight)
            {
                // si en bas (horaire) ou en haut (antihoraire)
                if ((HourRotation && isDown) || (!HourRotation && isUp))
                {
                    // vers la gauche
                    nextX += distance * -1;
                }
                else
                {
                    // vers le bas (horaire) ou vers le haut (antihoraire)
                    nextY += distance * (HourRotation ? 1 : -1);
                }
            }
            // si à gauche
            else if (isLeft)
            {
                // si en haut (horaire) ou en bas (antihoraire)
                if ((HourRotation && isUp) || (!HourRotation && isDown))
                {
                    // vers la droite
                    nextX += distance;
                }
                else
                {
                    // vers le haut (horaire) ou vers le bas (antihoraire)
                    nextY += distance * (HourRotation ? -1 : 1);
                }
            }
            // sinon
            else
            {
                // si en bas
                if (isDown)
                {
                    // vers la gauche (horaire) ou la droite (antihoraire)
                    nextX += distance * (HourRotation ? -1 : 1);
                }
                // si en haut
                else if (isUp)
                {
                    // vers la droite (horaire) ou la gauche (antihoraire)
                    nextX += distance * (HourRotation ? 1 : -1);
                }
            }

            if (engine.SolidStructures.Any(s => s.Overlap(CopyToPosition(new System.Windows.Point(nextX, nextY)))))
            {
                HourRotation = !HourRotation;
            }
            else
            {
                X = nextX;
                Y = nextY;
            }
        }

        /// <summary>
        /// Checks if the instance has been hit.
        /// </summary>
        public void CheckIfHasBeenHit(AbstractEngine engine)
        {
            bool hasBeenHit = false;

            // hit by player ?
            if (engine.Player.IsHitting && Overlap(engine.Player.HitSprite))
            {
                Hit(engine.Player.HitLifePointCost);
                hasBeenHit = true;
            }

            // hit by a bomb ?
            double lifePointCostByBomb = engine.OverlapAnExplodingBomb(this);
            if (lifePointCostByBomb > 0)
            {
                Hit(lifePointCostByBomb);
                hasBeenHit = true;
            }

            if (hasBeenHit)
            {
                HourRotation = !HourRotation;
            }
        }
    }
}
