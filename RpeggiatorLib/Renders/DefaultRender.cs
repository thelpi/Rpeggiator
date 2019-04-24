using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RpeggiatorLib.Renders
{
    /// <summary>
    /// Represents the default behavior to render a <see cref="Sprites.Sprite"/>.
    /// </summary>
    /// <seealso cref="IRender"/>
    public class DefaultRender : IRender
    {
        // Instance specific to coins in the menu.
        private static IRender _coinMenuRender = null;
        // Instance specific to keyring in the menu.
        private static IRender _keyringMenuRender = null;

        // Image name without extension (and without index for animations).
        private readonly string _imageName;
        // Dictionary of Brushes, one by direction and animation index.
        // For plain color instance, just get the first of the list.
        private readonly Dictionary<KeyValuePair<int, Enums.Direction>, Brush> _brushByStatus;
        // The sprite which owns this render.
        private readonly Sprites.Sprite _owner;
        // Indicates if the image is used as mosaic in the ImageBrush.
        private readonly bool _mosaicDisplay;
        // Time, in milliseconds, between each step of the animation.
        private readonly double _elapserNextStep;
        // Animation elapser use enum.
        private readonly Enums.ElapserUse? _animationElapser;
        // Color hexadecimal value.
        private readonly string _hexColor;
        // Biggest contiguous index as a suffix from _imageName (start to zero, the only mandatory index)
        private readonly int _topIndex;

        /// <summary>
        /// Creates a plain color instance.
        /// </summary>
        /// <param name="owner"><see cref="_owner"/></param>
        /// <param name="hexColor"><see cref="_hexColor"/></param>
        /// <returns><see cref="DefaultRender"/></returns>
        internal static DefaultRender PlainColor(Sprites.Sprite owner, string hexColor)
        {
            return new DefaultRender(owner, hexColor, null, false, null, 0);
        }

        /// <summary>
        /// Creates a basic image instance.
        /// </summary>
        /// <param name="owner"><see cref="_owner"/></param>
        /// <param name="imageName"><see cref="_imageName"/></param>
        /// <returns><see cref="DefaultRender"/></returns>
        internal static DefaultRender BasicImage(Sprites.Sprite owner, string imageName)
        {
            return new DefaultRender(owner, null, imageName, false, null, 0);
        }

        /// <summary>
        /// Creates an image instance with mosaic display.
        /// </summary>
        /// <param name="owner"><see cref="_owner"/></param>
        /// <param name="imageName"><see cref="_imageName"/></param>
        /// <returns><see cref="DefaultRender"/></returns>
        internal static DefaultRender ImageWithMosaic(Sprites.Sprite owner, string imageName)
        {
            return new DefaultRender(owner, null, imageName, true, null, 0);
        }

        /// <summary>
        /// Creates an animated image instance.
        /// </summary>
        /// <param name="owner"><see cref="_owner"/></param>
        /// <param name="imageName"><see cref="_imageName"/></param>
        /// <param name="animationElapser"><see cref="_animationElapser"/></param>
        /// <param name="elapserNextStep"><see cref="_elapserNextStep"/></param>
        /// <returns><see cref="DefaultRender"/></returns>
        internal static DefaultRender AnimatedBasicImage(Sprites.Sprite owner, string imageName,
            Enums.ElapserUse animationElapser, double elapserNextStep)
        {
            return new DefaultRender(owner, null, imageName, false, animationElapser, elapserNextStep);
        }

        /// <summary>
        /// Creates an animated image instance with mosaic display.
        /// </summary>
        /// <param name="owner"><see cref="_owner"/></param>
        /// <param name="imageName"><see cref="_imageName"/></param>
        /// <param name="animationElapser"><see cref="_animationElapser"/></param>
        /// <param name="elapserNextStep"><see cref="_elapserNextStep"/></param>
        /// <returns><see cref="DefaultRender"/></returns>
        internal static DefaultRender AnimatedImageWithMosaic(Sprites.Sprite owner, string imageName,
            Enums.ElapserUse animationElapser, double elapserNextStep)
        {
            return new DefaultRender(owner, null, imageName, true, animationElapser, elapserNextStep);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="owner"><see cref="_owner"/></param>
        /// <param name="hexColor"><see cref="_hexColor"/></param>
        /// <param name="imageName"><see cref="_imageName"/></param>
        /// <param name="mosaicDisplay"><see cref="_mosaicDisplay"/></param>
        /// <param name="animationElapser"><see cref="_animationElapser"/></param>
        /// <param name="elapserNextStep"><see cref="_elapserNextStep"/></param>
        private DefaultRender(Sprites.Sprite owner, string hexColor, string imageName, bool mosaicDisplay,
            Enums.ElapserUse? animationElapser, double elapserNextStep)
        {
            _owner = owner;
            _imageName = imageName;
            _mosaicDisplay = mosaicDisplay;
            _brushByStatus = new Dictionary<KeyValuePair<int, Enums.Direction>, Brush>();
            _elapserNextStep = elapserNextStep;
            _animationElapser = animationElapser;
            _hexColor = hexColor;
            if (animationElapser.HasValue)
            {
                _topIndex = 0;
                while (File.Exists(Tools.GetImagePath(Engine.ResourcesPath, string.Concat(_imageName, _topIndex))))
                {
                    _topIndex++;
                }
                _topIndex--;
            }
        }
 
        /// <inheritdoc />
        public Brush GetRenderBrush()
        {
            if (!string.IsNullOrWhiteSpace(_hexColor))
            {
                if (_brushByStatus.Keys.Count == 0)
                {
                    _brushByStatus.Add(new KeyValuePair<int, Enums.Direction>(), (SolidColorBrush)(new BrushConverter().ConvertFrom(_hexColor)));
                }

                return _brushByStatus.Values.First();
            }

            int currentIndex = -1;
            if (_animationElapser.HasValue)
            {
                Elapser elapser = Elapser.Instances.First(x => x.Owner == _owner && x.UseId == _animationElapser);
                if (elapser != null)
                {
                    currentIndex = elapser.GetStepIndex(_elapserNextStep, _topIndex);
                }
            }

            Enums.Direction currentDirection = _owner?.GetDirection() ?? Constants.DEFAULT_SPRITE_DIRECTION;
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

        /// <summary>
        /// <see cref="IRender"/> specific to coins in the menu.
        /// </summary>
        /// <returns><see cref="IRender"/></returns>
        internal static IRender CoinMenuRender()
        {
            if (_coinMenuRender == null)
            {
                _coinMenuRender = BasicImage(null, nameof(Enums.Filename.Coin));
            }

            return _coinMenuRender;
        }

        /// <summary>
        /// <see cref="IRender"/> specific to keyring in the menu.
        /// </summary>
        /// <returns><see cref="IRender"/></returns>
        internal static IRender KeyringMenuRender()
        {
            if (_keyringMenuRender == null)
            {
                _keyringMenuRender = BasicImage(null, nameof(Enums.Filename.Keyring));
            }

            return _keyringMenuRender;
        }
    }
}
