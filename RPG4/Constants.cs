using System;

namespace RPG4
{
    /// <summary>
    /// Set of constants
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Minimal delay between two frames, in milliseconds.
        /// </summary>
        public const double MIN_DELAY_BETWEEN_FRAMES = 10;
        /// <summary>
        /// Player's moves history max count.
        /// </summary>
        public const int MOVE_HISTORY_COUNT = 50;
        /// <summary>
        /// Player recovery time span, in milliseconds.
        /// </summary>
        public const double PLAYER_RECOVERY_TIME = 1000;
        /// <summary>
        /// First screen index.
        /// </summary>
        public const int FIRST_SCREEN_INDEX = 1;

        /// <summary>
        /// Recovered life points by drinking small life potion.
        /// </summary>
        public const double SMALL_LIFE_POTION_RECOVERY_LIFE_POINTS = 2;
        /// <summary>
        /// Recovered life points by drinking medium life potion.
        /// </summary>
        public const double MEDIUM_LIFE_POTION_RECOVERY_LIFE_POINTS = 5;
        /// <summary>
        /// Recovered life points by drinking large life potion.
        /// </summary>
        public const double LARGE_LIFE_POTION_RECOVERY_LIFE_POINTS = 10;
        /// <summary>
        /// Initial size of the inventory.
        /// </summary>
        public const int INVENTORY_SIZE = 10;
    }
}
