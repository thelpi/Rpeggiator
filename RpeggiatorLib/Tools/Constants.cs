using System.Collections.Generic;
using RpeggiatorLib.Enums;

namespace RpeggiatorLib
{
    /// <summary>
    /// Set of constants
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// Delay between two images while recovering.
        /// </summary>
        internal const double RECOVERY_BLINK_DELAY = 100;
        /// <summary>
        /// <see cref="Sprites.Sprite"/> default direction.
        /// </summary>
        internal const Direction DEFAULT_SPRITE_DIRECTION = Direction.Right;
        /// <summary>
        /// <see cref="Sprites.Screen"/> width.
        /// </summary>
        internal const double SCREEN_WIDTH = 800;
        /// <summary>
        /// <see cref="Sprites.Screen"/> height.
        /// </summary>
        internal const double SCREEN_HEIGHT = 600;
        /// <summary>
        /// Player's moves history max count.
        /// </summary>
        internal const int MOVE_HISTORY_COUNT = 50;
        /// <summary>
        /// First screen identifier.
        /// </summary>
        internal const int FIRST_SCREEN_ID = 1;
        /// <summary>
        /// Ratio of time acceleration betwean real world and game.
        /// </summary>
        internal const double TIME_RATIO = 240;
        /// <summary>
        /// Night darkness opacity ratio.
        /// </summary>
        internal const double NIGHT_DARKNESS_OPACITY = 0.7;
        /// <summary>
        /// Night peak of darkness hour beginning.
        /// </summary>
        internal const int NIGHT_PEAK_HOUR_BEGIN = 23;
        /// <summary>
        /// Night peak of darkness hour ending.
        /// </summary>
        internal const int NIGHT_PEAK_HOUR_END = 5;
        /// <summary>
        /// Dawn ending hour.
        /// </summary>
        internal const int NIGHT_DAWN_HOUR = 8;
        /// <summary>
        /// Dusk beginning hour.
        /// </summary>
        internal const int NIGHT_DUSK_HOUR = 20;
        /// <summary>
        /// First in-game day hour start.
        /// </summary>
        internal const int FIRST_DAY_HOUR_START = 6;
        /// <summary>
        /// Size ratio, compared to <see cref="Engine.Player"/>, which triggers pursue by enemies.
        /// </summary>
        internal const double PLAYER_SIZE_RATIO_TO_TRIGGER_ENEMY = 3;
        /// <summary>
        /// Maximal gap to consider two <see cref="double"/> as equal.
        /// </summary>
        internal const double TYPE_DOUBLE_COMPARISON_TOLERANCE = 0.00001;
        /// <summary>
        /// Overlap ratio to change from one <see cref="Sprites.Floor"/> to another.
        /// </summary>
        internal const double FLOOR_CHANGE_OVERLAP_RATIO = 0.5;
        /// <summary>
        /// Indicates the life points cost when a bomb explodes nearby a <see cref="Sprites.Rift"/>.
        /// </summary>
        internal const double RIFT_EXPLOSION_LIFE_POINT_COST = 5;
        /// <summary>
        /// Indicates the ratio of overlaping to fall into a <see cref="Sprites.Pit"/>.
        /// </summary>
        internal const double PIT_FALL_IN_OVERLAP_RATIO = 0.5;
        /// <summary>
        /// Speed ratio by <see cref="FloorType"/>
        /// </summary>
        internal static readonly Dictionary<FloorType, double> FLOOR_SPEED_RATIO = new Dictionary<FloorType, double>
        {
            { FloorType.Ground, 1 },
            { FloorType.Ice, 1.2 },
            { FloorType.Lava, 0 },
            { FloorType.Water, 0.5 }
        };

        /// <summary>
        /// Set of constants to indicate player initial status (position, speed, and so forth).
        /// </summary>
        internal static class Player
        {
            /// <summary>
            /// Initial <see cref="Sprites.Sprite.X"/>.
            /// </summary>
            internal const double INITIAL_X = 400;
            /// <summary>
            /// Initial <see cref="Sprites.Sprite.Y"/>.
            /// </summary>
            internal const double INITIAL_Y = 300;
            /// <summary>
            /// <see cref="Sprites.Sprite.Width"/>
            /// </summary>
            internal const int SPRITE_WIDTH = 40;
            /// <summary>
            /// <see cref="Sprites.Sprite.Height"/>
            /// </summary>
            internal const int SPRITE_HEIGHT = 40;
            /// <summary>
            /// Intial <see cref="Sprites.LifeSprite.Speed"/>.
            /// </summary>
            internal const double INITIAL_SPEED = 200;
            /// <summary>
            /// Ratio of the hit sprite compared to <see cref="Player"/>.
            /// </summary>
            internal const double HIT_SPRITE_RATIO = 1 / (double)3;
            /// <summary>
            /// Delay between two hits with the sword, in milliseconds.
            /// </summary>
            internal const double SWORD_HIT_DELAY = 250;
            /// <summary>
            /// Hit life points cost on enemies.
            /// </summary>
            internal const double HIT_LIFE_POINT_COST = 1;
            /// <summary>
            /// Initial list of items in the inventory.
            /// </summary>
            internal static readonly IReadOnlyDictionary<ItemType, int> INVENTORY_ITEMS = new Dictionary<ItemType, int>
            {
                { ItemType.Lamp, 1 },
                { ItemType.LifePotionSmall, 3 },
                { ItemType.LifePotionMedium, 1 },
                { ItemType.LifePotionLarge, 1 },
                { ItemType.Bomb, 1 },
                { ItemType.Bow, 1 },
                { ItemType.Arrow, 10 }
            };
            /// <summary>
            /// Initial coins.
            /// </summary>
            internal static int COINS = 10;
            /// <summary>
            /// Initial life points.
            /// </summary>
            internal const double MAXIMAL_LIFE_POINTS = 10;
            /// <summary>
            /// Player recovery time span, in milliseconds.
            /// </summary>
            internal const double RECOVERY_TIME = 1000;
            /// <summary>
            /// Range of action effet (compared to player size).
            /// </summary>
            internal const double ACTION_RANGE = 1.2;
        }

