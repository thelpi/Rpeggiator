﻿namespace RPG4
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
        /// Intial player's speed, in pixels by tick.
        /// </summary>
        public const int INITIAL_PLAYER_SPEED = 10;
        /// <summary>
        /// Size of the player on X-axis.
        /// </summary>
        public const int SPRITE_SIZE_X = 40;
        /// <summary>
        /// Size of the player on Y-axis.
        /// </summary>
        public const int SPRITE_SIZE_Y = 40;
        /// <summary>
        /// When kicking, indicates the life-points cost on the enemy.
        /// </summary>
        public const double INITIAL_HIT_SIZE_RATIO = 2;
        /// <summary>
        /// Tick count before the effect of a hit ends.
        /// </summary>
        public const int HIT_TICK_MAX_COUNT = 2;
        /// <summary>
        /// When kicking, indicates the life-points cost on the enemy.
        /// </summary>
        public const int HIT_LIFE_POINT_COST = 1;
    }
}
