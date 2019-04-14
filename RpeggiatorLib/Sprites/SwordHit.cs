using RpeggiatorLib.Render;

namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents the sword hit sprite.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class SwordHit : Sprite
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        internal SwordHit(double x, double y, double width, double height)
            : base(x, y, width, height, new ImageRender(nameof(Enums.Filename.Sword)))
        {
            // Empty.
        }

        /// <summary>
        /// Adjust the position of the sprite regarding to <see cref="Engine.Player"/>.
        /// </summary>
        internal void AdjustToPlayer()
        {
            // Shortcut.
            Player p = Engine.Default.Player;
            Move(X + (p.X - (p.LatestMove?.X ?? p.X)), Y + (p.Y - (p.LatestMove?.Y ?? p.Y)));
        }
    }
}
