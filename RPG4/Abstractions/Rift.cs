namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents a wall which can be destroyed by a <see cref="Bomb"/>.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class Rift : Sprite
    {
        /// <summary>
        /// Life points count.
        /// </summary>
        public double LifePoints { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="riftJson">The json dynamic object.</param>
        public Rift(dynamic riftJson) : base((object)riftJson)
        {
            LifePoints = riftJson.LifePoints;
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame(AbstractEngine engine, params object[] args)
        {
            LifePoints -= engine.OverlapAnExplodingBomb(this);
        }
    }
}
