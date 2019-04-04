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
        /// <summary>
        /// Bomb width.
        /// </summary>
        public const double WIDTH = 20;
        /// <summary>
        /// Bomb height.
        /// </summary>
        public const double HEIGHT = 20;
        // Milliseconds while pending explosion.
        private static readonly double TIME_WHILE_PENDING = 2000;
        // Milliseconds while exploding.
        private static readonly double TIME_WHILE_EXPLODING = 500;
        // Bomb graphic rendering.
        private static readonly ISpriteGraphic GRAPHIC_RENDERING = new ImageBrushGraphic("Bomb");

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
        public ActionnedBomb(Point point) : base(point.X, point.Y, WIDTH, HEIGHT, GRAPHIC_RENDERING)
        {
            _pendingTimeManager = new Elapser(TIME_WHILE_PENDING);
            ExplosionSprite = null;
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame()
        {
            // Explosion beginning.
            if (_explodingTimeManager == null && _pendingTimeManager.Elapsed)
            {
                _explodingTimeManager = new Elapser(TIME_WHILE_EXPLODING);
                ExplosionSprite = new BombExplosion(this);
            }
            else if (IsDone)
            {
                ExplosionSprite = null;
            }
        }

        /// <summary>
        /// Gets the life points nearby the specified instance, at the specific frame (not global).
        /// </summary>
        /// <typeparam name="T">Type of sprite requirement (must inherit from <see cref="IExplodable"/>).</typeparam>
        /// <param name="sprite"><see cref="Sprite"/>.</param>
        /// <returns>Life points cost.</returns>
        public double GetLifePointCost<T>(T sprite) where T : Sprite, IExplodable
        {
            return IsExploding && ExplosionSprite.Overlap(sprite) ? sprite.ExplosionLifePointCost : 0;
        }
    }
}
