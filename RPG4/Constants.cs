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
        /// Ticks count before a bomb explodes.
        /// </summary>
        public static readonly int BOMB_PENDING_TICK_COUNT = FPS * 2;
        /// <summary>
        /// Ticks count while a bomb explodes.
        /// </summary>
        public static readonly int BOMB_EXPLODING_TICK_COUNT = FPS;
        /// <summary>
        /// When exploding, indicates the ratio size of the halo (compared to the bomb itself).
        /// </summary>
        public const double BOMB_HALO_SIZE_RATIO = 3;
        /// <summary>
        /// Bomb item width.
        /// </summary>
        public const double BOMB_WIDTH = 20;
        /// <summary>
        /// Bomb item height.
        /// </summary>
        public const double BOMB_HEIGHT = 20;
        /// <summary>
        /// Ticks count while recovering from a hit.
        /// </summary>
        public static readonly int RECOVERY_TICK_COUNT = (int)(FPS * 0.5);
        /// <summary>
        /// Substitution parameters for <see cref="FPS"/> in formulas.
        /// </summary>
        public static readonly Tuple<string, object> SUBSTITUTE_FORMULA_FPS = new Tuple<string, object>("{FPS}", FPS);
    }
}
