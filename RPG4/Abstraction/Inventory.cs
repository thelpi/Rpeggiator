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
        private List<InventoryItem> _items;
        private Dictionary<ItemIdEnum, int> _maxQuantityByItem;
        private int _creationHashcode;

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
        }

        /// <summary>
        /// Tries to add or replace an item in the inventory.
        /// </summary>
        /// <param name="itemId"><see cref="ItemIdEnum"/></param>
        /// <param name="quantity">Quantity.</param>
        /// <returns><c>True</c> if the item has been added; <c>False</c> otherwise.</returns>
        public int TryAdd(ItemIdEnum itemId, int quantity)
        {
            int remaining = 0;

            if (_items.Any(item => item.BaseItem.Id == itemId))
            {
                remaining = _items.First(item => item.BaseItem.Id == itemId).TryStore(quantity, _maxQuantityByItem[itemId]);
            }
            else if (_items.Count < Constants.INVENTORY_SIZE)
            {
                _items.Add(new InventoryItem(itemId, quantity));
                SetItemMaxQuantity(itemId, Item.GetItem(itemId).InitialMaximalQuantity);
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
        /// <param name="engine"><see cref="AbstractEngine"/></param>
        /// <param name="inventorySlotId">Inventory slot index.</param>
        /// <returns><see cref="ActionnedItem"/>; <c>Null</c> if item dropped.</returns>
        public ActionnedItem UseItem(AbstractEngine engine, int inventorySlotId)
        {
            if (inventorySlotId >= _items.Count)
            {
                return null;
            }

            var item = _items.ElementAt(inventorySlotId);

            if (!ItemCanBeUseInContext(item.BaseItem.Id, engine) || !item.TryPick())
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
                    droppedItem = new ActionnedBomb(ComputeBombDroppingCoordinates(engine.Player));
                    break;
                case ItemIdEnum.SmallLifePotion:
                    engine.Player.DrinkLifePotion(_creationHashcode, Constants.SMALL_LIFE_POTION_RECOVERY_LIFE_POINTS);
                    break;
                case ItemIdEnum.MediumLifePotion:
                    engine.Player.DrinkLifePotion(_creationHashcode, Constants.MEDIUM_LIFE_POTION_RECOVERY_LIFE_POINTS);
                    break;
                case ItemIdEnum.LargeLifePotion:
                    engine.Player.DrinkLifePotion(_creationHashcode, Constants.LARGE_LIFE_POTION_RECOVERY_LIFE_POINTS);
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
        /// <param name="player"><see cref="Player"/></param>
        /// <returns>Coordinates point.</returns>
        private Point ComputeBombDroppingCoordinates(Player player)
        {
            Point pt = new Point(player.X, player.Y);
            switch (player.LastDirection)
            {
                case Directions.bottom_left:
                    pt.Y = player.BottomRightY - ActionnedBomb.HEIGHT;
                    break;
                case Directions.bottom:
                    pt.X = player.X + (player.Width / 2);
                    pt.Y = player.BottomRightY - ActionnedBomb.HEIGHT;
                    break;
                case Directions.bottom_right:
                    pt.X = player.BottomRightX - ActionnedBomb.WIDTH;
                    pt.Y = player.BottomRightY - ActionnedBomb.HEIGHT;
                    break;
                case Directions.right:
                    pt.X = player.BottomRightX - ActionnedBomb.WIDTH;
                    pt.Y = player.Y + (player.Height / 2);
                    break;
                case Directions.top_right:
                    pt.X = player.BottomRightX - ActionnedBomb.WIDTH;
                    break;
                case Directions.top:
                    pt.X = player.X + (player.Width / 2);
                    break;
                case Directions.left:
                    pt.Y = player.Y + (player.Height / 2);
                    break;
            }
            return pt;
        }

        /// <summary>
        /// Checks if an item can be used in the context.
        /// </summary>
        /// <param name="itemId"><see cref="ItemIdEnum"/></param>
        /// <param name="engine"><see cref="AbstractEngine"/></param>
        /// <returns><c>True</c> if it can be used; <c>False</c> otherwise.</returns>
        private bool ItemCanBeUseInContext(ItemIdEnum itemId, AbstractEngine engine)
        {
            switch (itemId)
            {
                case ItemIdEnum.Bomb:
                    // example : not underwater
                    break;
                case ItemIdEnum.SmallLifePotion:
                case ItemIdEnum.MediumLifePotion:
                case ItemIdEnum.LargeLifePotion:
                    if (engine.Player.CurrentLifePoints == engine.Player.MaximalLifePoints)
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
