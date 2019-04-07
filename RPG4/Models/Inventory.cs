using RPG4.Models.Enums;
using RPG4.Models.Sprites;
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
        private Dictionary<Enums.ItemType, int> _maxQuantityByItem;
        private int _creationHashcode;
        private List<int> _keyring;

        /// <summary>
        /// List of <see cref="InventoryItem"/>
        /// </summary>
        public IReadOnlyCollection<InventoryItem> Items { get { return _items; } }
        /// <summary>
        /// Maximal quantity carriable for each item.
        /// </summary>
        public IReadOnlyDictionary<Enums.ItemType, int> MaxQuantityByItem { get { return _maxQuantityByItem; } }
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
            _maxQuantityByItem = new Dictionary<Enums.ItemType, int>();
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
        public int TryAdd(Enums.ItemType? itemId, int quantity)
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

            if (_items.Any(item => item.BaseItem.Id == itemId.Value))
            {
                remaining = _items.First(item => item.BaseItem.Id == itemId.Value).TryStore(quantity, _maxQuantityByItem[itemId.Value]);
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

            if (inventorySlotId >= _items.Count)
            {
                return null;
            }

            InventoryItem item = _items.ElementAt(inventorySlotId);

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
                case Enums.ItemType.Bomb:
                    droppedItem = new ActionnedBomb(ComputeBombDroppingCoordinates());
                    break;
                case Enums.ItemType.SmallLifePotion:
                    Engine.Default.Player.DrinkLifePotion(_creationHashcode, Constants.Inventory.SMALL_LIFE_POTION_RECOVERY_LIFE_POINTS);
                    break;
                case Enums.ItemType.MediumLifePotion:
                    Engine.Default.Player.DrinkLifePotion(_creationHashcode, Constants.Inventory.MEDIUM_LIFE_POTION_RECOVERY_LIFE_POINTS);
                    break;
                case Enums.ItemType.LargeLifePotion:
                    Engine.Default.Player.DrinkLifePotion(_creationHashcode, Constants.Inventory.LARGE_LIFE_POTION_RECOVERY_LIFE_POINTS);
                    break;
                case Enums.ItemType.Lamp:
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
            Player sprite = Engine.Default.Player;

            Point pt = new Point(sprite.X, sprite.Y);
            switch (sprite.Direction)
            {
                case Direction.BottomLeft:
                    pt.Y = sprite.BottomRightY - Constants.Bomb.HEIGHT;
                    break;
                case Direction.Bottom:
                    pt.X = sprite.X + (sprite.Width / 2);
                    pt.Y = sprite.BottomRightY - Constants.Bomb.HEIGHT;
                    break;
                case Direction.BottomRight:
                    pt.X = sprite.BottomRightX - Constants.Bomb.WIDTH;
                    pt.Y = sprite.BottomRightY - Constants.Bomb.HEIGHT;
                    break;
                case Direction.Right:
                    pt.X = sprite.BottomRightX - Constants.Bomb.WIDTH;
                    pt.Y = sprite.Y + (sprite.Height / 2);
                    break;
                case Direction.TopRight:
                    pt.X = sprite.BottomRightX - Constants.Bomb.WIDTH;
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
        /// <param name="itemId"><see cref="Enums.ItemType"/></param>
        /// <returns><c>True</c> if it can be used; <c>False</c> otherwise.</returns>
        private bool ItemCanBeUseInContext(Enums.ItemType itemId)
        {
            switch (itemId)
            {
                case Enums.ItemType.Bomb:
                    // example : not underwater
                    break;
                case Enums.ItemType.SmallLifePotion:
                case Enums.ItemType.MediumLifePotion:
                case Enums.ItemType.LargeLifePotion:
                    if (Engine.Default.Player.CurrentLifePoints.Equal(Engine.Default.Player.MaximalLifePoints))
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
        /// <param name="itemId"><see cref="Enums.ItemType"/></param>
        /// <param name="maxQuantity">Maximal quantity.</param>
        private void SetItemMaxQuantity(Enums.ItemType itemId, int maxQuantity)
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
