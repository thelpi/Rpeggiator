using System.Windows;

namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents a two-dimensional point (i.e. a rectangle).
    /// </summary>
    public class SizedPoint
    {
        /// <summary>
        /// X
        /// </summary>
        public double X { get; protected set; }
        /// <summary>
        /// Y
        /// </summary>
        public double Y { get; protected set; }
        /// <summary>
        /// Width
        /// </summary>
        public double Width { get; protected set; }
        /// <summary>
        /// Height
        /// </summary>
        public double Height { get; protected set; }
        /// <summary>
        /// Inferred; BottomRightX
        /// </summary>
        public double BottomRightX { get { return X + Width; } }
        /// <summary>
        /// Inferred; BottomRightY
        /// </summary>
        public double BottomRightY { get { return Y + Height; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="X"/></param>
        /// <param name="y"><see cref="Y"/></param>
        /// <param name="width"><see cref="Width"/></param>
        /// <param name="height"><see cref="Height"/></param>
        public SizedPoint(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sizedPointJson">The json dynamic object.</param>
        public SizedPoint(dynamic sizedPointJson)
        {
            X = sizedPointJson.X;
            Y = sizedPointJson.Y;
            Width = sizedPointJson.Width;
            Height = sizedPointJson.Height;
        }

        /// <summary>
        /// Makes a copy of the current instance with the same <see cref="Width"/> and <see cref="Height"/>
        /// </summary>
        /// <param name="x">New value for <see cref="X"/>.</param>
        /// <param name="y">New value for <see cref="Y"/>.</param>
        /// <returns>The new instance.</returns>
        public SizedPoint Copy(double x, double y)
        {
            return new SizedPoint(x, y, Width, Height);
        }

        private bool SideOverlap(double a1, double a2, double b1, double b2)
        {
            return (a1 < b1 && a2 >= b1)
                || (b1 < a1 && b2 >= a1)
                || (a1 <= b1 && a2 > b1)
                || (b1 <= a1 && b2 > a1);
        }

        /// <summary>
        /// Checks if the instance horizontally overlaps another instance.
        /// </summary>
        /// <param name="other">The second instance.</param>
        /// <returns><c>True</c> if overlaps; <c>False</c> otherwise.</returns>
        private bool HorizontalOverlap(SizedPoint other)
        {
            return SideOverlap(X, BottomRightX, other.X, other.BottomRightX);
        }

        /// <summary>
        /// Checks if the instance vertically overlaps another instance.
        /// </summary>
        /// <param name="other">The second instance.</param>
        /// <returns><c>True</c> if overlaps; <c>False</c> otherwise.</returns>
        private bool VerticalOverlap(SizedPoint other)
        {
            return SideOverlap(Y, BottomRightY, other.Y, other.BottomRightY);
        }

        /// <summary>
        /// Checks if the instance overlaps another instance.
        /// </summary>
        /// <param name="other">The second instance.</param>
        /// <returns><c>True</c> if overlaps; <c>False</c> otherwise.</returns>
        public bool Overlap(SizedPoint other)
        {
            return HorizontalOverlap(other) && VerticalOverlap(other);
        }

        /// <summary>
        /// Overridden; behavior of the instance at ticking.
        /// </summary>
        /// <param name="engine">The <see cref="AbstractEngine"/>.</param>
        /// <param name="keys">Keys pressed at ticking.</param>
        public virtual void ComputeBehaviorAtTick(AbstractEngine engine, KeyPress keys)
        {
            // no default behavior to implement
        }

        /// <summary>
        /// Checks if the instance overlaps another instance, and proposes a new position for the second instance if overlapping is confirmed.
        /// </summary>
        /// <param name="currentPt">The second instance current position.</param>
        /// <param name="originalPt">The second instance previous position.</param>
        /// <param name="goLeft">Indicates if the second instance is going to the left.</param>
        /// <param name="goUp">Indicates if the second instance is going up.</param>
        /// <returns>A <see cref="Point"/> indicating the new position for the second instance if overlapping; A fake <see cref="Point"/> otherwise.</returns>
        public Point CheckOverlapAndAdjustPosition(SizedPoint currentPt, SizedPoint originalPt, bool? goLeft, bool? goUp)
        {
            if (Overlap(currentPt))
            {
                return new Point(
                    goLeft.HasValue && !HorizontalOverlap(originalPt) ? (goLeft == true ? BottomRightX : (X - originalPt.Width)) : currentPt.X,
                    goUp.HasValue && !VerticalOverlap(originalPt) ? (goUp == true ? BottomRightY : (Y - originalPt.Height)) : currentPt.Y
                );
            }

            return new Point(-1, -1);
        }
    }
}
