namespace RpeggiatorLib.Enums
{
    /// <summary>
    /// Enumeration of every possible uses of <see cref="Elapser"/>
    /// </summary>
    /// <remarks>An use is unique in the collection <see cref="Elapser.Instances"/>.</remarks>
    internal enum ElapserUse
    {
        /// <summary>
        /// <see cref="Sprites.ActionnedArrow"/> movement.
        /// </summary>
        ArrowMovement = 1,
        /// <summary>
        /// <see cref="Sprites.FloorTrigger"/> management.
        /// </summary>
        FloorTriggerManagement,
        /// <summary>
        /// <see cref="Sprites.ActionnedBomb"/> pending.
        /// </summary>
        BonbPending,
        /// <summary>
        /// <see cref="Sprites.ActionnedBomb"/> exploding.
        /// </summary>
        BonbExploding,
        /// <summary>
        /// <see cref="Sprites.Enemy"/> movement.
        /// </summary>
        EnemyMovement,
        /// <summary>
        /// <see cref="InventoryItem"/> use management.
        /// </summary>
        InventoryUseManagement,
        /// <summary>
        /// <see cref="Sprites.Player"/> movement.
        /// </summary>
        PlayerMovement,
        /// <summary>
        /// <see cref="Sprites.LifeSprite"/> recovery.
        /// </summary>
        LifeSpriteRecovery,
        /// <summary>
        /// <see cref="Sprites.PickableItem"/> lifetime.
        /// </summary>
        PickableItemLifetime,
        /// <summary>
        /// <see cref="Sprites.Player"/> sword management.
        /// </summary>
        PlayerSwordManagement
    }
}
