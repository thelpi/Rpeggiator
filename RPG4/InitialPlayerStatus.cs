﻿using RPG4.Abstraction;
using RPG4.Abstraction.Graphic;

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
        public const double INITIAL_PLAYER_X = 300;
        /// <summary>
        /// Player initial position on Y-axis.
        /// </summary>
        public const double INITIAL_PLAYER_Y = 220;
        /// <summary>
        /// Intial player's speed, in pixels by frame.
        /// </summary>
        public static readonly int INITIAL_PLAYER_SPEED = Constants.FPS / 2;
        /// <summary>
        /// Size of the player on X-axis.
        /// </summary>
        public const int SPRITE_SIZE_X = 40;
        /// <summary>
        /// Size of the player on Y-axis.
        /// </summary>
        public const int SPRITE_SIZE_Y = 40;
        /// <summary>
        /// Frames count before ability to hit again.
        /// </summary>
        public static readonly int HIT_FRAME_MAX_COUNT = Constants.FPS / 10;
        /// <summary>
        /// Hit life points cost on enemies.
        /// </summary>
        public const double HIT_LIFE_POINT_COST = 1;
        /// <summary>
        /// Initial list of items in the inventory.
        /// </summary>
        public static readonly InventoryItem[] INVENTORY_ITEMS = new InventoryItem[]
        {
            new InventoryItem(ItemIdEnum.SmallLifePotion, 3, Inventory.MAX_QUANTITY_BY_ITEM[ItemIdEnum.SmallLifePotion]),
            new InventoryItem(ItemIdEnum.Bomb, 10, Inventory.MAX_QUANTITY_BY_ITEM[ItemIdEnum.Bomb]),
        };
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
    }
}
