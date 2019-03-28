using System.Collections.Generic;

namespace RPG4.Abstraction
{
    /// <summary>
    /// Represents an item.
    /// </summary>
    public class Item
    {
        // list of every items
        private static List<Item> _items = null;

        /// <summary>
        /// <see cref="ItemIdEnum"/>
        /// </summary>
        public ItemIdEnum Id { get; private set; }
        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Indicates if the item is unique.
        /// </summary>
        public bool Unique { get; private set; }
        /// <summary>
        /// Frames count delay between two use; -1 for no delay at all.
        /// </summary>
        public int DelayBetweenUse { get; private set; }
        /// <summary>
        /// Maximal quantity carriable at eh beginning.
        /// </summary>
        public int InitialMaximalQuantity { get; private set; }

        // private constructor
        private Item() { }

        /// <summary>
        /// Gets an <see cref="Item"/> by its <see cref="ItemIdEnum"/>.
        /// </summary>
        /// <param name="itemId"><see cref="ItemIdEnum"/></param>
        /// <returns><see cref="Item"/></returns>
        public static Item GetItem(ItemIdEnum itemId)
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
                Id = ItemIdEnum.Bomb,
                Name = "Bomb",
                Unique = false,
                DelayBetweenUse = Constants.FPS,
                InitialMaximalQuantity = 20
            });
            _items.Add(new Item
            {
                Id = ItemIdEnum.SmallLifePotion,
                Name = "Life potion (small)",
                Unique = false,
                DelayBetweenUse = Constants.FPS * 2,
                InitialMaximalQuantity = 12
            });
            _items.Add(new Item
            {
                Id = ItemIdEnum.MediumLifePotion,
                Name = "Life potion (medium)",
                Unique = false,
                DelayBetweenUse = Constants.FPS * 2,
                InitialMaximalQuantity = 6
            });
            _items.Add(new Item
            {
                Id = ItemIdEnum.LargeLifePotion,
                Name = "Life potion (large)",
                Unique = false,
                DelayBetweenUse = Constants.FPS * 2,
                InitialMaximalQuantity = 3
            });
            _items.Add(new Item
            {
                Id = ItemIdEnum.Lamp,
                Name = "Lamp",
                Unique = true,
                DelayBetweenUse = -1
            });
        }
    }
}
