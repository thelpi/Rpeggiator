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
        private static readonly double TIME_WHILE_EXPLODING = 1000;
        // Halo graphic rendering.
        private static readonly ISpriteGraphic HALO_GRAPHIC_RENDERING = new PlainBrushGraphic("#FFFF4500");
        // Bomb graphic rendering.
        private static readonly ISpriteGraphic GRAPHIC_RENDERING = new ImageBrushGraphic("Bomb");

        // Timestamp of event beginning (pending or exploding).
        private DateTime _eventTimestamp;
        // Time while exploding.
        private TimeSpan? _explosionTime;
        // Time while pending.
        private TimeSpan? _pendingTime;

        /// <summary>
        /// Explosion <see cref="Sprite"/>.
        /// </summary>
        public Sprite ExplosionSprite { get; private set; }
        /// <summary>
        /// Inferred; Indicates the bomb explodes now.
        /// </summary>
        public bool IsExploding { get { return _explosionTime.HasValue && _explosionTime.Value.TotalMilliseconds == 0; } }
        /// <summary>
        /// Inferred; Indicates the bomb's explosion is done.
        /// </summary>
        public override bool IsDone { get { return _explosionTime.HasValue && _explosionTime.Value.TotalMilliseconds > TIME_WHILE_EXPLODING; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        public ActionnedBomb(double x, double y) : base(x, y, WIDTH, HEIGHT, GRAPHIC_RENDERING)
        {
            _pendingTime = new TimeSpan(0);
            _eventTimestamp = DateTime.Now;
            ExplosionSprite = null;
            _explosionTime = null;
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame(AbstractEngine engine, params object[] args)
        {
            // Explosion beginning.
            if (_pendingTime.HasValue && _pendingTime.Value.TotalMilliseconds >= TIME_WHILE_PENDING)
            {
                _pendingTime = null;
                _explosionTime = new TimeSpan(0);
                _eventTimestamp = DateTime.Now;
                ExplosionSprite = new Sprite(X - Width, Y - Height, Width * HALO_SIZE_RATIO, Height * HALO_SIZE_RATIO, HALO_GRAPHIC_RENDERING);
            }
            // Explosion pending.
            else if (_pendingTime.HasValue && _pendingTime.Value.TotalMilliseconds < TIME_WHILE_PENDING)
            {
                _pendingTime = DateTime.Now - _eventTimestamp;
            }
            // Post-explosion.
            else
            {
                _explosionTime = DateTime.Now - _eventTimestamp;
                if (_explosionTime.Value.TotalMilliseconds > TIME_WHILE_EXPLODING)
                {
                    ExplosionSprite = null;
                }
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
