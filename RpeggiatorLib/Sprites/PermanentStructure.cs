namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents a permanent structure.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class PermanentStructure : Sprite
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="renderType"><see cref="Render.ISpriteRender"/> subtype name.</param>
        /// <param name="renderProperties">Datas required to initialize the <see cref="Render.ISpriteRender"/>.</param>
        internal PermanentStructure(double x, double y, double width, double height, string renderType, object[] renderProperties)
            : base(x, y, width, height, renderType, renderProperties)
        {
            // Empty.
        }
    }
}
