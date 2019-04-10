using RpeggiatorLib.Enums;
using RpeggiatorLib.Graphic;
using System.Linq;

namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents a chest.
    /// </summary>
    public class Chest : Sprite
    {
        private int? _keyId;
        private Enums.ItemType? _itemId;
        private int _quantity;
        private ISpriteGraphic _openGraphic;
        private int? _keyIdContainer;

        /// <summary>
        /// Indicates if the chest is open.
        /// </summary>
        public bool IsOpen { get; private set; }
        /// <inheritdoc />
        public override ISpriteGraphic Graphic { get { return IsOpen ? _openGraphic : base.Graphic; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="chestJsonDatas">Chest json datas.</param>
        public Chest(dynamic chestJsonDatas) : base(
            (double)chestJsonDatas.X, (double)chestJsonDatas.Y,
            (double)chestJsonDatas.Width, (double)chestJsonDatas.Height)
        {
            _itemId = chestJsonDatas.ItemId;
            _quantity = chestJsonDatas.Quantity;
            _keyId = chestJsonDatas.KeyId;
            _keyIdContainer = chestJsonDatas.KeyIdContainer;
            Graphic = new ImageBrushGraphic("Chest");
            _openGraphic = new ImageBrushGraphic("OpenChest");
        }

        /// <summary>
        /// Tries to open the chest.
        /// </summary>
        public void TryOpen()
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
                    Engine.Default.Player.Inventory.TryAdd(_itemId, _quantity);
                }
            }
        }
    }
}
