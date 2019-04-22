using System.Windows.Media;

namespace RpeggiatorLib.Renders
{
    /// <summary>
    /// Interface for <see cref="Sprites.Sprite"/> graphic render.
    /// </summary>
    public abstract class Render
    {
        // Instance specific to coins in the menu.
        private static Render _coinMenuRender = null;
        // Instance specific to keyring in the menu.
        private static Render _keyringMenuRender = null;

        /// <summary>
        /// Gets the render of a <see cref="Sprites.Sprite"/>.
        /// </summary>
        /// <returns><see cref="Brush"/></returns>
        public abstract Brush GetRenderBrush();

        /// <summary>
        /// <see cref="Render"/> specific to coins in the menu.
        /// </summary>
        /// <returns><see cref="Render"/></returns>
        internal static Render CoinMenuRender()
        {
            if (_coinMenuRender == null)
            {
                _coinMenuRender = ImageRender.Basic(nameof(Enums.Filename.Coin));
            }

            return _coinMenuRender;
        }

        /// <summary>
        /// <see cref="Render"/> specific to keyring in the menu.
        /// </summary>
        /// <returns><see cref="Render"/></returns>
        internal static Render KeyringMenuRender()
        {
            if (_keyringMenuRender == null)
            {
                _keyringMenuRender = ImageRender.Basic(nameof(Enums.Filename.Keyring));
            }

            return _keyringMenuRender;
        }
    }
}
