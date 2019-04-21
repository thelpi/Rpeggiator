using RpeggiatorLib.Renders;

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
        public bool Deadly { get { return !ScreenIdEntrance.HasValue; } }
        /// <summary>
        /// The identifier of the new screen. <c>Null</c> if the pit is deadly.
        /// </summary>
        /// <remarks>Applies to <see cref="Player"/> only.</remarks>
        public int? ScreenIdEntrance { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"><see cref="Sprite.Id"/></param>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="screenIdEntrance"><see cref="ScreenIdEntrance"/></param>
        /// <param name="renderType"><see cref="Render.Render"/> subtype name.</param>
        /// <param name="renderProperties">Datas required to initialize the <see cref="Render.Render"/>.</param>
        internal Pit(int id, double x, double y, double width, double height,
            int? screenIdEntrance, string renderType, object[] renderProperties)
            : base(id, x, y, width, height, renderType, renderProperties)
        {
            ScreenIdEntrance = screenIdEntrance;
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
