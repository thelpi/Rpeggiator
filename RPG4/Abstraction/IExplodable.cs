namespace RPG4.Abstraction
{
    /// <summary>
    /// Ths interface indicates a <see cref="Sprites.Sprite"/> can be damages by <see cref="Sprites.ActionnedBomb"/>.
    /// </summary>
    public interface IExplodable
    {
        /// <summary>
        /// Indicates the life points cost when a bomb explodes nearby.
        /// </summary>
        double ExplosionLifePointCost { get; }
    }
}
