using RPG4.Models.Graphic;
using System;

namespace RPG4.Models.Sprites
{
    /// <summary>
    /// Represents a <see cref="Item"/>, or coins, when pickable on the floor.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class PickableItem : Sprite
    {
        // Time manager.
        private Elapser _timeManager;
        // Null for coins.
        private ItemEnum? _itemId;

        /// <summary>
        /// Indicates the quantity.
        /// </summary>
        public int Quantity { get; private set; }
        /// <summary>
        /// Inferred; indicates the item can be removed from the <see cref="Screen"/>.
        /// </summary>
        public bool Disapear { get { return _timeManager?.Elapsed == true; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="floorItemJson">The json dynamic object.</param>
        public PickableItem(dynamic floorItemJson)
            : base((double)floorItemJson.X, (double)floorItemJson.Y, Item.LOOT_WIDTH, Item.LOOT_HEIGHT,
                  Item.GetItem((ItemEnum)Enum.Parse(typeof(ItemEnum), (string)floorItemJson.ItemId)).LootGraphic)
        {
            _itemId = floorItemJson.ItemId;
            Quantity = floorItemJson.Quantity;
        }

        // Private constructor.
        private PickableItem(double x, double y, double with, double height, ISpriteGraphic graphic,
            ItemEnum? itemId, int quantity, double? timeBeForeDisapear)
            : base(x, y, with, height, graphic)
        {
            _itemId = itemId;
            Quantity = quantity;
            if (timeBeForeDisapear.HasValue)
            {
                _timeManager = new Elapser(timeBeForeDisapear.Value);
            }
        }

        /// <summary>
        /// Builds an instance from enemy's loot.
        /// </summary>
        /// <param name="enemy"><see cref="Enemy"/></param>
        /// <param name="itemId"><see cref="ItemEnum"/>; <c>Null</c> for coin.</param>
        /// <param name="quantity">Quantity looted.</param>
        /// <returns><see cref="PickableItem"/></returns>
        public static PickableItem Loot(Enemy enemy, ItemEnum? itemId, int quantity)
        {
            return new PickableItem(
                enemy.X + (enemy.Width / 2) - (Item.LOOT_WIDTH / 2),
                enemy.Y + (enemy.Height / 2) - (Item.LOOT_HEIGHT / 2),
                Item.LOOT_WIDTH,
                Item.LOOT_HEIGHT,
                itemId.HasValue ? Item.GetItem(itemId.Value).LootGraphic : Item.COIN_GRAPHIC,
                itemId,
                quantity,
                Item.LOOT_LIFETIME);
        }

        /// <summary>
        /// Picks the item.
        /// </summary>
        public void Pick()
        {
            Quantity = Engine.Default.Player.Inventory.TryAdd(_itemId, Quantity);
        }
    }
}
