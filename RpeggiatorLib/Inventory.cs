using System.Collections.Generic;
using System.Linq;
using RpeggiatorLib.Enums;
using RpeggiatorLib.Sprites;

namespace RpeggiatorLib
{
    /// <summary>
    /// Represents the player inventory.
    /// </summary>
    public class Inventory
    {
        private List<InventoryItem> _items;
        private Dictionary<ItemType, int> _maxQuantityByItem;
        private List<int> _keyring;

        /// <summary>
        /// List of <see cref="InventoryItem"/> which can be displayed on the screen.
        /// </summary>
        public IReadOnlyCollection<InventoryItem> DisplayableItems
        {
            get
            {
                return _items.Where(it => !it.BaseItem.AmmoFor.HasValue).ToList();
            }
        }
        /// <summary>
        /// Maximal quantity carriable for each item.
        /// </summary>
        public IReadOnlyDictionary<ItemType, int> MaxQuantityByItem { get { return _maxQuantityByItem; } }
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
        /// Inventory max size.
        /// </summary>
        public int InventoryMaxSize { get { return Constants.Inventory.SIZE; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        internal Inventory()
        {
            _items = new List<InventoryItem>();
            _maxQuantityByItem = new Dictionary<ItemType, int>();
            LampIsOn = false;
            foreach (ItemType itemId in Constants.Player.INVENTORY_ITEMS.Keys)
            {
                TryAdd(itemId, Constants.Player.INVENTORY_ITEMS[itemId]);
            }
            Coins = Constants.Player.COINS;
            _keyring = new List<int>();
        }

        /// <summary>
        /// Tries to add or replace an item in the inventory.
        /// </summary>
        /// <param name="itemId"><see cref="Enums.ItemType"/>; <c>Null</c> for coins</param>
        /// <param name="quantity">Quantity.</param>
        /// <returns><c>True</c> if the item has been added; <c>False</c> otherwise.</returns>
        internal int TryAdd(ItemType? itemId, int quantity)
        {
            if (!itemId.HasValue)
            {
                int toReachLimit = Constants.Inventory.COINS_LIMIT - Coins;
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

            if (_items.Any(item => item.BaseItem.Type == itemId.Value))
            {
                remaining = _items.First(item => item.BaseItem.Type == itemId.Value).TryStore(quantity, _maxQuantityByItem[itemId.Value]);
            }
            else if (_items.Count < Constants.Inventory.SIZE)
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
        /// Adds a key to the <see cref="Keyring"/>.
        /// </summary>
        /// <param name="keyId">Key identifier.</param>
        internal void AddToKeyring(int keyId)
        {
            if (!_keyring.Contains(keyId))
            {
                _keyring.Add(keyId);
            }
        }

        /// <summary>
        /// Uses an item of the inventory.
        /// </summary>
        /// <returns><see cref="ActionnedItem"/>; <c>Null</c> if item dropped.</returns>
        internal ActionnedItem UseItem()
        {
            if (!Engine.Default.KeyPress.InventorySlotId.HasValue)
            {
                return null;
            }

            int inventorySlotId = Engine.Default.KeyPress.InventorySlotId.Value;

            if (inventorySlotId >= DisplayableItems.Count)
            {
                return null;
            }

            InventoryItem item = DisplayableItems.ElementAt(inventorySlotId);

            Item ammoItemBase = Item.GetAmmoItem(item.BaseItem.Type);
            
            InventoryItem ammoItem = _items.FirstOrDefault(it => ammoItemBase == it.BaseItem);

            if (!ItemCanBeUseInContext(item.BaseItem.Type) || (ammoItemBase != null ? (ammoItem == null || !ammoItem.TryPick()) : !item.TryPick()))
            {
                return null;
            }

            // Note : items used as ammunitions are never removed from the list.
            if (item.Quantity == 0)
            {
                _items.Remove(item);
            }

            ActionnedItem droppedItem = null;

            switch (item.BaseItem.Type)
            {
                case ItemType.Bomb:
                    droppedItem = new ActionnedBomb(ComputeDropCoordinates(Constants.Bomb.WIDTH, Constants.Bomb.HEIGHT));
                    break;
                case ItemType.SmallLifePotion:
                    Engine.Default.Player.DrinkLifePotion(Constants.Inventory.SMALL_LIFE_POTION_RECOVERY_LIFE_POINTS);
                    break;
                case ItemType.MediumLifePotion:
                    Engine.Default.Player.DrinkLifePotion(Constants.Inventory.MEDIUM_LIFE_POTION_RECOVERY_LIFE_POINTS);
                    break;
                case ItemType.LargeLifePotion:
                    Engine.Default.Player.DrinkLifePotion(Constants.Inventory.LARGE_LIFE_POTION_RECOVERY_LIFE_POINTS);
                    break;
                case ItemType.Lamp:
                    LampIsOn = !LampIsOn;
                    break;
                case ItemType.Bow:
                    droppedItem = new ActionnedArrow(ComputeDropCoordinates(Constants.Arrow.WIDTH, Constants.Arrow.HEIGHT),
                        Engine.Default.Player.Direction, Engine.Default.Player);
                    break;
            }

            return droppedItem;
        }

        // Gets the current quantity of an InventoryItem by its ItemType.
        internal int QuantityOf(ItemType value)
        {
            Item ammoItemBase = Item.GetAmmoItem(value);

            if (ammoItemBase == null)
            {
                return _items.First(it => it.BaseItem.Type == value).Quantity;
            }
            
            return _items.FirstOrDefault(it => ammoItemBase == it.BaseItem)?.Quantity ?? 0;
        }

        // Computes drop coordinates.
        private Point ComputeDropCoordinates(double width, double height)
        {
            // Just a shortcut.
            Player sprite = Engine.Default.Player;

            Point pt = new Point(sprite.X, sprite.Y);
            switch (sprite.Direction)
            {
                case Direction.BottomLeft:
                    pt.Y = sprite.BottomRightY - height;
                    break;
                case Direction.Bottom:
                    pt.X = sprite.X + (sprite.Width / 2);
                    pt.Y = sprite.BottomRightY - height;
                    break;
                case Direction.BottomRight:
                    pt.X = sprite.BottomRightX - width;
                    pt.Y = sprite.BottomRightY - height;
                    break;
                case Direction.Right:
                    pt.X = sprite.BottomRightX - width;
                    pt.Y = sprite.Y + (sprite.Height / 2);
                    break;
                case Direction.TopRight:
                    pt.X = sprite.BottomRightX - width;
                    break;
                case Direction.Top:
                    pt.X = sprite.X + (sprite.Width / 2);
                    break;
                case Direction.Left:
                    pt.Y = sprite.Y + (sprite.Height / 2);
                    break;
            }
            return pt;
        }

        // Checks if an item can be used in the context.
        private bool ItemCanBeUseInContext(ItemType itemId)
        {
            switch (itemId)
            {
                case ItemType.Bomb:
                    if (Engine.Default.Player.CurrentFloor.FloorType == FloorType.Water)
                    {
                        return false;
                    }
                    break;
                case ItemType.SmallLifePotion:
                case ItemType.MediumLifePotion:
                case ItemType.LargeLifePotion:
                    if (Engine.Default.Player.CurrentLifePoints.Equal(Engine.Default.Player.MaximalLifePoints))
                    {
                        return false;
                    }
                    break;
                case ItemType.Bow:
                    if (!_items.Any(it => it.BaseItem.AmmoFor == ItemType.Bow && it.Quantity > 0))
                    {
                        return false;
                    }
                    break;
                case ItemType.Arrow:
                    return false;
            }

            return true;
        }

        // Sets the maximal quantity storable for an InventoryItem.
        private void SetItemMaxQuantity(ItemType itemId, int maxQuantity)
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
