using RPG4.Models.Graphic;
using System.Collections.Generic;

namespace RPG4.Models
{
    /// <summary>
    /// Represents an item.
    /// </summary>
    public class Item
    {
        /// <summary>
        /// Item loot width.
        /// </summary>
        public const double LOOT_WIDTH = 20;
        /// <summary>
        /// Item loot height.
        /// </summary>
        public const double LOOT_HEIGHT = 20;
        /// <summary>
        /// Item loot lifetime, in milliseconds.
        /// </summary>
        public const double LOOT_LIFETIME = 10000;
        /// <summary>
        /// Coin graphic.
        /// </summary>
        public static readonly ISpriteGraphic COIN_GRAPHIC = new ImageBrushGraphic("Coin");

        // list of every items
        private static List<Item> _items = null;

        /// <summary>
        /// <see cref="ItemEnum"/>
        /// </summary>
        public ItemEnum Id { get; private set; }
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
        /// Maximal quantity carriable at eh beginning.
        /// </summary>
        public int InitialMaximalQuantity { get; private set; }
        /// <summary>
        /// Loot graphic.
        /// </summary>
        public ISpriteGraphic LootGraphic { get; private set; }

        // private constructor
        private Item() { }

        /// <summary>
        /// Gets an <see cref="Item"/> by its <see cref="ItemEnum"/>.
        /// </summary>
        /// <param name="itemId"><see cref="ItemEnum"/></param>
        /// <returns><see cref="Item"/></returns>
        public static Item GetItem(ItemEnum itemId)
        {
            if (_items == null)
            {
                BuildItemList();
            }

            return _items.Find(item => item.Id == itemId);
        }

        // creates an instance of every items
        private static void BuildItemList()
        {
            _items = new List<Item>();
            _items.Add(new Item
            {
                Id = ItemEnum.Bomb,
                Name = "Bomb",
                Unique = false,
                UseDelay = 1000,
                InitialMaximalQuantity = 20,
                LootGraphic = new ImageBrushGraphic("Bomb")
            });
            _items.Add(new Item
            {
                Id = ItemEnum.SmallLifePotion,
                Name = "Life potion (small)",
                Unique = false,
                UseDelay = 500,
                InitialMaximalQuantity = 12,
                LootGraphic = new ImageBrushGraphic("LifePotionSmall")
            });
            _items.Add(new Item
            {
                Id = ItemEnum.MediumLifePotion,
                Name = "Life potion (medium)",
                Unique = false,
                UseDelay = 500,
                InitialMaximalQuantity = 6,
                LootGraphic = new ImageBrushGraphic("LifePotionMedium")
            });
            _items.Add(new Item
            {
                Id = ItemEnum.LargeLifePotion,
                Name = "Life potion (large)",
                Unique = false,
                UseDelay = 500,
                InitialMaximalQuantity = 3,
                LootGraphic = new ImageBrushGraphic("LifePotionLarge")
            });
            _items.Add(new Item
            {
                Id = ItemEnum.Lamp,
                Name = "Lamp",
                Unique = true,
                UseDelay = 0,
                LootGraphic = new ImageBrushGraphic("Lamp")
            });
        }
    }
}
