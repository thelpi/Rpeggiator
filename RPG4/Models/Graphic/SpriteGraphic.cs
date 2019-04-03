using System.Windows.Media;

namespace RPG4.Models.Graphic
{
    /// <summary>
    /// Interface for <see cref="Sprites.Sprite"/> graphic rendering.
    /// </summary>
    public interface ISpriteGraphic
    {
        /// <summary>
        /// Gets the rendering.
        /// </summary>
        /// <remarks>This method should be call only by the thread in charge of rendering.</remarks>
        /// <returns>The rendering <see cref="Brush"/>.</returns>
        Brush GetRenderingBrush();
    }
}
