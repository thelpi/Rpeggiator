using System.Linq;

namespace RPG4.Models.Sprites
{
    /// <summary>
    /// Represents a door.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class Door : Sprite
    {
        private int? _keyId;
        private int _screenId;

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
        /// Constructor.
        /// </summary>
        /// <param name="doorJsonDatas">Door json datas.</param>
        public Door(dynamic doorJsonDatas) : base((object)doorJsonDatas)
        {
            _keyId = doorJsonDatas.KeyId;
            _screenId = doorJsonDatas.ScreenId;
            Id = doorJsonDatas.Id;
            PlayerGoThroughX = doorJsonDatas.PlayerGoThroughX;
            PlayerGoThroughY = doorJsonDatas.PlayerGoThroughY;
        }

        /// <summary>
        /// Tries to open the door.
        /// </summary>
        /// <returns>The <see cref="Screen"/> identifier; <c>Null</c> if failure.</returns>
        public int? TryOpen()
        {
            if (!_keyId.HasValue || Engine.Default.Player.Inventory.Keyring.Any(k => k == _keyId))
            {
                return _screenId;
            }

            return null;
        }
    }
}
