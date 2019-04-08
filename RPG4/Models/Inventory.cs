using RPG4.Models.Enums;
using RPG4.Models.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RPG4.Models
{
    /// <summary>
    /// Represents the player inventory.
    /// </summary>
    public class Inventory
    {
        private List<InventoryItem> _items;
        private Dictionary<ItemType, int> _maxQuantityByItem;
        private int _creationHashcode;
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
        /// Constructor.
        /// </summary>
        public Inventory(int creationHashcode)
        {
            _creationHashcode = creationHashcode;
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
        public int TryAdd(ItemType? itemId, int quantity)
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
        public void AddToKeyring(int keyId)
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
        public ActionnedItem UseItem()
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

            IReadOnlyCollection<Item> baseitemList = Item.GetAmmoItem(item.BaseItem.Type);

            // TODO : ugly; mutualisable; the current ammo should be set manually by the player.
            InventoryItem ammoItem = _items.Where(it => baseitemList.Contains(it.BaseItem)).OrderByDescending(it => it.Quantity).FirstOrDefault();

            if (!ItemCanBeUseInContext(item.BaseItem.Type) || (baseitemList.Count > 0 ? (ammoItem == null || !ammoItem.TryPick()) : !item.TryPick()))
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
                    Engine.Default.Player.DrinkLifePotion(_creationHashcode, Constants.Inventory.SMALL_LIFE_POTION_RECOVERY_LIFE_POINTS);
                    break;
                case ItemType.MediumLifePotion:
                    Engine.Default.Player.DrinkLifePotion(_creationHashcode, Constants.Inventory.MEDIUM_LIFE_POTION_RECOVERY_LIFE_POINTS);
                    break;
                case ItemType.LargeLifePotion:
                    Engine.Default.Player.DrinkLifePotion(_creationHashcode, Constants.Inventory.LARGE_LIFE_POTION_RECOVERY_LIFE_POINTS);
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

        /// <summary>
        /// Gets the current quantity of an <see cref="InventoryItem"/> by its <see cref="ItemType"/>.
        /// </summary>
        /// <param name="value"><see cref="ItemType"/></param>
        /// <returns>Quantity.</returns>
        public int QuantityOf(ItemType value)
        {
            IReadOnlyCollection<Item> baseitemList = Item.GetAmmoItem(value);

            if (baseitemList.Count == 0)
            {
                return _items.First(it => it.BaseItem.Type == value).Quantity;
            }

            // TODO : ugly; mutualisable; the current ammo should be set manually by the player.
            return _items
                .Where(it => baseitemList.Contains(it.BaseItem))
                .OrderByDescending(it => it.Quantity)
                .FirstOrDefault()?.Quantity ?? 0;
        }

        /// <summary>
        /// Computes drop coordinates.
        /// </summary>
        /// <param name="width">Item sprite width.</param>
        /// <param name="height">Item sprite height.</param>
        /// <returns>Coordinates point.</returns>
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

        /// <summary>
        /// Checks if an item can be used in the context.
        /// </summary>
        /// <param name="itemId"><see cref="ItemType"/></param>
        /// <returns><c>True</c> if it can be used; <c>False</c> otherwise.</returns>
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

        /// <summary>
        /// Sets the maximal quantity storable for an <see cref="InventoryItem"/>.
        /// </summary>
        /// <remarks>The storage capacity can't be decrease.</remarks>
        /// <param name="itemId"><see cref="ItemType"/></param>
        /// <param name="maxQuantity">Maximal quantity.</param>
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
