using RPG4.Abstraction.Sprites;
using System.Windows;

namespace RPG4.Abstraction
{
    /// <summary>
    /// Represents a <see cref="Path"/> step.
    /// </summary>
    public class PathStep
    {
        /// <summary>
        /// <see cref="System.Windows.Point"/>
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
        public PathStep(Point point, bool permanent) : this(point, permanent, false) { }

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
        public static PathStep CreatePursueStep(Sprite spriteToPursue)
        {
            return new PathStep(spriteToPursue.TopLeftCorner, false, true);
        }
    }
}
