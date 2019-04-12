using RpeggiatorLib.Render;

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
        /// <param name="datas">Dynamic json datas.</param>
        internal PermanentStructure(dynamic datas)
            : base((double)datas.X, (double)datas.Y, (double)datas.Width, (double)datas.Height)
        {
            _render = new PlainRender(Tools.HexFromColor(System.Windows.Media.Colors.SlateGray));
        }
    }
}
