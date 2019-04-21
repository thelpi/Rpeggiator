using System.Collections.Generic;
using System.Windows.Media;

namespace RpeggiatorLib.Renders
{
    /// <summary>
    /// Represents a <see cref="Sprites.Sprite"/> render using a bitmap image for each direction.
    /// </summary>
    /// <seealso cref="ImageRender"/>
    public class ImageDirectionRender : ImageRender
    {
        // Dictionary of brushes, one by direction. 
        private Dictionary<Enums.Direction, ImageBrush> _directionBrushes = new Dictionary<Enums.Direction, ImageBrush>();
        // The property name which contains the current direction.
        private readonly string _directionPropertyName;
        // The sprite which owns this render.
        private readonly Sprites.Sprite _forInstance;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="imageName"><see cref="ImageRender._imageName"/></param>
        /// <param name="forInstance"><see cref="_forInstance"/></param>
        /// <param name="directionPropertyName"><see cref="_directionPropertyName"/></param>
        internal ImageDirectionRender(string imageName, Sprites.Sprite forInstance, string directionPropertyName) : base(imageName)
        {
            _forInstance = forInstance;
            _directionPropertyName = directionPropertyName;
        }
 
        /// <inheritdoc />
        public override Brush GetRenderBrush()
        {
            Enums.Direction currentDirection = (Enums.Direction)_forInstance
                                                                    .GetType()
                                                                    .GetProperty(_directionPropertyName)
                                                                    .GetValue(_forInstance);

            if (!_directionBrushes.ContainsKey(currentDirection))
            {
                _directionBrushes.Add(currentDirection, ComputeImageBrush(_imageName, currentDirection));
            }

            return _directionBrushes[currentDirection];
        }
    }
}
