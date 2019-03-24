﻿using System;
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
        private int _pendingExplosionFrameCount;
        // Frames count while exploding.
        private int _explosionFrameCount;

        /// <summary>
        /// Explosion <see cref="Sprite"/>.
        /// </summary>
        public Sprite ExplosionSprite { get; private set; }
        /// <summary>
        /// Inferred; Indicates the bomb explodes now.
        /// </summary>
        public bool IsExploding { get { return _explosionFrameCount == 0; } }
        /// <summary>
        /// Inferred; Indicates the bomb's explosion is done.
        /// </summary>
        public bool IsDone { get { return _explosionFrameCount > EXPLODING_FRAME_COUNT; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        public Bomb(double x, double y) : base(x, y, WIDTH, HEIGHT)
        {
            _pendingExplosionFrameCount = PENDING_FRAME_COUNT;
            ExplosionSprite = null;
            _explosionFrameCount = -1;
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame(AbstractEngine engine, params object[] args)
        {
            // Explosion beginning.
            if (_pendingExplosionFrameCount == 0)
            {
                _pendingExplosionFrameCount = -1;
                _explosionFrameCount = 0;
                ExplosionSprite = new Sprite(X - Width, Y - Height, Width * HALO_SIZE_RATIO, Height * HALO_SIZE_RATIO);
            }
            // Explosion pending.
            else if (_pendingExplosionFrameCount > 0 && _pendingExplosionFrameCount <= PENDING_FRAME_COUNT)
            {
                _pendingExplosionFrameCount--;
            }
            // Post-explosion.
            else
            {
                _explosionFrameCount++;
                if (_explosionFrameCount > EXPLODING_FRAME_COUNT)
                {
                    ExplosionSprite = null;
                }
            }
        }

        /// <summary>
        /// Gets the life points nearby the specified instance, at the specific frame (not global).
        /// </summary>
        /// <param name="sprite"><see cref="Sprite"/> (<see cref="Enemy"/>, <see cref="Player"/> or <see cref="Rift"/>).</param>
        /// <returns>Life points cost.</returns>
        public double GetLifePointCost(Sprite sprite)
        {
            return IsExploding && ExplosionSprite.Overlap(sprite) ? LIFE_POINT_COST[sprite.GetType()] : 0;
        }
    }
}
