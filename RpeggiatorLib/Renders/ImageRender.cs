using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;

namespace RpeggiatorLib.Renders
{
    /// <summary>
    /// Represents a <see cref="Sprites.Sprite"/> render using a bitmap image for each direction.
    /// </summary>
    /// <seealso cref="Render"/>
    public class ImageRender : Render
    {
        // Image name without extension.
        private readonly string _imageName;
        // Dictionary of ImageBrush, one by direction.
        private readonly Dictionary<Enums.Direction, ImageBrush> _directionBrushes = new Dictionary<Enums.Direction, ImageBrush>();
        // The PropertyInfo which contains the current direction.
        // If Null, the direction will always be right.
        private readonly PropertyInfo _directionProperty;
        // The sprite which owns this render.
        private readonly Sprites.Sprite _owner;
        // Indicates if the image is used as mosaic in the ImageBrush.
        private readonly bool _mosaicDisplay;

        /// <summary>
        /// Creates a basic instance.
        /// </summary>
        /// <param name="imageName"><see cref="_imageName"/></param>
        internal static ImageRender Basic(string imageName)
        {
            return new ImageRender(imageName, null, null, false);
        }

        /// <summary>
        /// Creates an instance with direction.
        /// </summary>
        /// <param name="imageName"><see cref="_imageName"/></param>
        /// <param name="owner"><see cref="_owner"/></param>
        /// <param name="directionProperty"><see cref="_directionProperty"/></param>
        internal static ImageRender WithDirection(string imageName, Sprites.Sprite owner, PropertyInfo directionProperty)
        {
            return new ImageRender(imageName, owner, directionProperty, false);
        }

        /// <summary>
        /// Creates an instance with mosaic.
        /// </summary>
        /// <param name="imageName"><see cref="_imageName"/></param>
        /// <param name="owner"><see cref="_owner"/></param>
        internal static ImageRender WithMosaic(string imageName, Sprites.Sprite owner)
        {
            return new ImageRender(imageName, owner, null, true);
        }

        /// <summary>
        /// Creates an instance with mosaic and direction.
        /// </summary>
        /// <param name="imageName"><see cref="_imageName"/></param>
        /// <param name="owner"><see cref="_owner"/></param>
        /// <param name="directionProperty"><see cref="_directionProperty"/></param>
        internal static ImageRender WithDirectionAndMosaic(string imageName, Sprites.Sprite owner, PropertyInfo directionProperty)
        {
            return new ImageRender(imageName, owner, directionProperty, true);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="imageName"><see cref="_imageName"/></param>
        /// <param name="owner"><see cref="_owner"/></param>
        /// <param name="directionProperty"><see cref="_directionProperty"/></param>
        /// <param name="mosaicDisplay"><see cref="_mosaicDisplay"/></param>
        private ImageRender(string imageName, Sprites.Sprite owner, PropertyInfo directionProperty, bool mosaicDisplay)
        {
            _owner = owner;
            _directionProperty = directionProperty;
            _imageName = imageName;
            _mosaicDisplay = mosaicDisplay;
        }
 
        /// <inheritdoc />
        public override Brush GetRenderBrush()
        {
            Enums.Direction currentDirection = Enums.Direction.Right;

            if (_directionProperty != null)
            {
                currentDirection = (Enums.Direction)_directionProperty.GetValue(_owner);
            }

            if (!_directionBrushes.ContainsKey(currentDirection))
            {
                ImageBrush brush = ComputeImageBrush(_imageName, currentDirection);

                if (_mosaicDisplay)
                {
                    brush.TileMode = TileMode.Tile;
                    brush.Stretch = Stretch.None;
                    brush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
                    brush.Viewport = new System.Windows.Rect(0, 0,
                        1 / (_owner.Width / brush.ImageSource.Width),
                        1 / (_owner.Height / brush.ImageSource.Height));
                }

                _directionBrushes.Add(currentDirection, brush);
            }

            return _directionBrushes[currentDirection];
        }
    }
}
