using System.Windows.Media;

namespace RpeggiatorLib.Render
{
    /// <summary>
    /// Interface for <see cref="Sprites.Sprite"/> graphic render.
    /// </summary>
    public interface ISpriteRender
    {
        /// <summary>
        /// Gets the render of a <see cref="Sprites.Sprite"/>.
        /// </summary>
        /// <returns><see cref="Brush"/></returns>
        Brush GetRenderBrush();
    }
}
