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
        /// When kicking, indicates the ratio size of the halo (compared to the player).
        /// </summary>
        public const double INITIAL_HIT_HALO_SIZE_RATIO = 2;
        /// <summary>
        /// Frames count before the effect of a hit ends.
        /// </summary>
        public static readonly int HIT_FRAME_MAX_COUNT = Constants.FPS / 10;
        /// <summary>
        /// When kicking, indicates the life-points cost on the enemy.
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
            new InventoryItem(ItemIdEnum.SmallLifePotion, 3)
        };
        /// <summary>
        /// Initial life points.
        /// </summary>
        public const double MAXIMAL_LIFE_POINTS = 10;
    }
}
