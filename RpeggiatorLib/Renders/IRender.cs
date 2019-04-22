using System.Windows.Media;

namespace RpeggiatorLib.Renders
{
    /// <summary>
    /// Interface for <see cref="Sprites.Sprite"/> graphic render.
    /// </summary>
    public interface IRender
    {
        /// <summary>
        /// Gets the render of a <see cref="Sprites.Sprite"/>.
        /// </summary>
        /// <returns><see cref="Brush"/></returns>
        Brush GetRenderBrush();
    }
}
