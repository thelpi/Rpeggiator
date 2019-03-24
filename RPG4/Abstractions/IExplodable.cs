namespace RPG4.Abstractions
{
    /// <summary>
    /// Ths interface indicates a <see cref="Sprite"/> can be damages by <see cref="Bomb"/>.
    /// </summary>
    public interface IExplodable
    {
        /// <summary>
        /// Indicates the life points cost when a bomb explodes nearby.
        /// </summary>
        double ExplosionLifePointCost { get; }
    }
}
