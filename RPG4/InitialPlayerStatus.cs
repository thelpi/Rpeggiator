using RPG4.Abstraction;
using RPG4.Abstraction.Graphic;
using System.Collections.Generic;

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
        public static readonly IReadOnlyDictionary<ItemIdEnum, int> INVENTORY_ITEMS = new Dictionary<ItemIdEnum, int>
        {
            { ItemIdEnum.Lamp, 1 },
            { ItemIdEnum.SmallLifePotion, 3 },
            { ItemIdEnum.MediumLifePotion, 1 },
            { ItemIdEnum.LargeLifePotion, 1 },
            { ItemIdEnum.Bomb, 1 }
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
        /// Indicates the life points cost when a bomb explodes nearby.
        /// </summary>
        public const double EXPLOSION_LIFE_POINT_COST = 3;
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
}
