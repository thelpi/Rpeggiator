using RPG4.Abstraction.Graphic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG4.Abstraction.Sprites
{
    /// <summary>
    /// Represents a chest.
    /// </summary>
    public class Chest : Sprite
    {
        private int? _keyId;
        private ItemIdEnum? _itemId;
        private int _quantity;
        private ISpriteGraphic _openGraphic;

        /// <summary>
        /// Indicates if the chest is open.
        /// </summary>
        public bool IsOpen { get; private set; }
        /// <inheritdoc />
        public new ISpriteGraphic Graphic { get { return IsOpen ? _openGraphic : base.Graphic; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="chestJsonDatas">Chest json datas.</param>
        public Chest(dynamic chestJsonDatas) : base((object)chestJsonDatas)
        {
            _itemId = chestJsonDatas.ItemId;
            _quantity = chestJsonDatas.Quantity;
            _keyId = chestJsonDatas.KeyId;
            _openGraphic = chestJsonDatas.OpenGraphic;
            switch ((string)chestJsonDatas.GraphicType)
            {
                case nameof(ImageBrushGraphic):
                    _openGraphic = new ImageBrushGraphic((string)chestJsonDatas.OpenHexColor);
                    break;
                case nameof(PlainBrushGraphic):
                    _openGraphic = new PlainBrushGraphic((string)chestJsonDatas.OpenImagePath);
                    break;
                    // TODO : other types of ISpriteGraphic must be implemented here.
            }
        }

        /// <summary>
        /// Tries to open the chest.
        /// </summary>
        public void TryOpen()
        {
            if (!IsOpen && !_keyId.HasValue || Engine.Default.Player.Inventory.Keyring.Contains(_keyId.Value))
            {
                IsOpen = true;
                Engine.Default.Player.Inventory.TryAdd(_itemId, _quantity);
            }
        }
    }
}
