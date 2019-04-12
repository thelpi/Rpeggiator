using RpeggiatorLib.Enums;
using RpeggiatorLib.Render;

namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents a <see cref="Item"/>, or coins, when pickable on the floor.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class PickableItem : Sprite
    {
        // Time manager.
        private Elapser _timeManager;

        /// <summary>
        /// Indicates the quantity.
        /// </summary>
        public int Quantity { get; private set; }
        /// <summary>
        /// Inferred; indicates the item can be removed from the <see cref="Screen"/>.
        /// </summary>
        public bool Disapear { get { return _timeManager?.Elapsed == true; } }
        /// <summary>
        /// <see cref="ItemType"/>. <c>Null</c> for coins.
        /// </summary>
        public ItemType? ItemType { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="floorItemJson">The json dynamic object.</param>
        internal PickableItem(dynamic floorItemJson) : this(
            (double)floorItemJson.X, (double)floorItemJson.Y,
            Constants.Item.LOOT_WIDTH, Constants.Item.LOOT_HEIGHT,
            (ItemType?)floorItemJson.ItemType, (int)floorItemJson.Quantity, null)
        { }

        // Private constructor.
        private PickableItem(double x, double y, double with, double height,
            ItemType? itemType, int quantity, double? timeBeForeDisapear) : base(x, y, with, height)
        {
            ItemType = itemType;
            Quantity = quantity;
            if (timeBeForeDisapear.HasValue)
            {
                _timeManager = new Elapser(timeBeForeDisapear.Value);
            }

            _render = new ImageRender(itemType.HasValue ? ItemType.Value.ToString() : "Coin");
        }

        /// <summary>
        /// Builds an instance from enemy's loot.
        /// </summary>
        /// <param name="enemy"><see cref="Enemy"/></param>
        /// <param name="itemType"><see cref="Enums.ItemType"/>; <c>Null</c> for coin.</param>
        /// <param name="quantity">Quantity looted.</param>
        /// <returns><see cref="PickableItem"/></returns>
        internal static PickableItem Loot(Enemy enemy, ItemType? itemType, int quantity)
        {
            return new PickableItem(
                enemy.X + (enemy.Width / 2) - (Constants.Item.LOOT_WIDTH / 2),
                enemy.Y + (enemy.Height / 2) - (Constants.Item.LOOT_HEIGHT / 2),
                Constants.Item.LOOT_WIDTH,
                Constants.Item.LOOT_HEIGHT,
                itemType,
                quantity,
                Constants.Item.LOOT_LIFETIME);
        }

        /// <summary>
        /// Picks the item.
        /// </summary>
        internal void Pick()
        {
            Quantity = Engine.Default.Player.Inventory.TryAdd(ItemType, Quantity);
        }
    }
}
