using System.Windows.Media;

namespace RpeggiatorLib.Render
{
    /// <summary>
    /// Represents a <see cref="Sprites.Sprite"/> render by plain color.
    /// </summary>
    /// <seealso cref="ISpriteRender"/>
    public class PlainRender : ISpriteRender
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
        public Brush GetRenderBrush()
        {
            if (_brush == null)
            {
                _brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(_hexColor));
            }

            return _brush;
        }
    }
}
