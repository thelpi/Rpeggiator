using System.Windows.Media;

namespace RpeggiatorLib.Graphic
{
    /// <summary>
    /// Represents a plain color as a way to render a <see cref="Sprites.Sprite"/>.
    /// </summary>
    /// <seealso cref="ISpriteGraphic"/>
    public class PlainBrushGraphic : ISpriteGraphic
    {
        /// <summary>
        /// Hexadecimal color code.
        /// </summary>
        public string HexadecimalColor { get; private set; }

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
            HexadecimalColor = hexadecimalColor;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="c"><see cref="Color"/></param>
        public PlainBrushGraphic(Color c)
        {
            HexadecimalColor = Tools.HexFromColor(c);
        }
    }
}
