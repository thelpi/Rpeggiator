using RpeggiatorLib.Renders;

namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents the bomb item when dropped on the floor.
    /// </summary>
    /// <seealso cref="ActionnedItem"/>
    public class ActionnedBomb : ActionnedItem
    {
        // Pending time manager
        private Elapser _pendingTimeManager;
        // Pending time manager
        private Elapser _explodingTimeManager;

        /// <summary>
        /// <see cref="BombExplosion"/> sprite.
        /// </summary>
        public BombExplosion ExplosionSprite { get; private set; }
        /// <summary>
        /// Inferred; Indicates the bomb explodes now.
        /// </summary>
        public bool IsExploding { get { return _explodingTimeManager?.Elapsed == false; } }
        /// <summary>
        /// Inferred; Indicates the bomb's explosion is done.
        /// </summary>
        public override bool IsDone { get { return _explodingTimeManager?.Elapsed == true; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="point">Drop coordinates.</param>
        internal ActionnedBomb(Point point)
            : base(0, point.X, point.Y, Constants.Bomb.WIDTH, Constants.Bomb.HEIGHT, Enums.RenderType.ImageRender, nameof(Enums.Filename.Bomb))
        {
            _pendingTimeManager = new Elapser(Constants.Bomb.TIME_WHILE_PENDING);
            ExplosionSprite = null;
        }

        /// <inheritdoc />
        internal override void BehaviorAtNewFrame()
        {
            // Explosion beginning.
            if (_explodingTimeManager == null && _pendingTimeManager.Elapsed)
            {
                _explodingTimeManager = new Elapser(Constants.Bomb.TIME_WHILE_EXPLODING);
                ExplosionSprite = new BombExplosion(this);
            }
            else if (IsDone)
            {
                ExplosionSprite = null;
            }
        }

        /// <inheritdoc />
        internal override double GetLifePointsCost(DamageableSprite sprite)
        {
            return IsExploding && ExplosionSprite.Overlap(sprite) ? sprite.ExplosionLifePointCost : 0;
        }
    }
}
