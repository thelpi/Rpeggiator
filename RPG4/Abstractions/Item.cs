using System.Collections.Generic;

namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents an item.
    /// </summary>
    public class Item
    {
        #region Common item identifiers

        /// <summary>
        /// Sword item identifier.
        /// </summary>
        public const int SWORD_ID = 2;
        /// <summary>
        /// Small life potion identifier.
        /// </summary>
        public const int SMALL_LIFE_POTION_ID = 3;
        /// <summary>
        /// Bomb item identifier.
        /// </summary>
        public const int BOMB_ID = 1;

        #endregion

        // list of every items
        private static List<Item> _items = null;

        /// <summary>
        /// Unique identifier.
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Indicates if the item is unique.
        /// </summary>
        public bool Unique { get; private set; }
        /// <summary>
        /// Indicates if the item can be stored without limit.
        /// </summary>
        public bool Unlimited { get; private set; }
        /*/// <summary>
        /// Indicates if the item's lifetime, in ticks.
        /// </summary>
        /// <remarks>-1 equals no lifetime.</remarks>
        public int Lifetime { get; private set; }*/

        // private constructor
        private Item() { }

        /// <summary>
        /// Gets an <see cref="Item"/> by its identifier.
        /// </summary>
        /// <param name="itemId">Item identifier.</param>
        /// <returns><see cref="Item"/></returns>
        public static Item GetItem(int itemId)
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
                Id = BOMB_ID,
                //Lifetime = 0,
                Name = "Bomb",
                Unique = false,
                Unlimited = false
            });
            _items.Add(new Item
            {
                Id = SWORD_ID,
                //Lifetime = 0,
                Name = "Sword",
                Unique = true,
                Unlimited = false
            });
            _items.Add(new Item
            {
                Id = 3,
                //Lifetime = 0,
                Name = "Life potion (small)",
                Unique = false,
                Unlimited = false
            });
            _items.Add(new Item
            {
                Id = SMALL_LIFE_POTION_ID,
                //Lifetime = 0,
                Name = "Life potion (medium)",
                Unique = false,
                Unlimited = false
            });
            _items.Add(new Item
            {
                Id = 5,
                //Lifetime = 0,
                Name = "Life potion (large)",
                Unique = false,
                Unlimited = false
            });
        }
    }
}
