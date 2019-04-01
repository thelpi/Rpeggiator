using RPG4.Abstraction.Sprites;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RPG4.Abstraction
{
    /// <summary>
    /// Represents the player inventory.
    /// </summary>
    public class Inventory
    {
        // Recovered life points by drinking small life potion.
        private const double SMALL_LIFE_POTION_RECOVERY_LIFE_POINTS = 2;
        // Recovered life points by drinking medium life potion.
        private const double MEDIUM_LIFE_POTION_RECOVERY_LIFE_POINTS = 5;
        // Recovered life points by drinking large life potion.
        private const double LARGE_LIFE_POTION_RECOVERY_LIFE_POINTS = 10;
        /// <summary>
        /// Initial size of the inventory.
        /// </summary>
        public const int SIZE = 10;
        /// <summary>
        /// Coins limit.
        /// </summary>
        private const int COINS_LIMIT = 999;

        private List<InventoryItem> _items;
        private Dictionary<ItemIdEnum, int> _maxQuantityByItem;
        private int _creationHashcode;
        private List<int> _keyring;

        /// <summary>
        /// List of <see cref="InventoryItem"/>
        /// </summary>
        public IReadOnlyCollection<InventoryItem> Items { get { return _items; } }
        /// <summary>
        /// Maximal quantity carriable for each item.
        /// </summary>
        public IReadOnlyDictionary<ItemIdEnum, int> MaxQuantityByItem { get { return _maxQuantityByItem; } }
        /// <summary>
        /// Indicates if the lamp item is currently used.
        /// </summary>
        public bool LampIsOn { get; private set; }
        /// <summary>
        /// Coins.
        /// </summary>
        public int Coins { get; private set; }
        /// <summary>
        /// Keyring.
        /// </summary>
        public IReadOnlyCollection<int> Keyring { get { return _keyring; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Inventory(int creationHashcode)
        {
            _creationHashcode = creationHashcode;
            _items = new List<InventoryItem>();
            _maxQuantityByItem = new Dictionary<ItemIdEnum, int>();
            LampIsOn = false;
            foreach (var itemId in InitialPlayerStatus.INVENTORY_ITEMS.Keys)
            {
                TryAdd(itemId, InitialPlayerStatus.INVENTORY_ITEMS[itemId]);
            }
            Coins = InitialPlayerStatus.COINS;
            _keyring = new List<int>();
        }

        /// <summary>
        /// Tries to add or replace an item in the inventory.
        /// </summary>
        /// <param name="itemId"><see cref="ItemIdEnum"/>; <c>Null</c> for coins</param>
        /// <param name="quantity">Quantity.</param>
        /// <returns><c>True</c> if the item has been added; <c>False</c> otherwise.</returns>
        public int TryAdd(ItemIdEnum? itemId, int quantity)
        {
            if (!itemId.HasValue)
            {
                int toReachLimit = COINS_LIMIT - Coins;
                if (toReachLimit == 0 || toReachLimit < quantity)
                {
                    Coins += toReachLimit;
                    return quantity - toReachLimit;
                }
                else
                {
                    Coins += quantity;
                    return 0;
                }
            }

            int remaining = 0;

            if (_items.Any(item => item.BaseItem.Id == itemId.Value))
            {
                remaining = _items.First(item => item.BaseItem.Id == itemId.Value).TryStore(quantity, _maxQuantityByItem[itemId.Value]);
            }
            else if (_items.Count < SIZE)
            {
                _items.Add(new InventoryItem(itemId.Value, quantity));
                SetItemMaxQuantity(itemId.Value, Item.GetItem(itemId.Value).InitialMaximalQuantity);
            }
            else
            {
                remaining = quantity;
            }
            return remaining;
        }

        /// <summary>
        /// Uses an item of the inventory.
        /// </summary>
        /// <returns><see cref="ActionnedItem"/>; <c>Null</c> if item dropped.</returns>
        public ActionnedItem UseItem()
        {
            if (!Engine.Default.KeyPress.InventorySlotId.HasValue)
            {
                return null;
            }

            var inventorySlotId = Engine.Default.KeyPress.InventorySlotId.Value;

            if (inventorySlotId >= _items.Count)
            {
                return null;
            }

            var item = _items.ElementAt(inventorySlotId);

            if (!ItemCanBeUseInContext(item.BaseItem.Id) || !item.TryPick())
            {
                return null;
            }

            if (item.Quantity == 0)
            {
                _items.Remove(item);
            }

            ActionnedItem droppedItem = null;

            switch (item.BaseItem.Id)
            {
                case ItemIdEnum.Bomb:
                    droppedItem = new ActionnedBomb(ComputeBombDroppingCoordinates());
                    break;
                case ItemIdEnum.SmallLifePotion:
                    Engine.Default.Player.DrinkLifePotion(_creationHashcode, SMALL_LIFE_POTION_RECOVERY_LIFE_POINTS);
                    break;
                case ItemIdEnum.MediumLifePotion:
                    Engine.Default.Player.DrinkLifePotion(_creationHashcode, MEDIUM_LIFE_POTION_RECOVERY_LIFE_POINTS);
                    break;
                case ItemIdEnum.LargeLifePotion:
                    Engine.Default.Player.DrinkLifePotion(_creationHashcode, LARGE_LIFE_POTION_RECOVERY_LIFE_POINTS);
                    break;
                case ItemIdEnum.Lamp:
                    LampIsOn = !LampIsOn;
                    break;
            }

            return droppedItem;
        }

        /// <summary>
        /// Computes bom dropping coordinates.
        /// </summary>
        /// <returns>Coordinates point.</returns>
        private Point ComputeBombDroppingCoordinates()
        {
            // Just a shortcut.
            var sprite = Engine.Default.Player;

            Point pt = new Point(sprite.X, sprite.Y);
            switch (sprite.LastDirection)
            {
                case Directions.bottom_left:
                    pt.Y = sprite.BottomRightY - ActionnedBomb.HEIGHT;
                    break;
                case Directions.bottom:
                    pt.X = sprite.X + (sprite.Width / 2);
                    pt.Y = sprite.BottomRightY - ActionnedBomb.HEIGHT;
                    break;
                case Directions.bottom_right:
                    pt.X = sprite.BottomRightX - ActionnedBomb.WIDTH;
                    pt.Y = sprite.BottomRightY - ActionnedBomb.HEIGHT;
                    break;
                case Directions.right:
                    pt.X = sprite.BottomRightX - ActionnedBomb.WIDTH;
                    pt.Y = sprite.Y + (sprite.Height / 2);
                    break;
                case Directions.top_right:
                    pt.X = sprite.BottomRightX - ActionnedBomb.WIDTH;
                    break;
                case Directions.top:
                    pt.X = sprite.X + (sprite.Width / 2);
                    break;
                case Directions.left:
                    pt.Y = sprite.Y + (sprite.Height / 2);
                    break;
            }
            return pt;
        }

        /// <summary>
        /// Checks if an item can be used in the context.
        /// </summary>
        /// <param name="itemId"><see cref="ItemIdEnum"/></param>
        /// <returns><c>True</c> if it can be used; <c>False</c> otherwise.</returns>
        private bool ItemCanBeUseInContext(ItemIdEnum itemId)
        {
            switch (itemId)
            {
                case ItemIdEnum.Bomb:
                    // example : not underwater
                    break;
                case ItemIdEnum.SmallLifePotion:
                case ItemIdEnum.MediumLifePotion:
                case ItemIdEnum.LargeLifePotion:
                    if (Engine.Default.Player.CurrentLifePoints == Engine.Default.Player.MaximalLifePoints)
                    {
                        return false;
                    }
                    break;
            }

            return true;
        }

        /// <summary>
        /// Sets the maximal quantity storable for an <see cref="InventoryItem"/>.
        /// </summary>
        /// <remarks>The storage capacity can't be decrease.</remarks>
        /// <param name="itemId"><see cref="ItemIdEnum"/></param>
        /// <param name="maxQuantity">Maximal quantity.</param>
        private void SetItemMaxQuantity(ItemIdEnum itemId, int maxQuantity)
        {
            if (_maxQuantityByItem.ContainsKey(itemId))
            {
                if (_maxQuantityByItem[itemId] < maxQuantity)
                {
                    _maxQuantityByItem[itemId] = maxQuantity;
                }
            }
            else
            {
                _maxQuantityByItem.Add(itemId, maxQuantity);
            }
        }
    }
}
