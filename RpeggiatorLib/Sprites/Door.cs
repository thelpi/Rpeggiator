using System.Linq;
using RpeggiatorLib.Render;

namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents a door.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class Door : Sprite
    {
        // The optionnal key identifier to unlock the door.
        private int? _keyId;
        // The screen identifier this door is connected.
        private int _screenId;
        // Render when locked.
        private ISpriteRender _renderLocked;

        /// <summary>
        /// Identifier.
        /// </summary>
        /// <remarks>A door has the same identifier in both screens.</remarks>
        public int Id { get; private set; }
        /// <summary>
        /// The <see cref="Player"/> X-axis position when he goes through the door and changes screen.
        /// </summary>
        public double PlayerGoThroughX { get; private set; }
        /// <summary>
        /// The <see cref="Player"/> Y-axis position when he goes through the door and changes screen.
        /// </summary>
        public double PlayerGoThroughY { get; private set; }
        /// <summary>
        /// Inferred; indicates if the door is locked (the <see cref="Player"/> doesn't have the key).
        /// </summary>
        public bool Locked { get { return !TryOpen().HasValue; } }
        /// <summary>
        /// The render, which changes regarding if the door is locked or not.
        /// </summary>
        public override ISpriteRender Render { get { return Locked ? _renderLocked : _render; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="keyId"><see cref="_keyId"/></param>
        /// <param name="screenId"><see cref="_screenId"/></param>
        /// <param name="id"><see cref="Id"/></param>
        /// <param name="playerGoThroughX"><see cref="PlayerGoThroughX"/></param>
        /// <param name="playerGoThroughY"><see cref="PlayerGoThroughY"/></param>
        /// <param name="lockedRenderType"><see cref="_renderLocked"/> subtype name.</param>
        /// <param name="lockedRenderProperties">Datas required to initialize <see cref="_renderLocked"/>.</param>
        /// <param name="renderType"><see cref="ISpriteRender"/> subtype name.</param>
        /// <param name="renderProperties">Datas required to initialize the <see cref="ISpriteRender"/>.</param>
        internal Door(double x, double y, double width, double height, int? keyId, int screenId, int id,
            double playerGoThroughX, double playerGoThroughY, string lockedRenderType, object[] lockedRenderProperties,
            string renderType, object[] renderProperties)
            : base(x, y, width, height, renderType, renderProperties)
        {
            _keyId = keyId;
            _screenId = screenId;
            Id = id;
            PlayerGoThroughX = playerGoThroughX;
            PlayerGoThroughY = playerGoThroughY;
            _renderLocked = GetRenderFromValues(lockedRenderType, lockedRenderProperties);
        }

        /// <summary>
        /// Tries to open the door.
        /// </summary>
        /// <returns>The <see cref="Screen"/> identifier; <c>Null</c> if failure.</returns>
        internal int? TryOpen()
        {
            if (!_keyId.HasValue || Engine.Default.Player.Inventory.Keyring.Any(k => k == _keyId))
            {
                return _screenId;
            }

            return null;
        }
    }
}
