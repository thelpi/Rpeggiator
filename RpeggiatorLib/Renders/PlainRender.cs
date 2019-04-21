using System.Windows.Media;

namespace RpeggiatorLib.Renders
{
    /// <summary>
    /// Represents a <see cref="Sprites.Sprite"/> render by plain color.
    /// </summary>
    /// <seealso cref="Render"/>
    public class PlainRender : Render
    {
        // Color hexadecimal value.
        private string _hexColor;
        // Color brush.
        private SolidColorBrush _brush = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="hexColor">Color hexadecimal value.</param>
        internal PlainRender(string hexColor)
        {
            _hexColor = hexColor;
        }

        /// <inheritdoc />
        public override Brush GetRenderBrush()
        {
            if (_brush == null)
            {
                _brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(_hexColor));
            }

            return _brush;
        }
    }
}
