namespace RpeggiatorLib
{
    /// <summary>
    /// Represents a point coordinates.
    /// </summary>
    internal struct Point
    {
        /// <summary>
        /// X-axis coordinate.
        /// </summary>
        internal double X { get; set; }
        /// <summary>
        /// Y-axis coordinate.
        /// </summary>
        internal double Y { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="X"/></param>
        /// <param name="y"><see cref="Y"/></param>
        internal Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
