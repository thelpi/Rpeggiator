﻿namespace RPG4.Models.Sprites
{
    /// <summary>
    /// Represents a structure which can be destroyed by a <see cref="ActionnedBomb"/>.
    /// </summary>
    /// <seealso cref="Sprite"/>
    /// <seealso cref="IExplodable"/>
    public class Rift : Sprite, IExplodable
    {
        /// <summary>
        /// Life points count.
        /// </summary>
        public double LifePoints { get; private set; }
        /// <inheritdoc />
        public double ExplosionLifePointCost { get { return Constants.RIFT_EXPLOSION_LIFE_POINT_COST; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="riftJson">The json dynamic object.</param>
        public Rift(dynamic riftJson) : base((object)riftJson)
        {
            LifePoints = riftJson.LifePoints;
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame()
        {
            LifePoints -= Engine.Default.CurrentScreen.OverlapAnExplodingBomb(this);
        }
    }
}