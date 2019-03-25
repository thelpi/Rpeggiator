using System.Collections.Generic;
using System.Linq;

namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents the player inventory.
    /// </summary>
    public class Inventory
    {
        /// <summary>
        /// Quantity
        /// </summary>
        /// <remarks>
        /// Move somewhere else when ready.
        /// If <see cref="Item.Unique"/> is set on the item, the value here should be ignored.
        /// </remarks>
        public static readonly IReadOnlyDictionary<ItemIdEnum, int> MAX_QUANTITY_BY_ITEM = new Dictionary<ItemIdEnum, int>
        {
            { ItemIdEnum.Bomb, 20 },
            { ItemIdEnum.SmallLifePotion, 20 },
            { ItemIdEnum.MediumLifePotion, 20 },
            { ItemIdEnum.LargeLifePotion, 20 },
        };

        private List<InventoryItem> _items;

        /// <summary>
        /// List of <see cref="InventoryItem"/>
        /// </summary>
        public IReadOnlyCollection<InventoryItem> Items { get { return _items; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Inventory()
        {
            _items = InitialPlayerStatus.INVENTORY_ITEMS.ToList();
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

            if (_items.Any(item => item.ItemId == itemId))
            {
                remaining = _items.First(item => item.ItemId == itemId).TryIncreaseQuantity(quantity);
            }
            else if (_items.Count < Constants.INVENTORY_SIZE)
            {
                _items.Add(new InventoryItem(itemId, quantity, MAX_QUANTITY_BY_ITEM[itemId]));
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
            ActionnedItem droppedItem = null;

            switch (item.ItemId)
            {
                case ItemIdEnum.Bomb:
                    droppedItem = new ActionnedBomb(engine.Player.X, engine.Player.Y);
                    break;
                case ItemIdEnum.SmallLifePotion:
                case ItemIdEnum.MediumLifePotion:
                case ItemIdEnum.LargeLifePotion:
                    engine.Player.DrinkLifePotion(item.ItemId);
                    break;
            }

            item.DecreaseQuantity();
            if (item.Quantity == 0)
            {
                _items.Remove(item);
            }

            return droppedItem;
        }
    }
}
