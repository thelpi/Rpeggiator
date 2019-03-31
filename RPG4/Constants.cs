using System;

namespace RPG4
{
    /// <summary>
    /// Set of constants
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// <see cref="DateTime.ToString(string)"/> pattern.
        /// </summary>
        public const string UNIQUE_TIMESTAMP_PATTERN = "fffffff";
        /// <summary>
        /// Minimal delay between two frames, in milliseconds.
        /// </summary>
        public const double MIN_DELAY_BETWEEN_FRAMES = 10;
        /// <summary>
        /// Player's moves history max count.
        /// </summary>
        public const int MOVE_HISTORY_COUNT = 50;
        /// <summary>
        /// First screen index.
        /// </summary>
        public const int FIRST_SCREEN_INDEX = 1;
        /// <summary>
        /// Ratio of time acceleration betwean real world and game.
        /// </summary>
        public const double TIME_RATIO = 120;
    }
}
