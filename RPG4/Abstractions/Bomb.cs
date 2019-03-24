using System;
using System.Collections.Generic;

namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents the bomb item when dropped on the floor.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class Bomb : Sprite
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
        // Life points cost when exploding nearby something.
        private static readonly Dictionary<Type, double> LIFE_POINT_COST = new Dictionary<Type, double>
        {
            { typeof(Player), 3 },
            { typeof(Enemy), 2 },
            { typeof(Rift), 5 },
        };

        // Frames count before exploding.
        private int _pendingFrameCount;

        /// <summary>
        /// Indicates if the bomb is pending for explosion.
        /// </summary>
        public bool IsPending { get { return _pendingFrameCount >= 0; } }
        /// <summary>
        /// Explosion halo.
        /// </summary>
        public HaloSprite ExplosionHalo { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        public Bomb(double x, double y) : base(x, y, WIDTH, HEIGHT)
        {
            _pendingFrameCount = PENDING_FRAME_COUNT;
            ExplosionHalo = new HaloSprite(X, Y, Width, Height, HALO_SIZE_RATIO, EXPLODING_FRAME_COUNT);
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame(AbstractEngine engine, params object[] args)
        {
            ExplosionHalo.BehaviorAtNewFrame(engine, _pendingFrameCount == 0, this);
            // we continue to decrease after the explosion starts to avoid that the condition above, which triggers the explosion, would be always TRUE
            _pendingFrameCount--;
        }

        /// <summary>
        /// Gets the life points nearby the specified instance, at the specific frame (not global).
        /// </summary>
        /// <param name="sprite"><see cref="Sprite"/> (<see cref="Enemy"/>, <see cref="Player"/> or <see cref="Rift"/>).</param>
        /// <returns>Life points cost.</returns>
        public double GetLifePointCost(Sprite sprite)
        {
            var tmp = ExplosionHalo.Active && ExplosionHalo.Overlap(sprite) ?
                LIFE_POINT_COST[sprite.GetType()] / ExplosionHalo.HaloFrameMaxCount : 0;
            return tmp;
        }
    }
}
