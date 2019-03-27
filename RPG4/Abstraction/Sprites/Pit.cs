namespace RPG4.Abstraction.Sprites
{
    /// <summary>
    /// Represents a pit, which has two possible deadly for <see cref="Player"/> and <see cref="Enemy"/>
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class Pit : Sprite
    {
        /// <summary>
        /// Indicates the ratio of overlaping to fall into.
        /// </summary>
        public const double FALL_IN_OVERLAP_RATIO = 0.5;

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
        /// Constructor.
        /// </summary>
        /// <param name="sizedPointJson">The json dynamic object.</param>
        public Pit(dynamic sizedPointJson) : base((object)sizedPointJson)
        {
            ScreenIndexEntrance = sizedPointJson.ScreenIndexEntrance;
        }

        /// <summary>
        /// Checks if <paramref name="other"/> can fall in the current instance.
        /// </summary>
        /// <param name="other">The instance to check.</param>
        /// <returns><c>True</c> if <paramref name="other"/> falls in; <c>False</c> otherwise.</returns>
        public bool CanFallIn(Sprite other)
        {
            return Width >= other.Width && Height >= other.Height && Overlap(other, FALL_IN_OVERLAP_RATIO);
        }
    }
}
