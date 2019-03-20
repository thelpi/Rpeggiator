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
        public int ItemId { get; private set; }
        /// <summary>
        /// Remaining quantity.
        /// </summary>
        public int Quantity { get; private set; }
        /*/// <summary>
        /// Remaining lifetime, in ticks.
        /// </summary>
        /// <remarks>-1 if the item doesn't have lifetime.</remarks>
        public int RemainingLifetime { get; private set; }*/

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="idemId"><see cref="ItemId"/></param>
        /// <param name="quantity"><see cref="Quantity"/>; ignored if the item is marked as unique.</param>
        public InventoryItem(int idemId, int quantity)
        {
            Item item = Item.GetItem(idemId);

            ItemId = idemId;
            Quantity = item.Unique ? 1 : quantity;
            //RemainingLifetime = item.Lifetime;
        }

        /*/// <summary>
        /// Decreases the life time at a tick.
        /// </summary>
        /// <returns><c>True</c> if the lifetime reachs the end; <c>False</c> otherwise.</returns>
        public bool DecreaseLifetime()
        {
            if (RemainingLifetime > 0)
            {
                RemainingLifetime--;
            }
            return RemainingLifetime == 0;
        }*/

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
