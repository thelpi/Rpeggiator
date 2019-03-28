namespace RPG4.Abstraction
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
        /// Constructor.
        /// </summary>
        /// <param name="itemId"><see cref="BaseItem"/> identifier.</param>
        public InventoryItem(ItemIdEnum itemId) : this(itemId, 1) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="itemId"><see cref="BaseItem"/> identifier.</param>
        /// <param name="quantity"><see cref="Quantity"/>; ignored if the item is marked as unique.</param>
        public InventoryItem(ItemIdEnum itemId, int quantity)
        {
            BaseItem = Item.GetItem(itemId);
            Quantity = BaseItem.Unique ? 1 : quantity;
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
        /// <param name="maxQuantity">Maximal quantity carriable for this item.</param>
        /// <returns>Remaining quantity if limit reached.</returns>
        public int TryIncreaseQuantity(int newQuantity, int maxQuantity)
        {
            if (BaseItem.Unique)
            {
                return newQuantity;
            }

            int remaining = 0;

            Quantity += newQuantity;
            if (Quantity > maxQuantity)
            {
                remaining = Quantity - maxQuantity;
                Quantity = maxQuantity;
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
            return _engineFrameCountFlag > 0
                && (int)(engineFrameCount - _engineFrameCountFlag) <= BaseItem.DelayBetweenUse
                && BaseItem.DelayBetweenUse >= 0;
        }
    }
}
