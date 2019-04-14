using System.Collections.Generic;
using System.Linq;
using RpeggiatorLib.Enums;
using RpeggiatorLib.Render;

namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents a two-dimensional point (i.e. a rectangle) which evolves in time.
    /// </summary>
    public class Sprite
    {
        /// <summary>
        /// Default <see cref="ISpriteRender"/>.
        /// </summary>
        protected ISpriteRender _render;

        /// <summary>
        /// X
        /// </summary>
        public double X { get; private set; }
        /// <summary>
        /// Y
        /// </summary>
        public double Y { get; private set; }
        /// <summary>
        /// Z-axis layer when displayed.
        /// </summary>
        public int Z { get; private set; }
        /// <summary>
        /// Width
        /// </summary>
        public double Width { get; protected set; }
        /// <summary>
        /// Height
        /// </summary>
        public double Height { get; protected set; }
        /// <summary>
        /// Overridden; <see cref="_render"/>
        /// </summary>
        public virtual ISpriteRender Render { get { return _render; } }

        /// <summary>
        /// Inferred; top left corner coordinates.
        /// </summary>
        internal Point TopLeftCorner { get { return new Point(X, Y); } }
        /// <summary>
        /// Inferred; BottomRightX
        /// </summary>
        public double BottomRightX { get { return X + Width; } }
        /// <summary>
        /// Inferred; BottomRightY
        /// </summary>
        public double BottomRightY { get { return Y + Height; } }
        /// <summary>
        /// Inferred; Center point X-axis.
        /// </summary>
        public double CenterPointX { get { return X + (Width / 2); } }
        /// <summary>
        /// Inferred; Center point Y-axis.
        /// </summary>
        public double CenterPointY { get { return Y + (Height / 2); } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="X"/></param>
        /// <param name="y"><see cref="Y"/></param>
        /// <param name="width"><see cref="Width"/></param>
        /// <param name="height"><see cref="Height"/></param>
        /// <param name="render"><see cref="Render"/></param>
        protected Sprite(double x, double y, double width, double height, ISpriteRender render)
            : this(x, y, width, height, 0, render)
        {
            Z = GetZIndexBySubType();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="X"/></param>
        /// <param name="y"><see cref="Y"/></param>
        /// <param name="width"><see cref="Width"/></param>
        /// <param name="height"><see cref="Height"/></param>
        /// <param name="renderType"><see cref="ISpriteRender"/> subtype name.</param>
        /// <param name="renderProperties">Datas required to initialize the <see cref="ISpriteRender"/>.</param>
        protected Sprite(double x, double y, double width, double height, string renderType, object[] renderProperties)
            : this(x, y, width, height, 0, null)
        {
            Z = GetZIndexBySubType();
            _render = GetRenderFromValues(renderType, renderProperties);
        }

        // Private constructor.
        private Sprite(double x, double y, double width, double height, int zIndex, ISpriteRender render)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Z = zIndex;
            _render = render;
        }

        /// <summary>
        /// Computes and gets a <see cref="ISpriteRender"/> from given properties.
        /// </summary>
        /// <param name="renderType"><see cref="ISpriteRender"/> subtype name.</param>
        /// <param name="renderProperties">Datas required to initialize the <see cref="ISpriteRender"/>.</param>
        /// <returns><see cref="ISpriteRender"/></returns>
        protected ISpriteRender GetRenderFromValues(string renderType, params object[] renderProperties)
        {
            switch (renderType)
            {
                case nameof(ImageDirectionRender):
                    return new ImageDirectionRender((string)renderProperties[0], this, (string)renderProperties[1]);
                case nameof(ImageMosaicRender):
                    return new ImageMosaicRender((string)renderProperties[0], this);
                case nameof(ImageRender):
                    return new ImageRender((string)renderProperties[0]);
                case nameof(PlainRender):
                    return new PlainRender((string)renderProperties[0]);
                default:
                    throw new System.NotImplementedException(Messages.NotImplementedGraphicExceptionMessage);
            }
        }

        /// <summary>
        /// Moves this instance.
        /// </summary>
        /// <param name="newX">New X-axis value.</param>
        /// <param name="newY">New Y-axis value.</param>
        protected void Move(double newX, double newY)
        {
            X = newX;
            Y = newY;
        }

        /// <summary>
        /// Makes a copy of the current instance on a new position, with the same <see cref="Width"/> and <see cref="Height"/>.
        /// </summary>
        /// <param name="newPosition">The new position.</param>
        /// <returns>The new instance.</returns>
        internal Sprite CopyToPosition(Point newPosition)
        {
            return new Sprite(newPosition.X, newPosition.Y, Width, Height, Z, Render);
        }

        /// <summary>
        /// Checks if the instance overlaps another instance.
        /// </summary>
        /// <param name="other">The second instance.</param>
        /// <param name="overlapMinimalRatio">Optionnal; The minimal ratio of this instance's surface which overlap the second instance.</param>
        /// <returns><c>True</c> if overlaps; <c>False</c> otherwise.</returns>
        internal bool Overlap(Sprite other, double overlapMinimalRatio = 0)
        {
            double surfaceCovered = ComputeHorizontalOverlap(other) * ComputeVerticalOverlap(other);

            if (overlapMinimalRatio.Equal(0))
            {
                return surfaceCovered.Greater(0);
            }

            double surfaceExpectedCovered = overlapMinimalRatio * other.Width * other.Height;

            return surfaceCovered.GreaterEqual(surfaceExpectedCovered);
        }

        /// <summary>
        /// Overridden; behavior of the instance at new frame.
        /// </summary>
        internal virtual void BehaviorAtNewFrame()
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
        internal Point CheckOverlapAndAdjustPosition(Sprite currentPt, Sprite originalPt, bool? goLeft, bool? goUp)
        {
            if (Overlap(currentPt))
            {
                return new Point(
                    goLeft.HasValue && ComputeHorizontalOverlap(originalPt).Equal(0) ? (goLeft == true ? BottomRightX : (X - originalPt.Width)) : currentPt.X,
                    goUp.HasValue && ComputeVerticalOverlap(originalPt).Equal(0) ? (goUp == true ? BottomRightY : (Y - originalPt.Height)) : currentPt.Y
                );
            }

            return new Point(-1, -1);
        }

        /// <summary>
        /// Computes, when a sprite in motion overlaps the current instance, from which direction its comes from.
        /// </summary>
        /// <param name="spriteInMotion"><see cref="Sprite"/> in motion.</param>
        /// <param name="positionToCheck">Position to check.</param>
        /// <returns><see cref="Direction"/> <paramref name="spriteInMotion"/> come before its overlaps; <c>Null</c> if no overlap.</returns>
        internal Direction? DirectionSourceOfOverlap(Sprite spriteInMotion, Point positionToCheck)
        {
            Sprite spriteInPosition = spriteInMotion.CopyToPosition(positionToCheck);

            double newOverlapX = ComputeHorizontalOverlap(spriteInPosition);
            double newOverlapY = ComputeVerticalOverlap(spriteInPosition);
            double oldOverlapX = ComputeHorizontalOverlap(spriteInMotion);
            double oldOverlapY = ComputeVerticalOverlap(spriteInMotion);

            if ((newOverlapX * newOverlapY).Equal(0))
            {
                return null;
            }

            bool fromLeft = spriteInMotion.X.Lower(positionToCheck.X);
            bool fromTop = spriteInMotion.Y.Lower(positionToCheck.Y);

            if (oldOverlapY.Equal(0) && oldOverlapX.Equal(0))
            {
                // in diagonal exactly right on a corner.
                return fromLeft ?
                    (fromTop ? Direction.TopLeft : Direction.BottomLeft)
                    : (fromTop ? Direction.TopRight : Direction.BottomRight);
            }
            else if (oldOverlapY.Equal(0))
            {
                // Y move only.
                return fromTop ? Direction.Top : Direction.Bottom;
            }
            else if (oldOverlapX.Equal(0))
            {
                // X move only.
                return fromLeft ? Direction.Left : Direction.Right;
            }
            else if (newOverlapX.Greater(newOverlapY))
            {
                // X move prior to Y move.
                return fromLeft ? Direction.Left : Direction.Right;
            }
            else
            {
                // Y move prior to X move.
                return fromTop ? Direction.Top : Direction.Bottom;
            }
        }

        /// <summary>
        /// Creates a copy of the current sprite compared to a size ratio.
        /// </summary>
        /// <param name="ratio">The size ratio.</param>
        /// <returns>The sprite copy.</returns>
        internal Sprite ResizeToRatio(double ratio)
        {
            double a = ((1 - ratio) / 2);
            double newX = X + (a * Width);
            double newY = Y + (a * Height);
            return new Sprite(newX, newY, Width * ratio, Height * ratio, Z, Render);
        }

        /// <summary>
        /// Checks if a <see cref="Sprite"/> is completely inside this one.
        /// Usefull to check if a <see cref="Sprite"/> is inside the screen.
        /// </summary>
        /// <param name="other"><see cref="Sprite"/> to check.</param>
        /// <returns><c>True</c> if is inside; <c>False</c> otherwise.</returns>
        internal bool IsInside(Sprite other)
        {
            return other.X.GreaterEqual(X)
                && other.BottomRightX.LowerEqual(BottomRightX)
                && other.Y.GreaterEqual(Y)
                && other.BottomRightY.LowerEqual(BottomRightY);
        }

        /// <summary>
        /// Checks if a <see cref="Point"/> is crossed by this instance while moving to a new point.
        /// </summary>
        /// <remarks>If exactly on the point, it's considered as crossed.</remarks>
        /// <param name="pointToCheck">The <see cref="Point"/> to check.</param>
        /// <param name="nextX">New point X-axis.</param>
        /// <param name="nextY">New point Y-axis.</param>
        /// <returns><c>True</c> if <paramref name="pointToCheck"/> is crossed; <c>False</c> otherwise.</returns>
        internal bool CheckPointIsCrossed(Point pointToCheck, double nextX, double nextY)
        {
            bool xIsCrossed = (X.LowerEqual(pointToCheck.X) && nextX.GreaterEqual(pointToCheck.X))
                || (X.GreaterEqual(pointToCheck.X) && nextX.LowerEqual(pointToCheck.X));
            bool yIsCrossed = (Y.LowerEqual(pointToCheck.Y) && nextY.GreaterEqual(pointToCheck.Y))
                || (Y.GreaterEqual(pointToCheck.Y) && nextY.LowerEqual(pointToCheck.Y));

            return xIsCrossed && yIsCrossed;
        }

        /// <summary>
        /// Checks if the <see cref="Engine.Player"/> is currently looking this instance.
        /// </summary>
        /// <returns><c>True</c> if looking; <c>False</c> otherwise.</returns>
        internal bool PlayerIsLookingTo()
        {
            // Shortcut.
            Player p = Engine.Default.Player;

            switch (p.Direction)
            {
                case Direction.Top:
                    return BottomRightY.LowerEqual(p.Y) && ComputeHorizontalOverlap(p).Greater(0);
                case Direction.Bottom:
                    return Y.GreaterEqual(p.BottomRightY) && ComputeHorizontalOverlap(p).Greater(0);
                case Direction.Right:
                    return X.GreaterEqual(p.BottomRightX) && ComputeVerticalOverlap(p).Greater(0);
                case Direction.Left:
                    return BottomRightX.LowerEqual(p.X) && ComputeVerticalOverlap(p).Greater(0);
                case Direction.BottomLeft:
                    return BottomRightX.LowerEqual(p.X) && Y.GreaterEqual(p.BottomRightY);
                case Direction.BottomRight:
                    return X.GreaterEqual(p.BottomRightX) && Y.GreaterEqual(p.BottomRightY);
                case Direction.TopLeft:
                    return BottomRightX.LowerEqual(p.X) && BottomRightY.LowerEqual(p.Y);
                case Direction.TopRight:
                    return X.GreaterEqual(p.BottomRightX) && BottomRightY.LowerEqual(p.Y);
            }

            return false;
        }

        // Computes an horizontal overlap (width).
        private double ComputeHorizontalOverlap(Sprite other)
        {
            return ComputeOneDimensionOverlap(X, BottomRightX, other.X, other.BottomRightX);
        }

        // Computes a vertival overlap (height).
        private double ComputeVerticalOverlap(Sprite other)
        {
            return ComputeOneDimensionOverlap(Y, BottomRightY, other.Y, other.BottomRightY);
        }

        // Computes a one-dimensional overlap (width / height).
        private double ComputeOneDimensionOverlap(double i1Start, double i1End, double i2Start, double i2End)
        {
            if (i1Start.LowerEqual(i2Start) && i1End.GreaterEqual(i2End))
            {
                return i2End - i2Start;
            }
            else if (i2Start.LowerEqual(i1Start) && i2End.GreaterEqual(i1End))
            {
                return i1End - i1Start;
            }
            else if (i1Start.LowerEqual(i2Start) && i2Start.LowerEqual(i1End))
            {
                return i1End - i2Start;
            }
            else if (i1Start.LowerEqual(i2End) && i1End.GreaterEqual(i2End))
            {
                return i2End - i1Start;
            }

            return 0;
        }

        // Gets the Z-axis index depending on real sprite type.
        private int GetZIndexBySubType()
        {
            Dictionary<int, System.Type[]> typesByZIndex = new Dictionary<int, System.Type[]>
            {
                { 0, new System.Type[] { typeof(Screen) } },
                { 1, new System.Type[] { typeof(Floor) } },
                { 2, new System.Type[] { typeof(Rift), typeof(Pit), typeof(Door), typeof(Gate), typeof(GateTrigger),
                    typeof(PickableItem), typeof(Chest), typeof(PermanentStructure) } },
                { 3, new System.Type[] { typeof(Enemy), typeof(Player) } },
                { 4, new System.Type[] { typeof(BombExplosion), } },
                { 5, new System.Type[] { typeof(ActionnedBomb), typeof(SwordHit), typeof(ActionnedArrow) } }
            };

            foreach (int zIndexKey in typesByZIndex.Keys)
            {
                if (typesByZIndex[zIndexKey].Contains(GetType()))
                {
                    return zIndexKey;
                }
            }

            throw new System.NotImplementedException(Messages.NotImplementedZindexTypeExceptionMessage);
        }
    }
}
