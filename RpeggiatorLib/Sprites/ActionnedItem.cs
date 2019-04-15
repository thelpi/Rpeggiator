namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents an item dropped on the floor.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public abstract class ActionnedItem : Sprite
    {
        /// <summary>
        /// Overriden; Indicates the item use is finished.
        /// </summary>
        public abstract bool IsDone { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="renderType"><see cref="Render.ISpriteRender"/> subtype name.</param>
        /// <param name="renderProperties">Datas required to initialize the <see cref="Render.ISpriteRender"/>.</param>
        protected ActionnedItem(double x, double y, double width, double height, string renderType, params object[] renderProperties)
            : base(x, y, width, height, renderType, renderProperties)
        {
            // Empty.
        }

        /// <summary>
        /// Overriden; Gets the lifepoints cost for the specified <paramref name="sprite"/>, if this instance hits it.
        /// </summary>
        /// <param name="sprite"><see cref="DamageableSprite"/></param>
        /// <returns>Life points cost.</returns>
        internal abstract double GetLifePointsCost(DamageableSprite sprite);
    }
}
