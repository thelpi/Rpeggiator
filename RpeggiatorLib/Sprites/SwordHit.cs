using RpeggiatorLib.Renders;

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
        /// <param name="id"><see cref="Sprite.Id"/></param>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        internal SwordHit(int id, double x, double y, double width, double height)
            : base(id, x, y, width, height, Enums.RenderType.Plain, new[] { Tools.HexFromColor(System.Windows.Media.Colors.Transparent) })
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
            double newX = 0;
            double newY = 0;
            double newWidth = 0;
            double newHeight = 0;
            switch (p.Direction)
            {
                case Enums.Direction.Bottom:
                    newX = p.X;
                    newY = p.Y + p.Height;
                    newWidth = p.Width;
                    newHeight = p.Height * Constants.Player.HIT_SPRITE_RATIO;
                    break;
                case Enums.Direction.Top:
                    newX = p.X;
                    newY = p.Y - (p.Height * Constants.Player.HIT_SPRITE_RATIO);
                    newWidth = p.Width;
                    newHeight = p.Height * Constants.Player.HIT_SPRITE_RATIO;
                    break;
                case Enums.Direction.Left:
                    newX = p.X - (p.Width * Constants.Player.HIT_SPRITE_RATIO);
                    newY = p.Y;
                    newWidth = p.Width * Constants.Player.HIT_SPRITE_RATIO;
                    newHeight = p.Height;
                    break;
                case Enums.Direction.Right:
                    newX = p.X + p.Width;
                    newY = p.Y;
                    newWidth = p.Width * Constants.Player.HIT_SPRITE_RATIO;
                    newHeight = p.Height;
                    break;
                case Enums.Direction.BottomLeft:
                    newX = p.X - (p.Width * Constants.Player.HIT_SPRITE_RATIO);
                    newY = p.Y + (p.Height * (1 - Constants.Player.HIT_SPRITE_RATIO));
                    newWidth = p.Width * (1 - Constants.Player.HIT_SPRITE_RATIO);
                    newHeight = p.Height * (1 - Constants.Player.HIT_SPRITE_RATIO);
                    break;
                case Enums.Direction.BottomRight:
                    newX = p.X + (p.Width * (1 - Constants.Player.HIT_SPRITE_RATIO));
                    newY = p.Y + (p.Height * (1 - Constants.Player.HIT_SPRITE_RATIO));
                    newWidth = p.Width * (1 - Constants.Player.HIT_SPRITE_RATIO);
                    newHeight = p.Height * (1 - Constants.Player.HIT_SPRITE_RATIO);
                    break;
                case Enums.Direction.TopLeft:
                    newX = p.X - (p.Width * Constants.Player.HIT_SPRITE_RATIO);
                    newY = p.Y - (p.Height * Constants.Player.HIT_SPRITE_RATIO);
                    newWidth = p.Width * (1 - Constants.Player.HIT_SPRITE_RATIO);
                    newHeight = p.Height * (1 - Constants.Player.HIT_SPRITE_RATIO);
                    break;
                case Enums.Direction.TopRight:
                    newX = p.X + (p.Width * (1 - Constants.Player.HIT_SPRITE_RATIO));
                    newY = p.Y - (p.Height * Constants.Player.HIT_SPRITE_RATIO);
                    newWidth = p.Width * (1 - Constants.Player.HIT_SPRITE_RATIO);
                    newHeight = p.Height * (1 - Constants.Player.HIT_SPRITE_RATIO);
                    break;
            }

            Move(newX, newY);
            Resize(newWidth, newHeight);
        }
    }
}
