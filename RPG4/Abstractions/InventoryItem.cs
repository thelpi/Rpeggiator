using System.Collections.Generic;

namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents an <see cref="Item"/> in the player inventory.
    /// </summary>
    public class InventoryItem
    {
        // Indicates if the item is unique.
        private bool _unique;

        /// <summary>
        /// Item identifier.
        /// </summary>
        public ItemIdEnum ItemId { get; private set; }
        /// <summary>
        /// Remaining quantity.
        /// </summary>
        public int Quantity { get; private set; }
        /// <summary>
        /// Maximal quantity carriable.
        /// </summary>
        public int MaxQuantity { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>If <paramref name="quantity"/> is greater than <paramref name="maxQuantity"/>, the second is used for <see cref="Quantity"/>.</remarks>
        /// <param name="idemId"><see cref="ItemId"/></param>
        /// <param name="quantity"><see cref="Quantity"/>; ignored if the item is marked as unique.</param>
        /// <param name="maxQuantity"><see cref="MaxQuantity"/>; ignored if the item is marked as unique.</param>
        public InventoryItem(ItemIdEnum idemId, int quantity, int maxQuantity)
        {
            Item item = Item.GetItem(idemId);

            ItemId = idemId;
            MaxQuantity = item.Unique ? 1 : maxQuantity;
            Quantity = item.Unique ? 1 : (quantity > maxQuantity ? maxQuantity : quantity);
            _unique = item.Unique;
        }

        /// <summary>
        /// Decreases the quantity when used.
        /// </summary>
        public void DecreaseQuantity()
        {
            if (Quantity > 0 && !_unique)
            {
                Quantity--;
            }
        }

        /// <summary>
        /// Tries to increase the quantity.
        /// </summary>
        /// <param name="newQuantity">Quantity to add.</param>
        /// <returns>Remaining quantity if limit reached.</returns>
        public int TryIncreaseQuantity(int newQuantity)
        {
            if (_unique)
            {
                return newQuantity;
            }

            int remaining = 0;

            Quantity += newQuantity;
            if (Quantity > MaxQuantity)
            {
                remaining = Quantity - MaxQuantity;
                Quantity = MaxQuantity;
            }

            return remaining;
        }
    }
}
