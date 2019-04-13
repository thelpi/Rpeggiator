using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RpeggiatorLib.Render
{
    /// <summary>
    /// Represents a <see cref="Sprites.Sprite"/> render using a bitmap image.
    /// </summary>
    /// <seealso cref="ISpriteRender"/>
    public class ImageRender : ISpriteRender
    {
        // Instance specific to coins in the menu.
        private static ImageRender _coinMenuRender = null;
        // Instance specific to keyring in the menu.
        private static ImageRender _keyringMenuRender = null;

        // Image name.
        private string _imageName;
        // Image brush.
        private ImageBrush _brush = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="imageName">Image name without extension in resources folder.</param>
        internal ImageRender(string imageName)
        {
            _imageName = imageName;
        }

        /// <inheritdoc />
        public virtual Brush GetRenderBrush()
        {
            if (_brush == null)
            {
                _brush = ComputeBrush();
            }

            return _brush;
        }

        /// <summary>
        /// Computes the <see cref="ImageBrush"/>, optionally depending on a <see cref="Enums.Direction"/>.
        /// </summary>
        /// <param name="direction">Optionnal <see cref="Enums.Direction"/>.</param>
        /// <returns><see cref="ImageBrush"/></returns>
        protected ImageBrush ComputeBrush(Enums.Direction direction = Enums.Direction.Right)
        {
            BitmapImage bitmapImage = new BitmapImage();

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format(Constants.IMAGE_RESSOURCE_FILE_FORMAT, _imageName)))
            {
                stream.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }

            double angle = 0;
            bool flip = false;
            switch (direction)
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

            return new ImageBrush
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
        }

        /// <summary>
        /// <see cref="ImageRender"/> specific to coins in the menu.
        /// </summary>
        /// <returns><see cref="ImageRender"/></returns>
        public static ImageRender CoinMenuRender()
        {
            if (_coinMenuRender == null)
            {
                _coinMenuRender = new ImageRender("Coin");
            }

            return _coinMenuRender;
        }

        /// <summary>
        /// <see cref="ImageRender"/> specific to keyring in the menu.
        /// </summary>
        /// <returns><see cref="ImageRender"/></returns>
        public static ImageRender KeyringMenuRender()
        {
            if (_keyringMenuRender == null)
            {
                _keyringMenuRender = new ImageRender("Keyring");
            }

            return _keyringMenuRender;
        }
    }
}
