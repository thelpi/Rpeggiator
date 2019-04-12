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
        public Brush GetRenderBrush()
        {
            if (_brush == null)
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

                _brush = new ImageBrush
                {
                    ImageSource = bitmapImage,
                    Stretch = Stretch.Fill
                };
            }

            return _brush;
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
