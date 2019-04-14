using RpeggiatorLib.Render;

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
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="lifepoints"><see cref="DamageableSprite.CurrentLifePoints"/></param>
        /// <param name="renderType"><see cref="ISpriteRender"/> subtype name.</param>
        /// <param name="renderProperties">Datas required to initialize the <see cref="ISpriteRender"/>.</param>
        internal Rift(double x, double y, double width, double height,
            double lifepoints, string renderType, object[] renderProperties)
            : base(x, y, width, height, lifepoints, renderType, renderProperties)
        {
            ExplosionLifePointCost = Constants.RIFT_EXPLOSION_LIFE_POINT_COST;
            ArrowLifePointCost = 0;
            CurrentLifePoints = lifepoints;
        }

        /// <inheritdoc />
        internal override void BehaviorAtNewFrame()
        {
            CurrentLifePoints -= Engine.Default.CurrentScreen.HitByAnActionnedItem(this);
        }
    }
}
