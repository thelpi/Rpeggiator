using System.Windows.Media;

namespace RpeggiatorLib.Render
{
    /// <summary>
    /// Represents a <see cref="Sprites.Sprite"/> render using a bitmap image used as mosaic.
    /// </summary>
    /// <seealso cref="ImageRender"/>
    public class ImageMosaicRender : ImageRender
    {
        // Image brush.
        private ImageBrush _brush;
        // The sprite which owns this render.
        private Sprites.Sprite _forInstance;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="imageName"><see cref="ImageRender._imageName"/></param>
        /// <param name="forInstance"></param>
        internal ImageMosaicRender(string imageName, Sprites.Sprite forInstance) : base(imageName)
        {
            _forInstance = forInstance;
        }

        /// <inheritdoc />
        public override Brush GetRenderBrush()
        {
            if (_brush == null)
            {
                _brush = ComputeBrush();
                _brush.TileMode = TileMode.Tile;
                _brush.Stretch = Stretch.None;
                _brush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
                _brush.Viewport = new System.Windows.Rect(0, 0,
                    1 / (_forInstance.Width / _brush.ImageSource.Width),
                    1 / (_forInstance.Height / _brush.ImageSource.Height));
            }

            return _brush;
        }
    }
}
