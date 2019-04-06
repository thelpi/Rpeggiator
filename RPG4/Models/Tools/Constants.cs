using RPG4.Models.Enums;
using RPG4.Models.Graphic;
using RPG4.Models.Sprites;
using System;
using System.Collections.Generic;

namespace RPG4.Models
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
        /// Size ratio, compared to <see cref="Engine.Player"/>, which triggers pursue by enemies.
        /// </summary>
        public const double PLAYER_SIZE_RATIO_TO_TRIGGER_ENEMY = 3;
        /// <summary>
        /// Maximal gap to consider two <see cref="double"/> as equal.
        /// </summary>
        public const double TYPE_DOUBLE_COMPARISON_TOLERANCE = 0.00001;
        /// <summary>
        /// Overlap ratio to change from one <see cref="Floor"/> to another.
        /// </summary>
        public const double FLOOR_CHANGE_OVERLAP_RATIO = 0.5;
        /// <summary>
        /// Indicates the life points cost when a bomb explodes nearby a <see cref="Rift"/>.
        /// </summary>
        public const double RIFT_EXPLOSION_LIFE_POINT_COST = 5;
        /// <summary>
        /// Indicates the ratio of overlaping to fall into a <see cref="Pit"/>.
        /// </summary>
        public const double PIT_FALL_IN_OVERLAP_RATIO = 0.5;
        /// <summary>
        /// Speed ratio when walking on <see cref="FloorType.Water"/>.
        /// </summary>
        public const double FLOOR_WATER_SPEED_RATIO = 0.5;
        /// <summary>
        /// Speed ratio when walking on <see cref="FloorType.Ice"/>.
        /// </summary>
        public const double FLOOR_ICE_SPEED_RATIO = 1.2;

        /// <summary>
        /// Set of constants to indicate player initial status (position, speed, and so forth).
        /// </summary>
        public static class Player
        {
            /// <summary>
            /// Player initial position on X-axis.
            /// </summary>
            public const double INITIAL_PLAYER_X = 400;
            /// <summary>
            /// Player initial position on Y-axis.
            /// </summary>
            public const double INITIAL_PLAYER_Y = 300;
            /// <summary>
            /// Intial player's speed, in pixels by second.
            /// </summary>
            public static readonly double INITIAL_PLAYER_SPEED = 200;
            /// <summary>
            /// Size of the player on X-axis.
            /// </summary>
            public const int SPRITE_SIZE_X = 40;
            /// <summary>
            /// Size of the player on Y-axis.
            /// </summary>
            public const int SPRITE_SIZE_Y = 40;
            /// <summary>
            /// Delay between two hits with the sword, in milliseconds.
            /// </summary>
            public static readonly double SWORD_HIT_DELAY = 500;
            /// <summary>
            /// Hit life points cost on enemies.
            /// </summary>
            public const double HIT_LIFE_POINT_COST = 1;
            /// <summary>
            /// Initial list of items in the inventory.
            /// </summary>
            public static readonly IReadOnlyDictionary<Enums.ItemType, int> INVENTORY_ITEMS = new Dictionary<Enums.ItemType, int>
            {
                { Enums.ItemType.Lamp, 1 },
                { Enums.ItemType.SmallLifePotion, 3 },
                { Enums.ItemType.MediumLifePotion, 1 },
                { Enums.ItemType.LargeLifePotion, 1 },
                { Enums.ItemType.Bomb, 1 }
            };
            /// <summary>
            /// Initial coins.
            /// </summary>
            public static int COINS = 10;
            /// <summary>
            /// Initial life points.
            /// </summary>
            public const double MAXIMAL_LIFE_POINTS = 10;
            /// <summary>
            /// Hit graphic rendering.
            /// </summary>
            public static readonly ISpriteGraphic HIT_GRAPHIC = new ImageBrushGraphic("Sword");
            /// <summary>
            /// Graphic rendering.
            /// </summary>
            public static readonly ISpriteGraphic GRAPHIC = new ImageBrushGraphic("Player");
            /// <summary>
            /// Recovery graphic rendering.
            /// </summary>
            public static readonly ISpriteGraphic RECOVERY_GRAPHIC = new ImageBrushGraphic("PlayerRecovery");
            /// <summary>
            /// Player recovery time span, in milliseconds.
            /// </summary>
            public const double RECOVERY_TIME = 1000;
            /// <summary>
            /// Range of action effet (compared to player size).
            /// </summary>
            public const double ACTION_RANGE = 1.2;
        }

        /// <summary>
        /// Set of constants relatives to <see cref="Models.Inventory"/>.
        /// </summary>
        public static class Inventory
        {
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
            public const int SIZE = 10;
            /// <summary>
            /// Coins limit.
            /// </summary>
            public const int COINS_LIMIT = 999;
        }

        /// <summary>
        /// Set of constants relatives to <see cref="Models.Item"/>.
        /// </summary>
        public static class Item
        {
            /// <summary>
            /// Item loot width.
            /// </summary>
            public const double LOOT_WIDTH = 20;
            /// <summary>
            /// Item loot height.
            /// </summary>
            public const double LOOT_HEIGHT = 20;
            /// <summary>
            /// Item loot lifetime, in milliseconds.
            /// </summary>
            public const double LOOT_LIFETIME = 10000;
            /// <summary>
            /// Coin graphic.
            /// </summary>
            public static readonly ISpriteGraphic COIN_GRAPHIC = new ImageBrushGraphic("Coin");
        }

        /// <summary>
        /// Set of constants relatives to <see cref="ActionnedBomb"/>.
        /// </summary>
        public static class Bomb
        {
            /// <summary>
            /// Width.
            /// </summary>
            public const double WIDTH = 20;
            /// <summary>
            /// Height.
            /// </summary>
            public const double HEIGHT = 20;
            /// <summary>
            /// Milliseconds while pending explosion.
            /// </summary>
            public static readonly double TIME_WHILE_PENDING = 2000;
            /// <summary>
            /// Milliseconds while exploding.
            /// </summary>
            public static readonly double TIME_WHILE_EXPLODING = 500;
            /// <summary>
            /// <see cref="ActionnedBomb"/> graphic rendering.
            /// </summary>
            public static readonly ISpriteGraphic GRAPHIC_RENDERING = new ImageBrushGraphic("Bomb");
            /// <summary>
            /// Indicates the life points cost when a bomb explodes nearby.
            /// </summary>
            public const double EXPLOSION_LIFE_POINT_COST = 3;
            /// <summary>
            /// Indicates the ratio size of a <see cref="ActionnedBomb"/> explosion (compared to the bomb itself).
            /// </summary>
            public const double EXPLOSION_SIZE_RATIO = 3;
            /// <summary>
            /// <see cref="ActionnedBomb"/> explosion graphic rendering.
            /// </summary>
            public static readonly ISpriteGraphic EXPLOSION_GRAPHIC_RENDERING = new PlainBrushGraphic("#FFFF4500");
        }
    }
}
