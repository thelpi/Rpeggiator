using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ReadOnlyCollection<InventoryItem> Items { get { return _items.AsReadOnly(); } }

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
        /// <param name="itemId"><see cref="Item"/> identifier.</param>
        /// <param name="quantity">Quantity.</param>
        /// <param name="removalItemId">Optionnal; <see cref="Item"/> identifier to substitute.</param>
        /// <returns><c>True</c> if the item has been added; <c>False</c> otherwise.</returns>
        public bool TryAdd(int itemId, int quantity, int? removalItemId = null)
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

        /// <summary>
        /// Sets a unique use of the item.
        /// </summary>
        /// <param name="itemId">The <see cref="Item"/> identifier used.</param>
        /// <returns><c>True</c> if the item is out of quantity; <c>False</c> otherwise.</returns>
        public bool UseItem(int itemId)
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
    }
}
