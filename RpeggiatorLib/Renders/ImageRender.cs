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
        // Image name without extension (and index for animations).
        private readonly string _imageName;
        // Dictionary of ImageBrush, one by direction.
        private readonly Dictionary<KeyValuePair<int, Enums.Direction>, ImageBrush> _brushByStatus;
        // The PropertyInfo which contains the current direction.
        // If Null, the direction will always be right.
        private readonly PropertyInfo _directionProperty;
        // The sprite which owns this render.
        private readonly Sprites.Sprite _owner;
        // Indicates if the image is used as mosaic in the ImageBrush.
        private readonly bool _mosaicDisplay;
        // Time, in milliseconds, between each step of the animation.
        private readonly double _elapserNextStep;
        // Animation elapser
        private readonly Elapser _animationElapser;

        /// <summary>
        /// Creates a basic instance.
        /// </summary>
        /// <param name="imageName"><see cref="_imageName"/></param>
        /// <returns><see cref="ImageRender"/></returns>
        internal static ImageRender Basic(string imageName)
        {
            return new ImageRender(imageName, null, null, false, null, 0);
        }

        /// <summary>
        /// Creates an instance with direction.
        /// </summary>
        /// <param name="imageName"><see cref="_imageName"/></param>
        /// <param name="owner"><see cref="_owner"/></param>
        /// <param name="directionProperty"><see cref="_directionProperty"/></param>
        /// <returns><see cref="ImageRender"/></returns>
        internal static ImageRender WithDirection(string imageName, Sprites.Sprite owner, PropertyInfo directionProperty)
        {
            return new ImageRender(imageName, owner, directionProperty, false, null, 0);
        }

        /// <summary>
        /// Creates an instance with mosaic display.
        /// </summary>
        /// <param name="imageName"><see cref="_imageName"/></param>
        /// <param name="owner"><see cref="_owner"/></param>
        /// <returns><see cref="ImageRender"/></returns>
        internal static ImageRender WithMosaic(string imageName, Sprites.Sprite owner)
        {
            return new ImageRender(imageName, owner, null, true, null, 0);
        }

        /// <summary>
        /// Creates an instance with mosaic display and direction.
        /// </summary>
        /// <param name="imageName"><see cref="_imageName"/></param>
        /// <param name="owner"><see cref="_owner"/></param>
        /// <param name="directionProperty"><see cref="_directionProperty"/></param>
        /// <returns><see cref="ImageRender"/></returns>
        internal static ImageRender WithDirectionAndMosaic(string imageName, Sprites.Sprite owner, PropertyInfo directionProperty)
        {
            return new ImageRender(imageName, owner, directionProperty, true, null, 0);
        }

        /// <summary>
        /// Creates an animated instance.
        /// </summary>
        /// <param name="imageName"><see cref="_imageName"/></param>
        /// <param name="animationElapser"><see cref="_animationElapser"/></param>
        /// <param name="elapserNextStep"><see cref="_elapserNextStep"/></param>
        /// <returns><see cref="ImageRender"/></returns>
        internal static ImageRender AnimatedBasic(string imageName, Elapser animationElapser, double elapserNextStep)
        {
            return new ImageRender(imageName, null, null, false, animationElapser, elapserNextStep);
        }

        /// <summary>
        /// Creates an animated instance with mosaic display.
        /// </summary>
        /// <param name="imageName"><see cref="_imageName"/></param>
        /// <param name="animationElapser"><see cref="_animationElapser"/></param>
        /// <param name="elapserNextStep"><see cref="_elapserNextStep"/></param>
        /// <returns><see cref="ImageRender"/></returns>
        internal static ImageRender AnimatedWithMosaic(string imageName, Elapser animationElapser, double elapserNextStep)
        {
            return new ImageRender(imageName, null, null, true, animationElapser, elapserNextStep);
        }

        /// <summary>
        /// Creates an animated instance with direction.
        /// </summary>
        /// <param name="imageName"><see cref="_imageName"/></param>
        /// <param name="animationElapser"><see cref="_animationElapser"/></param>
        /// <param name="elapserNextStep"><see cref="_elapserNextStep"/></param>
        /// <param name="owner"><see cref="_owner"/></param>
        /// <param name="directionProperty"><see cref="_directionProperty"/></param>
        /// <returns><see cref="ImageRender"/></returns>
        internal static ImageRender AnimatedWithDirection(string imageName, Elapser animationElapser, double elapserNextStep,
            Sprites.Sprite owner, PropertyInfo directionProperty)
        {
            return new ImageRender(imageName, owner, directionProperty, false, animationElapser, elapserNextStep);
        }

        /// <summary>
        /// Creates an animated instance with mosaic display and direction.
        /// </summary>
        /// <param name="imageName"><see cref="_imageName"/></param>
        /// <param name="animationElapser"><see cref="_animationElapser"/></param>
        /// <param name="elapserNextStep"><see cref="_elapserNextStep"/></param>
        /// <param name="owner"><see cref="_owner"/></param>
        /// <param name="directionProperty"><see cref="_directionProperty"/></param>
        /// <returns><see cref="ImageRender"/></returns>
        internal static ImageRender AnimatedWithDirectionAndMosaic(string imageName, Elapser animationElapser, double elapserNextStep,
            Sprites.Sprite owner, PropertyInfo directionProperty)
        {
            return new ImageRender(imageName, owner, directionProperty, true, animationElapser, elapserNextStep);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="imageName"><see cref="_imageName"/></param>
        /// <param name="owner"><see cref="_owner"/></param>
        /// <param name="directionProperty"><see cref="_directionProperty"/></param>
        /// <param name="mosaicDisplay"><see cref="_mosaicDisplay"/></param>
        /// <param name="animationElapser"><see cref="_animationElapser"/></param>
        /// <param name="elapserNextStep"><see cref="_elapserNextStep"/></param>
        private ImageRender(string imageName, Sprites.Sprite owner, PropertyInfo directionProperty, bool mosaicDisplay,
            Elapser animationElapser, double elapserNextStep)
        {
            _owner = owner;
            _directionProperty = directionProperty;
            _imageName = imageName;
            _mosaicDisplay = mosaicDisplay;
            _brushByStatus = new Dictionary<KeyValuePair<int, Enums.Direction>, ImageBrush>();
            _elapserNextStep = elapserNextStep;
            _animationElapser = animationElapser;
        }
 
        /// <inheritdoc />
        public override Brush GetRenderBrush()
        {
            int currentIndex = _animationElapser?.GetStepIndex(_elapserNextStep) ?? -1;
            Enums.Direction currentDirection =
                _directionProperty != null ? (Enums.Direction)_directionProperty.GetValue(_owner) : Enums.Direction.Right;
            KeyValuePair<int, Enums.Direction> statusKey = new KeyValuePair<int, Enums.Direction>(currentIndex, currentDirection);

            if (!_brushByStatus.ContainsKey(statusKey))
            {
                BitmapImage bitmapImage = new BitmapImage();

                string actualImageName = currentIndex >= 0 ? string.Concat(_imageName, currentIndex) : _imageName;
                string resourcePath = Tools.GetImagePath(Engine.ResourcesPath, actualImageName);

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

                _brushByStatus.Add(statusKey, brush);
            }

            return _brushByStatus[statusKey];
        }
    }
}
