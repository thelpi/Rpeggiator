﻿using RpeggiatorLib.Render;

namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents a rift (destroyable structure).
    /// </summary>
    /// <seealso cref="DamageableSprite"/>
    public class Rift : DamageableSprite
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="riftJson">The json dynamic object.</param>
        internal Rift(dynamic riftJson) : base((object)riftJson)
        {
            ExplosionLifePointCost = Constants.RIFT_EXPLOSION_LIFE_POINT_COST;
            ArrowLifePointCost = 0;
            CurrentLifePoints = riftJson.LifePoints;
            _render = new PlainRender(Tools.HexFromColor(System.Windows.Media.Colors.Thistle));
        }

        /// <inheritdoc />
        internal override void BehaviorAtNewFrame()
        {
            CurrentLifePoints -= Engine.Default.CurrentScreen.HitByAnActionnedItem(this);
        }
    }
}
