using System.Windows.Media;

namespace RPG4.Models.Graphic
{
    /// <summary>
    /// Represents a plain color as a way to render a <see cref="Sprites.Sprite"/>.
    /// </summary>
    /// <seealso cref="ISpriteGraphic"/>
    public class PlainBrushGraphic : ISpriteGraphic
    {
        // Hexadecimal color code.
        private string _hexadecimalColor;
        // Brush.
        private SolidColorBrush _brush = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="hexadecimalColor">The hexadecimal value of the color.</param>
        public PlainBrushGraphic(string hexadecimalColor)
        {
            if (!hexadecimalColor.StartsWith("#"))
            {
                hexadecimalColor = string.Concat("#", hexadecimalColor);
            }
            _hexadecimalColor = hexadecimalColor;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="c"><see cref="Color"/></param>
        public PlainBrushGraphic(Color c)
        {
            _hexadecimalColor = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", c.A, c.R, c.G, c.B);
        }

        /// <inheritdoc />
        public Brush GetRenderingBrush()
        {
            if (_brush == null)
            {
                _brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_hexadecimalColor));
            }
            return _brush;
        }
    }
}
