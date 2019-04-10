using System;
using System.Windows;

namespace RpeggiatorLib.Exceptions
{
    /// <summary>
    /// Exception thrown when the resolution of quadratic equation (to determinate motion) has failed.
    /// </summary>
    /// <seealso cref="Exception"/>
    public class NoQuadraticSolutionException : Exception
    {
        /// <summary>
        /// Starting point.
        /// </summary>
        public Point StartingPoint { get; private set; }
        /// <summary>
        /// Destination point.
        /// </summary>
        public Point DestinationPoint { get; private set; }
        /// <summary>
        /// Distance.
        /// </summary>
        public double Distance { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="startingPoint"><see cref="StartingPoint"/></param>
        /// <param name="destinationPoint"><see cref="DestinationPoint"/></param>
        /// <param name="distance"><see cref="Distance"/></param>
        public NoQuadraticSolutionException(Point startingPoint, Point destinationPoint, double distance)
            : base(Messages.NoQuadraticSolutionExceptionMessage)
        {
            StartingPoint = startingPoint;
            DestinationPoint = destinationPoint;
            Distance = distance;
        }
    }
}