        /// <summary>
        /// Set of constants relatives to <see cref="Inventory"/>.
        /// </summary>
        internal static class Inventory
        {
            /// <summary>
            /// Recovered life points by drinking small life potion.
            /// </summary>
            internal const double SMALL_LIFE_POTION_RECOVERY_LIFE_POINTS = 2;
            /// <summary>
            /// Recovered life points by drinking medium life potion.
            /// </summary>
            internal const double MEDIUM_LIFE_POTION_RECOVERY_LIFE_POINTS = 5;
            /// <summary>
            /// Recovered life points by drinking large life potion.
            /// </summary>
            internal const double LARGE_LIFE_POTION_RECOVERY_LIFE_POINTS = 10;
            /// <summary>
            /// Initial size of the inventory.
            /// </summary>
            internal const int SIZE = 10;
            /// <summary>
            /// Coins limit.
            /// </summary>
            internal const int COINS_LIMIT = 999;
        }

        /// <summary>
        /// Set of constants relatives to <see cref="Item"/>.
        /// </summary>
        internal static class Item
        {
            /// <summary>
            /// Item loot width.
            /// </summary>
            internal const double LOOT_WIDTH = 20;
            /// <summary>
            /// Item loot height.
            /// </summary>
            internal const double LOOT_HEIGHT = 20;
            /// <summary>
            /// Item loot lifetime, in milliseconds.
            /// </summary>
            internal const double LOOT_LIFETIME = 10000;
            /// <summary>
            /// Delay between two uses of a specific item, in milliseconds.
            /// </summary>
            internal static readonly IReadOnlyDictionary<ItemType, int> DELAY_BETWEEN_USE = new Dictionary<ItemType, int>
            {
                { ItemType.Bomb, 500 },
                { ItemType.LifePotionLarge, 500 },
                { ItemType.LifePotionMedium, 500 },
                { ItemType.LifePotionSmall, 500 },
                { ItemType.Lamp, 0 },
                { ItemType.Bow, 1000 },
                { ItemType.Arrow, 0 }
            };
        }

        /// <summary>
        /// Set of constants relatives to <see cref="Sprites.ActionnedBomb"/>.
        /// </summary>
        internal static class Bomb
        {
            /// <summary>
            /// Width.
            /// </summary>
            internal const double WIDTH = 20;
            /// <summary>
            /// Height.
            /// </summary>
            internal const double HEIGHT = 20;
            /// <summary>
            /// Milliseconds while pending explosion.
            /// </summary>
            internal const double TIME_WHILE_PENDING = 2000;
            /// <summary>
            /// Milliseconds while exploding.
            /// </summary>
            internal const double TIME_WHILE_EXPLODING = 500;
            /// <summary>
            /// Indicates the life points cost when a bomb explodes nearby.
            /// </summary>
            internal const double EXPLOSION_LIFE_POINT_COST = 3;
            /// <summary>
            /// Indicates the ratio size of a <see cref="Sprites.ActionnedBomb"/> explosion (compared to the bomb itself).
            /// </summary>
            internal const double EXPLOSION_SIZE_RATIO = 3;
        }

        /// <summary>
        /// Set of constants relatives to <see cref="Sprites.ActionnedArrow"/>.
        /// </summary>
        internal static class Arrow
        {
            /// <summary>
            /// Width.
            /// </summary>
            internal const double WIDTH = 20;
            /// <summary>
            /// Height.
            /// </summary>
            internal const double HEIGHT = 20;
            /// <summary>
            /// Speed, in pixels by second.
            /// </summary>
            internal const double SPEED = 250;
            /// <summary>
            /// Life points cost.
            /// </summary>
            internal const double LIFE_POINT_COST = 3;
        }
    }
}
