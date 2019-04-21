using System.Linq;
using RpeggiatorLib.Renders;

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
        private readonly int _connectedScreenId;
        // Render when locked.
        private readonly Render _renderLocked;

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
        public override Render Render { get { return Locked ? _renderLocked : _render; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"><see cref="Sprite.Id"/></param>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="keyId"><see cref="_keyId"/></param>
        /// <param name="connectedScreenId"><see cref="_connectedScreenId"/></param>
        /// <param name="playerGoThroughX"><see cref="PlayerGoThroughX"/></param>
        /// <param name="playerGoThroughY"><see cref="PlayerGoThroughY"/></param>
        /// <param name="lockedRenderType"><see cref="Enums.RenderType"/></param>
        /// <param name="lockedRenderProperties">Datas required to initialize <see cref="_renderLocked"/>.</param>
        /// <param name="renderType"><see cref="Enums.RenderType"/></param>
        /// <param name="renderProperties">Datas required to initialize the <see cref="Renders.Render"/>.</param>
        internal Door(int id, double x, double y, double width, double height, int? keyId, int connectedScreenId,
            double playerGoThroughX, double playerGoThroughY, Enums.RenderType lockedRenderType, string[] lockedRenderProperties,
            Enums.RenderType renderType, string[] renderProperties)
            : base(id, x, y, width, height, renderType, renderProperties)
        {
            _keyId = keyId;
            _connectedScreenId = connectedScreenId;
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
                return _connectedScreenId;
            }

            return null;
        }
    }
}
