using System.Windows.Media;

namespace RpeggiatorLib.Renders
{
    /// <summary>
    /// Represents a <see cref="Sprites.Sprite"/> render using a bitmap image.
    /// </summary>
    /// <seealso cref="Render"/>
    public class ImageRender : Render
    {
        // Instance specific to coins in the menu.
        private static ImageRender _coinMenuRender = null;
        // Instance specific to keyring in the menu.
        private static ImageRender _keyringMenuRender = null;

        /// <summary>
        /// Image name without extension in resources folder.
        /// </summary>
        protected readonly string _imageName;
        // Image brush.
        private ImageBrush _brush = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="imageName"><see cref="_imageName"/></param>
        internal ImageRender(string imageName)
        {
            _imageName = imageName;
        }

        /// <inheritdoc />
        public override Brush GetRenderBrush()
        {
            if (_brush == null)
            {
                _brush = ComputeImageBrush(_imageName);
            }

            return _brush;
        }

        /// <summary>
        /// <see cref="ImageRender"/> specific to coins in the menu.
        /// </summary>
        /// <returns><see cref="ImageRender"/></returns>
        internal static ImageRender CoinMenuRender()
        {
            if (_coinMenuRender == null)
            {
                _coinMenuRender = new ImageRender(nameof(Enums.Filename.Coin));
            }

            return _coinMenuRender;
        }

        /// <summary>
        /// <see cref="ImageRender"/> specific to keyring in the menu.
        /// </summary>
        /// <returns><see cref="ImageRender"/></returns>
        internal static ImageRender KeyringMenuRender()
        {
            if (_keyringMenuRender == null)
            {
                _keyringMenuRender = new ImageRender(nameof(Enums.Filename.Keyring));
            }

            return _keyringMenuRender;
        }
    }
}
