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
        private int? _keyId;
        private int _screenId;
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
        /// <inheritdoc />
        public override ISpriteRender Render { get { return Locked ? _renderLocked : _render; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="doorJsonDatas">Door json datas.</param>
        internal Door(dynamic doorJsonDatas)
            : base((double)doorJsonDatas.X, (double)doorJsonDatas.Y, (double)doorJsonDatas.Width, (double)doorJsonDatas.Height)
        {
            _keyId = doorJsonDatas.KeyId;
            _screenId = doorJsonDatas.ScreenId;
            Id = doorJsonDatas.Id;
            PlayerGoThroughX = doorJsonDatas.PlayerGoThroughX;
            PlayerGoThroughY = doorJsonDatas.PlayerGoThroughY;
            _render = new ImageRender("Door");
            _renderLocked = new ImageRender("DoorLocked");
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
