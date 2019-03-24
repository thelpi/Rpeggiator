using RPG4.Abstractions;

namespace RPG4
{
    /// <summary>
    /// Set of constants to indicate player initial status (position, speed, and so forth).
    /// </summary>
    public static class InitialPlayerStatus
    {
        /// <summary>
        /// Player initial position on X-axis.
        /// </summary>
        public const double INITIAL_PLAYER_X = 300;
        /// <summary>
        /// Player initial position on Y-axis.
        /// </summary>
        public const double INITIAL_PLAYER_Y = 220;
        /// <summary>
        /// Intial player's speed, in pixels by frame.
        /// </summary>
        public static readonly int INITIAL_PLAYER_SPEED = Constants.FPS / 2;
        /// <summary>
        /// Size of the player on X-axis.
        /// </summary>
        public const int SPRITE_SIZE_X = 40;
        /// <summary>
        /// Size of the player on Y-axis.
        /// </summary>
        public const int SPRITE_SIZE_Y = 40;
        /// <summary>
        /// Frames count before ability to hit again.
        /// </summary>
        public static readonly int HIT_FRAME_MAX_COUNT = Constants.FPS / 10;
        /// <summary>
        /// Hit life points cost on enemies.
        /// </summary>
        public const double HIT_LIFE_POINT_COST = 1;
        /// <summary>
        /// Initial size of the inventory.
        /// </summary>
        public const int INVENTORY_SIZE = 10;
        /// <summary>
        /// Initial list of items in the inventory.
        /// </summary>
        public static readonly InventoryItem[] INVENTORY_ITEMS = new InventoryItem[]
        {
            new InventoryItem(ItemIdEnum.SmallLifePotion, 3),
            new InventoryItem(ItemIdEnum.Bomb, 10),
        };
        /// <summary>
        /// Initial life points.
        /// </summary>
        public const double MAXIMAL_LIFE_POINTS = 10;
        /// <summary>
        /// Indicates the life points cost when a bomb explodes nearby.
        /// </summary>
        public const double EXPLOSION_LIFE_POINT_COST = 3;
    }
}
