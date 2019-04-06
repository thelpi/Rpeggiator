using RPG4.Models.Enums;
using RPG4.Models.Graphic;
using RPG4.Properties;
using System.Collections.Generic;

namespace RPG4.Models
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
        public ItemType Id { get; private set; }
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
        /// <see cref="ISpriteGraphic"/> as loot.
        /// </summary>
        public ISpriteGraphic LootGraphic { get; private set; }

        // Private constructor. Only the method "BuildItemList" can create items.
        private Item() { }

        /// <summary>
        /// Gets an <see cref="Item"/> by its <see cref="ItemType"/>.
        /// </summary>
        /// <remarks>Calls <see cref="BuildItemList"/> if never called before.</remarks>
        /// <param name="itemId"><see cref="ItemType"/></param>
        /// <returns><see cref="Item"/></returns>
        public static Item GetItem(ItemType itemId)
        {
            if (_items == null)
            {
                BuildItemList();
            }

            return _items.Find(item => item.Id == itemId);
        }

        // Creates an instance of every items.
        private static void BuildItemList()
        {
            _items = new List<Item>();
            _items.Add(new Item
            {
                Id = ItemType.Bomb,
                Name = Names.ItemBomb,
                Unique = false,
                InitialMaximalQuantity = 20,
                LootGraphic = new ImageBrushGraphic(nameof(Resources.Bomb))
            });
            _items.Add(new Item
            {
                Id = ItemType.SmallLifePotion,
                Name = Names.ItemLifePotionSmall,
                Unique = false,
                InitialMaximalQuantity = 12,
                LootGraphic = new ImageBrushGraphic(nameof(Resources.LifePotionSmall))
            });
            _items.Add(new Item
            {
                Id = ItemType.MediumLifePotion,
                Name = Names.ItemLifePotionMedium,
                Unique = false,
                InitialMaximalQuantity = 6,
                LootGraphic = new ImageBrushGraphic(nameof(Resources.LifePotionMedium))
            });
            _items.Add(new Item
            {
                Id = ItemType.LargeLifePotion,
                Name = Names.ItemLifePotionLarge,
                Unique = false,
                InitialMaximalQuantity = 3,
                LootGraphic = new ImageBrushGraphic(nameof(Resources.LifePotionLarge))
            });
            _items.Add(new Item
            {
                Id = ItemType.Lamp,
                Name = Names.ItemLamp,
                Unique = true,
                LootGraphic = new ImageBrushGraphic(nameof(Resources.Lamp))
            });
            _items.Add(new Item
            {
                Id = ItemType.Bow,
                Name = Names.ItemBow,
                Unique = true,
                LootGraphic = new ImageBrushGraphic(nameof(Resources.Bow))
            });
            _items.Add(new Item
            {
                Id = ItemType.Arrow,
                Name = Names.ItemArrow,
                Unique = false,
                InitialMaximalQuantity = 20,
                LootGraphic = new ImageBrushGraphic(nameof(Resources.Arrow))
            });
            // Sets the use delay for every items of the list.
            _items.ForEach(item => item.UseDelay = Constants.Item.DELAY_BETWEEN_USE[item.Id]);
        }
    }
}
