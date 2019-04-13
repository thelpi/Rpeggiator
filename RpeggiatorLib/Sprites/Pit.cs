using RpeggiatorLib.Render;

namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents a pit, which has two possible deadly for <see cref="Player"/> and <see cref="Enemy"/>
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class Pit : Sprite
    {
        /// <summary>
        /// Inferred; indicates if the pit is deadly (I.e. no scrren entrance).
        /// </summary>
        public bool Deadly { get { return !ScreenIndexEntrance.HasValue; } }
        /// <summary>
        /// The index of the new screen. <c>Null</c> if the pit is deadly.
        /// </summary>
        /// <remarks>Applies to <see cref="Player"/> only.</remarks>
        public int? ScreenIndexEntrance { get; private set; }

        /// <summary>
        /// Creates an instance from json datas.
        /// </summary>
        /// <param name="datas">Json datas.</param>
        /// <returns><see cref="Pit"/></returns>
        internal static Pit FromDynamic(dynamic datas)
        {
            Pit p = new Pit((double)datas.X, (double)datas.Y, (double)datas.Width, (double)datas.Height, 
                (int?)datas.ScreenIndexEntrance);
            p.SetRenderFromDynamic((object)datas);
            return p;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="screenIndexEntrance"><see cref="ScreenIndexEntrance"/></param>
        internal Pit(double x, double y, double width, double height, int? screenIndexEntrance)
            : base(x, y, width, height)
        {
            ScreenIndexEntrance = screenIndexEntrance;
        }

        /// <summary>
        /// Checks if <paramref name="other"/> can fall in the current instance.
        /// </summary>
        /// <param name="other">The instance to check.</param>
        /// <returns><c>True</c> if <paramref name="other"/> falls in; <c>False</c> otherwise.</returns>
        internal bool CanFallIn(Sprite other)
        {
            return Width.GreaterEqual(other.Width) && Height.GreaterEqual(other.Height) && Overlap(other, Constants.PIT_FALL_IN_OVERLAP_RATIO);
        }
    }
}
