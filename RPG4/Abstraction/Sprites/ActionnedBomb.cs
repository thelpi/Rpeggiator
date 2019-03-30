using RPG4.Abstraction.Graphic;
using System;

namespace RPG4.Abstraction.Sprites
{
    /// <summary>
    /// Represents the bomb item when dropped on the floor.
    /// </summary>
    /// <seealso cref="ActionnedItem"/>
    public class ActionnedBomb : ActionnedItem
    {
        // Width.
        private const double WIDTH = 20;
        // Height.
        private const double HEIGHT = 20;
        // When exploding, indicates the ratio size of the halo (compared to the bomb itself).
        private const double HALO_SIZE_RATIO = 3;
        // Milliseconds while pending explosion.
        private static readonly double TIME_WHILE_PENDING = 2000;
        // Milliseconds while exploding.
        private static readonly double TIME_WHILE_EXPLODING = 500;
        // Halo graphic rendering.
        private static readonly ISpriteGraphic HALO_GRAPHIC_RENDERING = new PlainBrushGraphic("#FFFF4500");
        // Bomb graphic rendering.
        private static readonly ISpriteGraphic GRAPHIC_RENDERING = new ImageBrushGraphic("Bomb");

        // Pending time manager
        private Elapser _pendingTimeManager;
        // Pending time manager
        private Elapser _explodingTimeManager;

        /// <summary>
        /// Explosion <see cref="Sprite"/>.
        /// </summary>
        public Sprite ExplosionSprite { get; private set; }
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
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        public ActionnedBomb(double x, double y) : base(x, y, WIDTH, HEIGHT, GRAPHIC_RENDERING)
        {
            _pendingTimeManager = new Elapser(TIME_WHILE_PENDING);
            ExplosionSprite = null;
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame(AbstractEngine engine, params object[] args)
        {
            // Explosion beginning.
            if (_explodingTimeManager == null && _pendingTimeManager.Elapsed)
            {
                _explodingTimeManager = new Elapser(TIME_WHILE_EXPLODING);
                ExplosionSprite = new Sprite(X - Width, Y - Height, Width * HALO_SIZE_RATIO, Height * HALO_SIZE_RATIO, HALO_GRAPHIC_RENDERING);
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
