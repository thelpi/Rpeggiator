using RPG4.Models.Graphic;
using System.Windows;

namespace RPG4.Models.Sprites
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
        public ActionnedBomb(Point point) : base(point.X, point.Y,
            Constants.Bomb.WIDTH, Constants.Bomb.HEIGHT,
            Constants.Bomb.GRAPHIC_RENDERING)
        {
            _pendingTimeManager = new Elapser(Constants.Bomb.TIME_WHILE_PENDING);
            ExplosionSprite = null;
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame()
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

        /// <summary>
        /// Gets the lifepoints cost for the specified <paramref name="sprite"/>, if the bomb is currently exploding nearby.
        /// </summary>
        /// <typeparam name="T">Type of sprite. Must inherit from <see cref="IExplodable"/>.</typeparam>
        /// <param name="sprite"><see cref="Sprite"/></param>
        /// <returns>Life points cost.</returns>
        public double GetLifePointsCost<T>(T sprite) where T : Sprite, IExplodable
        {
            return IsExploding && ExplosionSprite.Overlap(sprite) ? sprite.ExplosionLifePointCost : 0;
        }
    }
}
