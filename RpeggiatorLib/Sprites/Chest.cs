using System.Linq;
using RpeggiatorLib.Render;

namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents a chest.
    /// </summary>
    public class Chest : Sprite
    {
        // Optionnal key identifier to open the chest.
        private int? _keyId;
        // Item contained in the chest; Null for coins or if "_keyIdContainer" is specified.
        private Enums.ItemType? _itemType;
        // Quantity of what's inside the chest; ignored if "_keyIdContainer" is specified.
        private int _quantity;
        // Optionnal key identifier inside the chest; in that case, replace any value of "_itemType".
        private int? _keyIdContainer;
        // Render after opening the chest.
        private ISpriteRender _renderOpen;

        /// <summary>
        /// Indicates if the chest has been open.
        /// </summary>
        public bool IsOpen { get; private set; }
        /// <summary>
        /// The render, which changes regarding if the chest has been opened or not.
        /// </summary>
        public override ISpriteRender Render { get { return IsOpen ? _renderOpen : _render; } }

        /// <summary>
        /// Creates a <see cref="Chest"/> instance from json datas.
        /// </summary>
        /// <param name="datas">Json datas.</param>
        /// <returns><see cref="Chest"/></returns>
        internal static Chest FromDynamic(dynamic datas)
        {
            Chest c = new Chest((double)datas.X, (double)datas.Y, (double)datas.Width, (double)datas.Height,
                (Enums.ItemType?)datas.ItemType, (int)datas.Quantity, (int?)datas.KeyId, (int?)datas.KeyIdContainer,
                (string)datas.OpenRenderType, (string)datas.OpenRenderValue);
            c.SetRenderFromDynamic((object)datas);
            return c;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="itemType"><see cref="_itemType"/></param>
        /// <param name="quantity"><see cref="_quantity"/></param>
        /// <param name="keyId"><see cref="_keyId"/></param>
        /// <param name="keyIdContainer"><see cref="_keyIdContainer"/></param>
        /// <param name="openRenderType"><see cref="_renderOpen"/> render type.</param>
        /// <param name="openRenderValue"><see cref="_renderOpen"/> render value.</param>
        internal Chest(double x, double y, double width, double height, Enums.ItemType? itemType, int quantity,
            int? keyId, int? keyIdContainer, string openRenderType, string openRenderValue)
            : base(x, y, width, height)
        {
            _itemType = itemType;
            _quantity = quantity;
            _keyId = keyId;
            _keyIdContainer = keyIdContainer;
            switch (openRenderType)
            {
                case nameof(PlainRender):
                    _renderOpen = new PlainRender(openRenderValue);
                    break;
                case nameof(ImageRender):
                    _renderOpen = new ImageRender(openRenderValue);
                    break;
                case nameof(ImageMosaicRender):
                    _renderOpen = new ImageMosaicRender(openRenderValue, this);
                    break;
                default:
                    throw new System.NotImplementedException(Messages.NotImplementedGraphicExceptionMessage);
            }
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
