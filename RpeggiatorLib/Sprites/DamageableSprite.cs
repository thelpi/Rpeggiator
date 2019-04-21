namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents a sprite which can be damaged.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public abstract class DamageableSprite : Sprite
    {
        /// <summary>
        /// Current number of life points.
        /// </summary>
        public double CurrentLifePoints { get; protected set; }
        /// <summary>
        /// <see cref="Enums.ItemType.Bomb"/> explosion life points cost.
        /// </summary>
        public double ExplosionLifePointCost { get; protected set; }
        /// <summary>
        /// <see cref="Enums.ItemType.Arrow"/> targeting life points cost.
        /// </summary>
        public double ArrowLifePointCost { get; protected set; }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"><see cref="Sprite.Id"/></param>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="lifepoints"><see cref="CurrentLifePoints"/></param>
        /// <param name="renderType"><see cref="Enums.RenderType"/></param>
        /// <param name="renderProperties">Datas required to initialize the <see cref="Renders.Render"/>.</param>
        protected DamageableSprite(int id, double x, double y, double width, double height,
            double lifepoints, Enums.RenderType renderType, string[] renderProperties) 
            : base(id, x, y, width, height, renderType, renderProperties)
        {
            CurrentLifePoints = lifepoints;
        }
    }
}
