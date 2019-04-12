using System.Linq;
using RpeggiatorLib.Render;

namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents a chest.
    /// </summary>
    public class Chest : Sprite
    {
        private int? _keyId;
        private Enums.ItemType? _itemType;
        private int _quantity;
        private int? _keyIdContainer;
        private ISpriteRender _renderOpen;

        /// <summary>
        /// Indicates if the chest is open.
        /// </summary>
        public bool IsOpen { get; private set; }
        /// <inheritdoc />
        public override ISpriteRender Render { get { return IsOpen ? _renderOpen : _render; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="chestJsonDatas">Chest json datas.</param>
        internal Chest(dynamic chestJsonDatas) : base(
            (double)chestJsonDatas.X, (double)chestJsonDatas.Y,
            (double)chestJsonDatas.Width, (double)chestJsonDatas.Height)
        {
            _itemType = chestJsonDatas.ItemType;
            _quantity = chestJsonDatas.Quantity;
            _keyId = chestJsonDatas.KeyId;
            _keyIdContainer = chestJsonDatas.KeyIdContainer;
            _render = new ImageRender("Chest");
            _renderOpen = new ImageRender("OpenChest");
        }

        /// <summary>
        /// Tries to open the chest.
        /// </summary>
        internal void TryOpen()
        {
            if (!IsOpen && (!_keyId.HasValue || Engine.Default.Player.Inventory.Keyring.Contains(_keyId.Value)))
            {
                IsOpen = true;
                if (_keyIdContainer.HasValue)
                {
                    Engine.Default.Player.Inventory.AddToKeyring(_keyIdContainer.Value);
                }
                else
                {
                    Engine.Default.Player.Inventory.TryAdd(_itemType, _quantity);
                }
            }
        }
    }
}
