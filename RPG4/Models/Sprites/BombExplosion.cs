using RPG4.Models.Graphic;

namespace RPG4.Models.Sprites
{
    /// <summary>
    /// Represents a <see cref="ActionnedBomb"/> explosion.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class BombExplosion : Sprite
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="bomb"><see cref="ActionnedBomb"/></param>
        public BombExplosion(ActionnedBomb bomb) : base(bomb.X - bomb.Width, bomb.Y - bomb.Height,
            bomb.Width * Constants.Bomb.EXPLOSION_SIZE_RATIO, bomb.Height * Constants.Bomb.EXPLOSION_SIZE_RATIO,
            Constants.Bomb.EXPLOSION_GRAPHIC_RENDERING) { }
    }
}
