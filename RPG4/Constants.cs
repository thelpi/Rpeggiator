namespace RPG4
{
    public static class Constants
    {
        public const double AREA_WIDTH = 640;
        public const double AREA_HEIGHT = 480;
        public const int REFRESH_DELAY_MS = 50;
        public const int PLAYER_SPEED = 100;
        public const int SPRITE_SIZE_X = 40;
        public const int SPRITE_SIZE_Y = 40;
        public const double KICK_SIZE_RATIO = 2;
        public const int KICK_TICK_MAX_COUNT = 2;
        public const int MOVE_HISTORY_COUNT = 50;
        public static readonly double KICK_THICK_X = ((KICK_SIZE_RATIO - 1) / 2) * SPRITE_SIZE_X;
        public static readonly double KICK_THICK_Y = ((KICK_SIZE_RATIO - 1) / 2) * SPRITE_SIZE_Y;
        public const int HIT_LIFE_POINT_COST = 1;
    }
}
