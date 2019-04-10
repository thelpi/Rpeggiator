using System.Collections.Generic;
using System.Linq;
using RpeggiatorLib.Enums;

namespace RpeggiatorLib
{
    /// <summary>
    /// Represents an item.
    /// </summary>
    public class Item
    {
        // Static list of every instancied items.
        private static List<Item> _items = null;

        /// <summary>
        /// <see cref="ItemType"/>
        /// </summary>
        public ItemType Type { get; private set; }
        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Indicates if the item is unique.
        /// </summary>
        public bool Unique { get; private set; }
        /// <summary>
        /// Delay between two uses, in milliseconds.
        /// </summary>
        public double UseDelay { get; private set; }
        /// <summary>
        /// Maximal quantity initially carriable in the <see cref="Inventory"/>.
        /// </summary>
        public int InitialMaximalQuantity { get; private set; }
        /// <summary>
        /// If this item is ammunitions for another item, indicates this item.
        /// </summary>
        public ItemType? AmmoFor { get; private set; }

        // Private constructor. Only the method "BuildItemList" can create items.
        private Item() { }

        /// <summary>
        /// Gets an <see cref="Item"/> by its <see cref="ItemType"/>.
        /// </summary>
        /// <remarks>Calls <see cref="BuildItemList"/> if never called before.</remarks>
        /// <param name="itemId"><see cref="ItemType"/></param>
        /// <returns><see cref="Item"/></returns>
        internal static Item GetItem(ItemType itemId)
        {
            if (_items == null)
            {
                BuildItemList();
            }

            return _items.Find(item => item.Type == itemId);
        }

        /// <summary>
        /// Gets the single <see cref="Item"/> which is used as ammo by the specified <paramref name="itemType"/>
        /// </summary>
        /// <param name="itemType"><see cref="ItemType"/></param>
        /// <returns>The <see cref="Item"/> or <c>Null</c>.</returns>
        internal static Item GetAmmoItem(ItemType itemType)
        {
            if (_items == null)
            {
                BuildItemList();
            }

            return _items.SingleOrDefault(it => it.AmmoFor == itemType);
        }

        // Creates an instance of every items.
        private static void BuildItemList()
        {
            _items = new List<Item>();
            _items.Add(new Item
            {
                Type = ItemType.Bomb,
                Name = Names.ItemBomb,
                Unique = false,
                InitialMaximalQuantity = 20
            });
            _items.Add(new Item
            {
                Type = ItemType.SmallLifePotion,
                Name = Names.ItemLifePotionSmall,
                Unique = false,
                InitialMaximalQuantity = 12
            });
            _items.Add(new Item
            {
                Type = ItemType.MediumLifePotion,
                Name = Names.ItemLifePotionMedium,
                Unique = false,
                InitialMaximalQuantity = 6
            });
            _items.Add(new Item
            {
                Type = ItemType.LargeLifePotion,
                Name = Names.ItemLifePotionLarge,
                Unique = false,
                InitialMaximalQuantity = 3
            });
            _items.Add(new Item
            {
                Type = ItemType.Lamp,
                Name = Names.ItemLamp,
                Unique = true
            });
            _items.Add(new Item
            {
                Type = ItemType.Bow,
                Name = Names.ItemBow,
                Unique = true
            });
            _items.Add(new Item
            {
                Type = ItemType.Arrow,
                Name = Names.ItemArrow,
                Unique = false,
                InitialMaximalQuantity = 20,
                AmmoFor = ItemType.Bow
            });
            // Sets the use delay for every items of the list.
            _items.ForEach(item => item.UseDelay = Constants.Item.DELAY_BETWEEN_USE[item.Type]);
        }
    }
}
