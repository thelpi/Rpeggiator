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
        /// <param name="floorItemJson">The json dynamic object.</param>
        public PickableItem(dynamic floorItemJson) : base((object)floorItemJson)
        {
            ItemId = (ItemIdEnum)Enum.Parse(typeof(ItemIdEnum), (string)floorItemJson.ItemId);
            Quantity = floorItemJson.Quantity;
        }

        /// <summary>
        /// Picks the item.
        /// </summary>
        /// <param name="engine"><see cref="Engine"/></param>
        public void Pick(Engine engine)
        {
            Quantity = engine.Player.Inventory.TryAdd(ItemId, Quantity);
        }
    }
}
