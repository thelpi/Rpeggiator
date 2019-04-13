namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents a permanent structure.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class PermanentStructure : Sprite
    {
        /// <summary>
        /// Creates an instance from json datas.
        /// </summary>
        /// <param name="datas">Json datas.</param>
        /// <returns><see cref="PermanentStructure"/></returns>
        internal static PermanentStructure FromDynamic(dynamic datas)
        {
            PermanentStructure ps = new PermanentStructure((double)datas.X, (double)datas.Y, (double)datas.Width, (double)datas.Height);
            ps.SetRenderFromDynamic((object)datas);
            return ps;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        internal PermanentStructure(double x, double y, double width, double height)
            : base(x, y, width, height)
        {

        }
    }
}
