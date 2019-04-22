using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
                BitmapImage bitmapImage = new BitmapImage();

                string resourcePath = Tools.GetImagePath(Engine.ResourcesPath, _imageName);

                using (FileStream stream = new FileStream(resourcePath, FileMode.Open, FileAccess.Read))
                {
                    stream.Position = 0;
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = stream;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                }

                double angle = 0;
                bool flip = false;
                switch (currentDirection)
                {
                    case Enums.Direction.Bottom:
                        angle = 90;
                        break;
                    case Enums.Direction.BottomLeft:
                        angle = 45;
                        flip = true;
                        break;
                    case Enums.Direction.BottomRight:
                        angle = 45;
                        break;
                    case Enums.Direction.Left:
                        flip = true;
                        break;
                    case Enums.Direction.Right:
                        break;
                    case Enums.Direction.Top:
                        angle = -90;
                        break;
                    case Enums.Direction.TopLeft:
                        angle = -45;
                        flip = true;
                        break;
                    case Enums.Direction.TopRight:
                        angle = -45;
                        break;
                }

                ImageBrush brush = new ImageBrush
                {
                    ImageSource = bitmapImage,
                    Stretch = Stretch.UniformToFill,
                    RelativeTransform = new TransformGroup
                    {
                        Children = new TransformCollection
                    {
                        new RotateTransform
                        {
                            CenterX = 0.5,
                            CenterY = 0.5,
                            Angle = angle
                        }, new ScaleTransform
                        {
                            CenterX = 0.5,
                            CenterY = 0.5,
                            ScaleX = flip ? -1 : 1
                        }
                    }
                    }
                };

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
