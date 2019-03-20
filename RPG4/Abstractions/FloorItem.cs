namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents a <see cref="Item"/> when pickable on the floor.
    /// </summary>
    /// <seealso cref="SizedPoint"/>
    public class FloorItem : SizedPoint
    {
        /// <summary>
        /// The underlying <see cref="Item"/> identifier
        /// </summary>
        public int ItemId { get; private set; }
        /// <summary>
        /// Indicates the quantity.
        /// </summary>
        public int Quantity { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="itemId"><see cref="Item"/> identifier.</param>
        /// <param name="quantity"><see cref="Quantity"/></param>
        /// <param name="x"><see cref="base.X"/></param>
        /// <param name="y"><see cref="base.Y"/></param>
        /// <param name="width"><see cref="base.Width"/></param>
        /// <param name="height"><see cref="base.Height"/></param>
        public FloorItem(int itemId, int quantity, double x, double y, double width, double height)
            : base(x, y, width, height)
        {
            ItemId = itemId;
            Quantity = quantity;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="floorItemJson">The json dynamic object.</param>
        public FloorItem(dynamic floorItemJson) : base((object)floorItemJson)
        {
            ItemId = floorItemJson.ItemId;
            Quantity = floorItemJson.Quantity;
        }
    }
}
