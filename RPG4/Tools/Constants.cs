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
        public const double TIME_RATIO = 240;
        /// <summary>
        /// Night darkness opacity ratio.
        /// </summary>
        public const double NIGHT_DARKNESS_OPACITY = 0.7;
        /// <summary>
        /// Night peak of darkness hour beginning.
        /// </summary>
        public const int NIGHT_PEAK_HOUR_BEGIN = 23;
        /// <summary>
        /// Night peak of darkness hour ending.
        /// </summary>
        public const int NIGHT_PEAK_HOUR_END = 5;
        /// <summary>
        /// Dawn ending hour.
        /// </summary>
        public const int NIGHT_DAWN_HOUR = 8;
        /// <summary>
        /// Dusk beginning hour.
        /// </summary>
        public const int NIGHT_DUSK_HOUR = 20;
        /// <summary>
        /// Size ratio, compared to <see cref="Models.Engine.Player"/>, which triggers pursue by enemies.
        /// </summary>
        public const double PLAYER_SIZE_RATIO_TO_TRIGGER_ENEMY = 3;
        /// <summary>
        /// Maximal gap to consider two <see cref="double"/> as equal.
        /// </summary>
        public const double TYPE_DOUBLE_COMPARISON_TOLERANCE = 0.00001;
    }
}
