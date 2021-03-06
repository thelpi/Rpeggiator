﻿using System.Linq;
using RpeggiatorLib.Renders;

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
        private readonly int _quantity;
        // Optionnal key identifier inside the chest; in that case, replace any value of "_itemType".
        private int? _keyIdContainer;
        // Render after opening the chest.
        private readonly IRender _renderOpen;

        /// <summary>
        /// Indicates if the chest has been open.
        /// </summary>
        public bool IsOpen { get; private set; }
        /// <summary>
        /// The render, which changes regarding if the chest has been opened or not.
        /// </summary>
        public override IRender Render { get { return IsOpen ? _renderOpen : _render; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"><see cref="Sprite.Id"/></param>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="itemType"><see cref="_itemType"/></param>
        /// <param name="quantity"><see cref="_quantity"/></param>
        /// <param name="keyId"><see cref="_keyId"/></param>
        /// <param name="keyIdContainer"><see cref="_keyIdContainer"/></param>
        /// <param name="renderType"><see cref="Enums.RenderType"/></param>
        /// <param name="renderProperties">Datas required to initialize the <see cref="Renders.IRender"/>.</param>
        /// <param name="openRenderType"><see cref="Enums.RenderType"/></param>
        /// <param name="openRenderProperties">Datas required to initialize <see cref="_renderOpen"/>.</param>
        internal Chest(int id, double x, double y, double width, double height,
            Enums.ItemType? itemType, int quantity, int? keyId, int? keyIdContainer,
            Enums.RenderType renderType, string[] renderProperties, Enums.RenderType openRenderType, string[] openRenderProperties)
            : base(id, x, y, width, height, renderType, renderProperties)
        {
            _itemType = itemType;
            _quantity = quantity;
            _keyId = keyId;
            _keyIdContainer = keyIdContainer;
            _renderOpen = GetRenderFromValues(openRenderType, openRenderProperties);
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
