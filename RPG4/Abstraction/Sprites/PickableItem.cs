using System;

namespace RPG4.Abstraction.Sprites
{
    /// <summary>
    /// Represents a <see cref="Item"/> when pickable on the floor.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class PickableItem : Sprite
    {
        /// <summary>
        /// <see cref="ItemIdEnum"/>
        /// </summary>
        public ItemIdEnum ItemId { get; private set; }
        /// <summary>
        /// Indicates the quantity.
        /// </summary>
        public int Quantity { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="itemId"><see cref="ItemId"/></param>
        /// <param name="quantity"><see cref="Quantity"/></param>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        public PickableItem(ItemIdEnum itemId, int quantity, double x, double y, double width, double height)
            : base(x, y, width, height)
        {
            ItemId = itemId;
            Quantity = quantity;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="floorItemJson">The json dynamic object.</param>
        public PickableItem(dynamic floorItemJson) : base((object)floorItemJson)
        {
            string realJsonValue = floorItemJson.ItemId;

            ItemId = (ItemIdEnum)Enum.Parse(typeof(ItemIdEnum), realJsonValue);
            Quantity = floorItemJson.Quantity;
        }

        /// <summary>
        /// Picks the item.
        /// </summary>
        /// <param name="engine"><see cref="AbstractEngine"/></param>
        public void Pick(AbstractEngine engine)
        {
            Quantity = engine.Player.Inventory.TryAdd(ItemId, Quantity);
        }
    }
}
