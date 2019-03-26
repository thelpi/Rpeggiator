namespace RPG4.Abstractions
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
        public const double FALL_IN_OVERLAP_RATIO = 0.25;

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

        /// <inheritdoc />
        public override void BehaviorAtNewFrame(AbstractEngine engine, params object[] args)
        {
            base.BehaviorAtNewFrame(engine, args);
        }

        /// <summary>
        /// Checks if the instance overlaps another instance.
        /// </summary>
        /// <param name="other">The second instance.</param>
        /// <returns><c>True</c> if overlaps; <c>False</c> otherwise.</returns>
        public new bool Overlap(Sprite other)
        {
            return base.Overlap(other, FALL_IN_OVERLAP_RATIO);
        }
    }
}
