using RPG4.Abstraction.Graphic;
using System.Windows;

namespace RPG4.Abstraction.Sprites
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
        /// Inferred; surface, in pixels square.
        /// </summary>
        public double Surface { get { return Width * Height; } }
        /// <summary>
        /// Graphic rendering.
        /// </summary>
        public ISpriteGraphic Graphic { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="X"/></param>
        /// <param name="y"><see cref="Y"/></param>
        /// <param name="width"><see cref="Width"/></param>
        /// <param name="height"><see cref="Height"/></param>
        /// <param name="graphic"><see cref="Graphic"/></param>
        public Sprite(double x, double y, double width, double height, ISpriteGraphic graphic)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Graphic = graphic;
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
            switch ((string)sizedPointJson.GraphicType)
            {
                case nameof(ImageBrushGraphic):
                    Graphic = new ImageBrushGraphic((string)sizedPointJson.ImagePath);
                    break;
                case nameof(PlainBrushGraphic):
                    Graphic = new PlainBrushGraphic((string)sizedPointJson.HexColor);
                    break;
                    // TODO : other types of ISpriteGraphic must be implemented here.
            }
        }

        /// <summary>
        /// Makes a copy of the current instance with the same <see cref="Width"/> and <see cref="Height"/>
        /// </summary>
        /// <param name="x">New value for <see cref="X"/>.</param>
        /// <param name="y">New value for <see cref="Y"/>.</param>
        /// <returns>The new instance.</returns>
        public Sprite Copy(double x, double y)
        {
            return new Sprite(x, y, Width, Height, Graphic);
        }

        /// <summary>
        /// Checks if the instance overlaps another instance.
        /// </summary>
        /// <param name="other">The second instance.</param>
        /// <param name="overlapMinimalRatio">Optionnal; The minimal ratio of this instance's surface which overlap the second instance.</param>
        /// <returns><c>True</c> if overlaps; <c>False</c> otherwise.</returns>
        public bool Overlap(Sprite other, double overlapMinimalRatio = 0)
        {
            double overlapX = ComputeOneDimensionOverlap(X, BottomRightX, other.X, other.BottomRightX);
            double overlapY = ComputeOneDimensionOverlap(Y, BottomRightY, other.Y, other.BottomRightY);

            double surfaceCovered = ComputeHorizontalOverlap(other) * ComputeVerticalOverlap(other);

            if (overlapMinimalRatio == 0)
            {
                return surfaceCovered > 0;
            }

            double surfaceExpectedCovered = overlapMinimalRatio * other.Surface;

            return surfaceCovered >= surfaceExpectedCovered;
        }

        // Computes an horizontal overlap (width)
        private double ComputeHorizontalOverlap(Sprite other)
        {
            return ComputeOneDimensionOverlap(X, BottomRightX, other.X, other.BottomRightX);
        }

        // Computes a vertival overlap (height)
        private double ComputeVerticalOverlap(Sprite other)
        {
            return ComputeOneDimensionOverlap(Y, BottomRightY, other.Y, other.BottomRightY);
        }

        // Computes a one-dimensional overlap (width / height)
        private double ComputeOneDimensionOverlap(double i1Start, double i1End, double i2Start, double i2End)
        {
            if (i1Start <= i2Start && i1End >= i2End)
            {
                return i2End - i2Start;
            }
            else if (i2Start <= i1Start && i2End >= i1End)
            {
                return i1End - i1Start;
            }
            else if (i1Start <= i2Start && i2Start <= i1End)
            {
                return i1End - i2Start;
            }
            else if (i1Start <= i2End && i1End >= i2End)
            {
                return i2End - i1Start;
            }

            return 0;
        }

        /// <summary>
        /// Overridden; behavior of the instance at new frame.
        /// </summary>
        /// <param name="engine">The <see cref="AbstractEngine"/>.</param>
        /// <param name="args">Other arguments.</param>
        public virtual void BehaviorAtNewFrame(AbstractEngine engine, params object[] args)
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
                    goLeft.HasValue && ComputeHorizontalOverlap(originalPt) == 0 ? (goLeft == true ? BottomRightX : (X - originalPt.Width)) : currentPt.X,
                    goUp.HasValue && ComputeVerticalOverlap(originalPt) == 0 ? (goUp == true ? BottomRightY : (Y - originalPt.Height)) : currentPt.Y
                );
            }

            return new Point(-1, -1);
        }
    }
}
