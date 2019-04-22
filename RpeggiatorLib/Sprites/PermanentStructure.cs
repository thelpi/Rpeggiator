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
        /// <param name="id"><see cref="Sprite.Id"/></param>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="renderType"><see cref="Enums.RenderType"/></param>
        /// <param name="renderProperties">Datas required to initialize the <see cref="Renders.IRender"/>.</param>
        internal PermanentStructure(int id, double x, double y, double width, double height,
            Enums.RenderType renderType, string[] renderProperties)
            : base(id, x, y, width, height, renderType, renderProperties)
        {
            // Empty.
        }
    }
}
