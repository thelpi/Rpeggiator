namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents the bomb item when dropped on the floor.
    /// </summary>
    /// <seealso cref="HaloSprite"/>
    /// <seealso cref="Item"/>
    public class Bomb : HaloSprite
    {
        // Width.
        private const double WIDTH = 20;
        // Height.
        private const double HEIGHT = 20;
        // When exploding, indicates the ratio size of the halo (compared to the bomb itself).
        private const double HALO_SIZE_RATIO = 3;
        // Frames count before exploding.
        private static readonly int PENDING_FRAME_COUNT = Constants.FPS * 2;
        // Frames count while exploding.
        private static readonly int EXPLODING_FRAME_COUNT = Constants.FPS;

        // Frames count before exploding.
        private int _pendingFrameCount;

        /// <summary>
        /// Indicates if the bomb is pending for explosion.
        /// </summary>
        public bool IsPending { get { return _pendingFrameCount >= 0; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        public Bomb(double x, double y)
            : base(x, y, WIDTH, HEIGHT, HALO_SIZE_RATIO, EXPLODING_FRAME_COUNT)
        {
            _pendingFrameCount = PENDING_FRAME_COUNT;
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame(AbstractEngine engine, params object[] args)
        {
            base.BehaviorAtNewFrame(engine, _pendingFrameCount == 0);
            // we continue to decrease after the explosion starts to avoid that the condition above, which triggers the explosion, would be always TRUE
            _pendingFrameCount--;
        }
    }
}
