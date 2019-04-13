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
        /// Constructor from json.
        /// </summary>
        /// <param name="datas">Json datas.</param>
        /// <returns><see cref="Rift"/></returns>
        internal static Rift FromDynamic(dynamic datas)
        {
            return new Rift((double)datas.X, (double)datas.Y, (double)datas.Width, (double)datas.Height, (double)datas.LifePoints);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="lifepoints"><see cref="DamageableSprite.CurrentLifePoints"/></param>
        internal Rift(double x, double y, double width, double height, double lifepoints)
            : base(x, y, width, height, lifepoints)
        {
            ExplosionLifePointCost = Constants.RIFT_EXPLOSION_LIFE_POINT_COST;
            ArrowLifePointCost = 0;
            CurrentLifePoints = lifepoints;
            _render = new PlainRender(Tools.HexFromColor(System.Windows.Media.Colors.Thistle));
        }

        /// <inheritdoc />
        internal override void BehaviorAtNewFrame()
        {
            CurrentLifePoints -= Engine.Default.CurrentScreen.HitByAnActionnedItem(this);
        }
    }
}
