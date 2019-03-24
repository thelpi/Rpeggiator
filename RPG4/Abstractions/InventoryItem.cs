namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents an <see cref="Item"/> in the player inventory.
    /// </summary>
    public class InventoryItem
    {
        /// <summary>
        /// Item identifier.
        /// </summary>
        public ItemIdEnum ItemId { get; private set; }
        /// <summary>
        /// Remaining quantity.
        /// </summary>
        public int Quantity { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="idemId"><see cref="ItemId"/></param>
        /// <param name="quantity"><see cref="Quantity"/>; ignored if the item is marked as unique.</param>
        public InventoryItem(ItemIdEnum idemId, int quantity)
        {
            Item item = Item.GetItem(idemId);

            ItemId = idemId;
            Quantity = item.Unique ? 1 : quantity;
        }

        /// <summary>
        /// Decreases the quantity when used.
        /// </summary>
        public void DecreaseQuantity()
        {
            if (Quantity > 0)
            {
                Quantity--;
            }
        }
    }
}
