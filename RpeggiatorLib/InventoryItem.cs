using RpeggiatorLib.Enums;
using RpeggiatorLib.Render;

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
        /// <see cref="ISpriteRender"/>
        /// </summary>
        public ISpriteRender Render { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="itemType"><see cref="ItemType"/>.</param>
        internal InventoryItem(ItemType itemType) : this(itemType, 1) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="itemType"><see cref="ItemType"/>.</param>
        /// <param name="quantity"><see cref="Quantity"/>; ignored if the item is marked as unique.</param>
        internal InventoryItem(ItemType itemType, int quantity)
        {
            BaseItem = Item.GetItem(itemType);
            Quantity = BaseItem.Unique ? 1 : quantity;
            Render = new ImageRender(BaseItem.Type.ToString());
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
