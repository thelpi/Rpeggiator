using System.Windows;

namespace RPG4.Abstraction
{
    /// <summary>
    /// Represents a <see cref="Path"/> step.
    /// </summary>
    public struct PathStep
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
        /// Constructor.
        /// </summary>
        /// <param name="point"><see cref="Point"/></param>
        /// <param name="permanent"><see cref="Permanent"/></param>
        public PathStep(Point point, bool permanent)
        {
            Point = point;
            Permanent = permanent;
        }
    }
}
