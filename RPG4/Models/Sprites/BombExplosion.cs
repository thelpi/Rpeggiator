using RPG4.Models.Graphic;

namespace RPG4.Models.Sprites
{
    /// <summary>
    /// Represents a <see cref="ActionnedBomb"/> explosion.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class BombExplosion : Sprite
    {
        // Indicates the ratio size of the explosion (compared to the bomb itself).
        private const double SIZE_RATIO = 3;
        // Explosion graphic rendering.
        private static readonly ISpriteGraphic GRAPHIC_RENDERING = new PlainBrushGraphic("#FFFF4500");

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="bomb"><see cref="ActionnedBomb"/></param>
        public BombExplosion(ActionnedBomb bomb) : base(bomb.X - bomb.Width, bomb.Y - bomb.Height,
            bomb.Width * SIZE_RATIO, bomb.Height * SIZE_RATIO,
            GRAPHIC_RENDERING) { }
    }
}
