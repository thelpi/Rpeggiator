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
            if (datas.RenderType != null && datas.RenderValue != null)
            {
                switch ((string)datas.RenderType)
                {
                    case nameof(ImageMosaicRender):
                        _render = new ImageMosaicRender((string)datas.RenderValue, this);
                        break;
                    case nameof(ImageRender):
                        _render = new ImageRender((string)datas.RenderValue);
                        break;
                    case nameof(PlainRender):
                        _render = new PlainRender((string)datas.RenderValue);
                        break;
                    default:
                        throw new System.NotImplementedException(Messages.NotImplementedGraphicExceptionMessage);
                }
            }
            else
            {
                _render = new PlainRender(Tools.HexFromColor(System.Windows.Media.Colors.SlateGray));
            }
        }
    }
}
