﻿using RPG4.Abstraction.Sprites;
using System.Collections.Generic;
using System.Linq;

namespace RPG4.Abstraction
{
    /// <summary>
    /// Represents the player inventory.
    /// </summary>
    public class Inventory
    {
        private List<InventoryItem> _items;
        private Dictionary<ItemIdEnum, int> _maxQuantityByItem;

        /// <summary>
        /// List of <see cref="InventoryItem"/>
        /// </summary>
        public IReadOnlyCollection<InventoryItem> Items { get { return _items; } }
        /// <summary>
        /// Maximal quantity carriable for each item.
        /// </summary>
        public IReadOnlyDictionary<ItemIdEnum, int> MaxQuantityByItem { get { return _maxQuantityByItem; } }
        /// <summary>
        /// Indicates if the lamp item is currently used.
        /// </summary>
        public bool LampIsOn { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Inventory()
        {
            _items = new List<InventoryItem>();
            _maxQuantityByItem = new Dictionary<ItemIdEnum, int>();
            LampIsOn = false;
            foreach (var itemId in InitialPlayerStatus.INVENTORY_ITEMS.Keys)
            {
                TryAdd(itemId, InitialPlayerStatus.INVENTORY_ITEMS[itemId]);
            }
        }

        /// <summary>
        /// Tries to add or replace an item in the inventory.
        /// </summary>
        /// <param name="itemId"><see cref="ItemIdEnum"/></param>
        /// <param name="quantity">Quantity.</param>
        /// <returns><c>True</c> if the item has been added; <c>False</c> otherwise.</returns>
        public int TryAdd(ItemIdEnum itemId, int quantity)
        {
            int remaining = 0;

            if (_items.Any(item => item.BaseItem.Id == itemId))
            {
                remaining = _items.First(item => item.BaseItem.Id == itemId).TryStore(quantity, _maxQuantityByItem[itemId]);
            }
            else if (_items.Count < Constants.INVENTORY_SIZE)
            {
                _items.Add(new InventoryItem(itemId, quantity));
                SetItemMaxQuantity(itemId, Item.GetItem(itemId).InitialMaximalQuantity);
            }
            else
            {
                remaining = quantity;
            }
            return remaining;
        }

        /// <summary>
        /// Uses an item of the inventory.
        /// </summary>
        /// <param name="engine"><see cref="AbstractEngine"/></param>
        /// <param name="inventorySlotId">Inventory slot index.</param>
        /// <returns><see cref="ActionnedItem"/>; <c>Null</c> if item dropped.</returns>
        public ActionnedItem UseItem(AbstractEngine engine, int inventorySlotId)
        {
            if (inventorySlotId >= _items.Count)
            {
                return null;
            }

            var item = _items.ElementAt(inventorySlotId);

            if (!item.TryPick())
            {
                return null;
            }

            if (item.Quantity == 0)
            {
                _items.Remove(item);
            }

            ActionnedItem droppedItem = null;

            switch (item.BaseItem.Id)
            {
                case ItemIdEnum.Bomb:
                    droppedItem = new ActionnedBomb(engine.Player.X, engine.Player.Y);
                    break;
                case ItemIdEnum.SmallLifePotion:
                case ItemIdEnum.MediumLifePotion:
                case ItemIdEnum.LargeLifePotion:
                    engine.Player.DrinkLifePotion(item.BaseItem.Id);
                    break;
                case ItemIdEnum.Lamp:
                    LampIsOn = !LampIsOn;
                    break;
            }

            return droppedItem;
        }

        /// <summary>
        /// Sets the maximal quantity storable for an <see cref="InventoryItem"/>.
        /// </summary>
        /// <remarks>The storage capacity can't be decrease.</remarks>
        /// <param name="itemId"><see cref="ItemIdEnum"/></param>
        /// <param name="maxQuantity">Maximal quantity.</param>
        private void SetItemMaxQuantity(ItemIdEnum itemId, int maxQuantity)
        {
            if (_maxQuantityByItem.ContainsKey(itemId))
            {
                if (_maxQuantityByItem[itemId] < maxQuantity)
                {
                    _maxQuantityByItem[itemId] = maxQuantity;
                }
            }
            else
            {
                _maxQuantityByItem.Add(itemId, maxQuantity);
            }
        }
    }
}
