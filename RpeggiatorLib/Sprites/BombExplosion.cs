using RpeggiatorLib.Renders;

namespace RpeggiatorLib.Sprites
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
        internal BombExplosion(ActionnedBomb bomb)
            : base(bomb.Id, bomb.X - bomb.Width, bomb.Y - bomb.Height,
                  bomb.Width * Constants.Bomb.EXPLOSION_SIZE_RATIO,
                  bomb.Height * Constants.Bomb.EXPLOSION_SIZE_RATIO,
                  Enums.RenderType.PlainRender,
                  new[] { Tools.HexFromColor(System.Windows.Media.Colors.OrangeRed) })
        {
            // Empty.
        }
    }
}
