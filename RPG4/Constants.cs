using System;

namespace RPG4
{
    /// <summary>
    /// Set of constants
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Frames per second.
        /// </summary>
        public const int FPS = 20;
        /// <summary>
        /// Player's moves history max count.
        /// </summary>
        public const int MOVE_HISTORY_COUNT = 50;
        /// <summary>
        /// Frames count while recovering from a hit.
        /// </summary>
        public static readonly int RECOVERY_FRAME_COUNT = (int)(FPS * 0.5);
        /// <summary>
        /// Substitution parameters for <see cref="FPS"/> in formulas.
        /// </summary>
        public static readonly Tuple<string, object> SUBSTITUTE_FORMULA_FPS = new Tuple<string, object>("{FPS}", FPS);
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
