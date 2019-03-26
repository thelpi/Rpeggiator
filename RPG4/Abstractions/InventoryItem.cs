namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents an <see cref="Item"/> in the player inventory.
    /// </summary>
    public class InventoryItem
    {
        private ulong _engineFrameCountFlag;

        /// <summary>
        /// base item.
        /// </summary>
        public Item BaseItem { get; private set; }
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
        /// <param name="idemId"><see cref="BaseItem"/> identifier.</param>
        /// <param name="quantity"><see cref="Quantity"/>; ignored if the item is marked as unique.</param>
        /// <param name="maxQuantity"><see cref="MaxQuantity"/>; ignored if the item is marked as unique.</param>
        public InventoryItem(ItemIdEnum idemId, int quantity, int maxQuantity)
        {
            BaseItem = Item.GetItem(idemId);
            MaxQuantity = BaseItem.Unique ? 1 : maxQuantity;
            Quantity = BaseItem.Unique ? 1 : (quantity > maxQuantity ? maxQuantity : quantity);
            _engineFrameCountFlag = 0;
        }

        /// <summary>
        /// Decreases the quantity when used.
        /// </summary>
        /// <param name="engineFrameCountFlag">The current engine <see cref="AbstractEngine.FramesCount"/> value.</param>
        public void DecreaseQuantity(ulong engineFrameCountFlag)
        {
            _engineFrameCountFlag = engineFrameCountFlag;
            if (Quantity > 0 && !BaseItem.Unique)
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
            if (BaseItem.Unique)
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

        /// <summary>
        /// Indicates if the item is alreayd in use.
        /// </summary>
        /// <param name="engineFrameCount">The current engine <see cref="AbstractEngine.FramesCount"/> value.</param>
        /// <returns><c>True</c> if in use; <c>False</c> otherwise.</returns>
        public bool IsCurrentlyUsed(ulong engineFrameCount)
        {
            return _engineFrameCountFlag > 0 && (int)(engineFrameCount - _engineFrameCountFlag) <= BaseItem.DelayBetweenUse;
        }
    }
}
