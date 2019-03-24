using System.Collections.Generic;
using System.Linq;

namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents the player inventory.
    /// </summary>
    public class Inventory
    {
        private List<InventoryItem> _items;

        /// <summary>
        /// Maximal size of the inventory.
        /// </summary>
        public int Size { get; private set; }
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
            Size = InitialPlayerStatus.INVENTORY_SIZE;
        }

        /// <summary>
        /// Tries to add or replace an item in the inventory.
        /// </summary>
        /// <param name="itemId"><see cref="ItemIdEnum"/></param>
        /// <param name="quantity">Quantity.</param>
        /// <param name="removalItemId">Optionnal; <see cref="ItemIdEnum"/> to substitute.</param>
        /// <returns><c>True</c> if the item has been added; <c>False</c> otherwise.</returns>
        public bool TryAdd(ItemIdEnum itemId, int quantity, ItemIdEnum? removalItemId = null)
        {
            if (removalItemId.HasValue && _items.Any(item => item.ItemId == removalItemId.Value))
            {
                _items.RemoveAll(item => item.ItemId == removalItemId.Value);
            }
            if (_items.Count < Size)
            {
                _items.Add(new InventoryItem(itemId, quantity));
                return true;
            }
            return false;
        }

        // Sets a unique use of the item.
        private bool MarkItemAsUsed(ItemIdEnum itemId)
        {
            var item = _items.Find(it => it.ItemId == itemId);
            item.DecreaseQuantity();
            if (item.Quantity == 0)
            {
                _items.Remove(item);
                return true;
            }
            return false;
        }

        // Gets the slot associated to an item identifier.
        private int GetSlotByItemId(ItemIdEnum itemId)
        {
            return _items.FindIndex(it => it.ItemId == itemId);
        }

        /// <summary>
        /// Uses an item of the inventory.
        /// </summary>
        /// <param name="engine"><see cref="AbstractEngine"/></param>
        /// <param name="inventorySlotId">Inventory slot index.</param>
        /// <returns>Item dropped; <c>Null</c> if item dropped.</returns>
        public Sprite UseItem(AbstractEngine engine, int inventorySlotId)
        {
            // checks for bombs dropped on the new position
            int indexId = GetSlotByItemId(ItemIdEnum.Bomb);
            if (inventorySlotId == indexId)
            {
                MarkItemAsUsed(ItemIdEnum.Bomb);
                return new Bomb(engine.Player.X, engine.Player.Y);
            }

            // checks for small life potions
            indexId = GetSlotByItemId(ItemIdEnum.SmallLifePotion);
            if (inventorySlotId == indexId)
            {
                engine.Player.DrinkLifePotion(ItemIdEnum.SmallLifePotion);
                MarkItemAsUsed(ItemIdEnum.SmallLifePotion);
            }

            return null;
        }
    }
}
