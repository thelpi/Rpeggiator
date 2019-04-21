using RpeggiatorLib.Enums;
using RpeggiatorLib.Renders;

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
        /// Builds an instance from enemy's loot.
        /// </summary>
        /// <param name="enemy"><see cref="Enemy"/></param>
        /// <param name="itemType"><see cref="Enums.ItemType"/>; <c>Null</c> for coin.</param>
        /// <param name="quantity">Quantity looted.</param>
        /// <returns><see cref="PickableItem"/></returns>
        internal static PickableItem Loot(Enemy enemy, ItemType? itemType, int quantity)
        {
            return new PickableItem(
                0,
                enemy.X + (enemy.Width / 2) - (Constants.Item.LOOT_WIDTH / 2),
                enemy.Y + (enemy.Height / 2) - (Constants.Item.LOOT_HEIGHT / 2),
                Constants.Item.LOOT_WIDTH,
                Constants.Item.LOOT_HEIGHT,
                itemType,
                quantity,
                Constants.Item.LOOT_LIFETIME);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"><see cref="Sprite.Id"/></param>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="itemType"><see cref="ItemType"/></param>
        /// <param name="quantity"><see cref="Quantity"/></param>
        /// <param name="timeBeforeDisapear"><see cref="_timeManager"/> lifetime, in milliseconds.</param>
        internal PickableItem(int id, double x, double y, double width, double height,
            ItemType? itemType, int quantity, double? timeBeforeDisapear)
            : base(id, x, y, width, height, nameof(ImageRender), new[] { itemType.HasValue ? itemType.Value.ToString() : nameof(Filename.Coin) })
        {
            ItemType = itemType;
            Quantity = quantity;
            if (timeBeforeDisapear.HasValue)
            {
                _timeManager = new Elapser(timeBeforeDisapear.Value);
            }
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
