﻿using RpeggiatorLib.Sprites;

namespace RpeggiatorLib
{
    /// <summary>
    /// Represents a <see cref="Path"/> step.
    /// </summary>
    internal class PathStep
    {
        /// <summary>
        /// <see cref="Point"/>
        /// </summary>
        public Point Point { get; private set; }
        /// <summary>
        /// Indicates if the step is permanent.
        /// </summary>
        public bool Permanent { get; private set; }
        /// <summary>
        /// Indicates if the step is for pursue.
        /// </summary>
        public bool Pursue { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="point"><see cref="Point"/></param>
        /// <param name="permanent"><see cref="Permanent"/></param>
        internal PathStep(Point point, bool permanent) : this(point, permanent, false) { }

        // Private constructor.
        private PathStep(Point point, bool permanent, bool pursue)
        {
            Point = point;
            Permanent = permanent && !pursue;
            Pursue = pursue;
        }

        /// <summary>
        /// Creates a <see cref="PathStep"/> to pursue a sprite.
        /// </summary>
        /// <param name="spriteToPursue"><see cref="Sprite"/> to pursue.</param>
        /// <returns><see cref="PathStep"/></returns>
        internal static PathStep CreatePursueStep(Sprite spriteToPursue)
        {
            return new PathStep(spriteToPursue.TopLeftCorner, false, true);
        }
    }
}
