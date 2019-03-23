namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents the bomb item when dropped on the floor.
    /// </summary>
    /// <seealso cref="HaloSprite"/>
    /// <seealso cref="Item"/>
    public class Bomb : HaloSprite
    {
        // ticks count before exploding
        private int _pendingTickCount;

        /// <summary>
        /// Indicates if the bomb is pending for explosion.
        /// </summary>
        public bool IsPending { get { return _pendingTickCount >= 0; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        public Bomb(double x, double y, double width, double height)
            : base(x, y, width, height, Constants.BOMB_HALO_SIZE_RATIO, Constants.BOMB_EXPLODING_TICK_COUNT)
        {
            _pendingTickCount = Constants.BOMB_PENDING_TICK_COUNT;
        }

        /// <summary>
        /// Instance behavior at tick.
        /// </summary>
        /// <param name="engine"><see cref="AbstractEngine"/></param>
        /// <param name="args">Other arguments.</param>
        public override void ComputeBehaviorAtTick(AbstractEngine engine, params object[] args)
        {
            base.ComputeBehaviorAtTick(engine, _pendingTickCount == 0);
            // we continue to decrease after the explosion starts to avoid that the condition above, which triggers the explosion, would be always TRUE
            _pendingTickCount--;
        }
    }
}
