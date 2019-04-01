namespace RPG4.Abstraction.Sprites
{
    /// <summary>
    /// Represents a structure which can be destroyed by a <see cref="ActionnedBomb"/>.
    /// </summary>
    /// <seealso cref="Sprite"/>
    /// <seealso cref="IExplodable"/>
    public class Rift : Sprite, IExplodable
    {
        // Indicates the life points cost when a bomb explodes nearby.
        private const double EXPLOSION_LIFE_POINT_COST = 5;

        /// <summary>
        /// Life points count.
        /// </summary>
        public double LifePoints { get; private set; }
        /// <inheritdoc />
        public double ExplosionLifePointCost { get { return EXPLOSION_LIFE_POINT_COST; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="riftJson">The json dynamic object.</param>
        public Rift(dynamic riftJson) : base((object)riftJson)
        {
            LifePoints = riftJson.LifePoints;
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame(Engine engine, params object[] args)
        {
            LifePoints -= engine.CurrentScreen.OverlapAnExplodingBomb(this);
        }
    }
}
