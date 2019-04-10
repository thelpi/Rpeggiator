using RpeggiatorLib.Enums;

namespace RpeggiatorLib
{
    /// <summary>
    /// Represents an <see cref="Item"/> in the player inventory.
    /// </summary>
    public class InventoryItem
    {
        // Time manager between each use.
        private Elapser _useTimeManager;

        /// <summary>
        /// base item.
        /// </summary>
        public Item BaseItem { get; private set; }
        /// <summary>
        /// Remaining quantity.
        /// </summary>
        public int Quantity { get; private set; }
        /// <summary>
        /// Displayable remaining quantity.
        /// </summary>
        public int DisplayableQuantity
        {
            get
            {
                return Engine.Default.Player.Inventory.QuantityOf(BaseItem.Type);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="itemId"><see cref="BaseItem"/> identifier.</param>
        internal InventoryItem(ItemType itemId) : this(itemId, 1) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="itemId"><see cref="BaseItem"/> identifier.</param>
        /// <param name="quantity"><see cref="Quantity"/>; ignored if the item is marked as unique.</param>
        internal InventoryItem(ItemType itemId, int quantity)
        {
            BaseItem = Item.GetItem(itemId);
            Quantity = BaseItem.Unique ? 1 : quantity;
        }

        /// <summary>
        /// Tries to pick a single use of the item.
        /// </summary>
        /// <returns><c>True</c> if pickable; <c>False</c> otherwise.</returns>
        internal bool TryPick()
        {
            if (_useTimeManager?.Elapsed == false)
            {
                return false;
            }

            _useTimeManager = new Elapser(BaseItem.UseDelay);
            if (Quantity > 0 && !BaseItem.Unique)
            {
                Quantity--;
            }

            return true;
        }

        /// <summary>
        /// Tries to increase the quantity.
        /// </summary>
        /// <param name="addedQuantity">Quantity to add.</param>
        /// <param name="maxQuantity">Maximal quantity carriable for this item.</param>
        /// <returns>Remaining quantity if limit reached.</returns>
        internal int TryStore(int addedQuantity, int maxQuantity)
        {
            if (BaseItem.Unique)
            {
                return addedQuantity;
            }

            int remaining = 0;

            Quantity += addedQuantity;
            if (Quantity > maxQuantity)
            {
                remaining = Quantity - maxQuantity;
                Quantity = maxQuantity;
            }

            return remaining;
        }
    }
}
