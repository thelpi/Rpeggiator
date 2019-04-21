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
        /// <param name="id"><see cref="Sprite.Id"/></param>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="renderType"><see cref="Enums.RenderType"/></param>
        /// <param name="renderProperties">Datas required to initialize the <see cref="Renders.Render"/>.</param>
        protected ActionnedItem(int id, double x, double y, double width, double height,
            Enums.RenderType renderType, params object[] renderProperties)
            : base(id, x, y, width, height, renderType, renderProperties)
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
