using System.Windows;

namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents a two-dimensional point (i.e. a rectangle) which evolves in time.
    /// </summary>
    public class Sprite
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
        public Sprite(double x, double y, double width, double height)
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
        public Sprite(dynamic sizedPointJson)
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
        public Sprite Copy(double x, double y)
        {
            return new Sprite(x, y, Width, Height);
        }

        // Checks if the instance overlaps another instance on one dimension.
        private bool OndeDimensionOverlap(double d1, double d2, double od1, double od2)
        {
            // 2 cases should be enough
            bool xCase1 = d1 < od1 && d2 > od1;
            bool xCase2 = d1 < od2 && d2 > od2;
            bool xCase3 = d1 > od1 && d2 < od2;

            return xCase1 || xCase2 || xCase3;
        }

        // Checks if the instance horizontally overlaps another instance.
        private bool HorizontalOverlap(Sprite other)
        {
            return OndeDimensionOverlap(X, BottomRightX, other.X, other.BottomRightX);
        }

        // Checks if the instance vertically overlaps another instance.
        private bool VerticalOverlap(Sprite other)
        {
            return OndeDimensionOverlap(Y, BottomRightY, other.Y, other.BottomRightY);
        }

        /// <summary>
        /// Checks if the instance overlaps another instance.
        /// </summary>
        /// <param name="other">The second instance.</param>
        /// <returns><c>True</c> if overlaps; <c>False</c> otherwise.</returns>
        public bool Overlap(Sprite other)
        {
            return HorizontalOverlap(other) && VerticalOverlap(other);
        }

        /// <summary>
        /// Overridden; behavior of the instance at ticking.
        /// </summary>
        /// <param name="engine">The <see cref="AbstractEngine"/>.</param>
        /// <param name="args">Other arguments.</param>
        public virtual void ComputeBehaviorAtTick(AbstractEngine engine, params object[] args)
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
        public Point CheckOverlapAndAdjustPosition(Sprite currentPt, Sprite originalPt, bool? goLeft, bool? goUp)
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
