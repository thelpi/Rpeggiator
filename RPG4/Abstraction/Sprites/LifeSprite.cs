using System.Linq;

namespace RPG4.Abstraction.Sprites
{
    /// <summary>
    /// Represents a <see cref="Sprite"/> with a life status.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class LifeSprite : Sprite
    {
        /// <summary>
        /// Maximal number of life points.
        /// </summary>
        public double MaximalLifePoints { get; private set; }
        /// <summary>
        /// Current number of life points.
        /// </summary>
        public double CurrentLifePoints { get; private set; }
        /// <summary>
        /// When hitting, indicates the life points cost on the enemy.
        /// </summary>
        public double HitLifePointCost { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="maximalLifePoints"><see cref="MaximalLifePoints"/></param>
        /// <param name="hitLifePointCost"><see cref="HitLifePointCost"/></param>
        public LifeSprite(double x, double y, double width, double height, double maximalLifePoints, double hitLifePointCost)
            : base(x, y, width, height)
        {
            MaximalLifePoints = maximalLifePoints;
            CurrentLifePoints = maximalLifePoints;
            HitLifePointCost = hitLifePointCost;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="lifeSpriteJson">The json dynamic object.</param>
        public LifeSprite(dynamic lifeSpriteJson) : base((object)lifeSpriteJson)
        {
            MaximalLifePoints = lifeSpriteJson.MaximalLifePoints;
            CurrentLifePoints = MaximalLifePoints;
            HitLifePointCost = lifeSpriteJson.HitLifePointCost;
        }

        /// <summary>
        /// Recomputes <see cref="CurrentLifePoints"/> after an hit.
        /// </summary>
        /// <param name="lifePoints">Life points lost in the process.</param>
        protected void Hit(double lifePoints)
        {
            CurrentLifePoints -= lifePoints;
        }

        /// <summary>
        /// Checks if the instance is in-game dead.
        /// </summary>
        /// <param name="engine"><see cref="AbstractEngine"/></param>
        /// <returns><c>True</c> if death; <c>False</c> otherwise.</returns>
        public bool CheckDeath(AbstractEngine engine)
        {
            if (engine.SolidStructures.Any(ss => ss.Overlap(this))
                || engine.Pits.Any(r => r.CanFallIn(this) && (GetType() == typeof(Enemy) || r.Deadly)))
            {
                CurrentLifePoints = 0;
            }

            return CurrentLifePoints <= 0;
        }

        /// <summary>
        /// Regenerate instance life points.
        /// </summary>
        /// <param name="lifePoints">Life points gained count.</param>
        protected void RegenerateLifePoints(double lifePoints)
        {
            if (lifePoints > 0)
            {
                CurrentLifePoints += lifePoints;
                CurrentLifePoints = CurrentLifePoints > MaximalLifePoints ? MaximalLifePoints : CurrentLifePoints;
            }
        }
    }
}
