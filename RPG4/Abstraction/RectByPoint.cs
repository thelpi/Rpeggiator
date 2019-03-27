using System.Windows;

namespace RPG4.Abstractions
{
    public class RectByPoint
    {
        public Point TopLeft { get; private set; }
        public Point BottomRight { get; private set; }
        public Point TopRight { get; private set; }
        public Point BottomLeft { get; private set; }

        public RectByPoint(double left, double top, double right, double bottom)
        {
            TopLeft = new Point(left, top);
            BottomRight = new Point(right, bottom);
            TopRight = new Point(right, top);
            BottomLeft = new Point(left, bottom);
        }

        public bool CollideFromX(SizedPoint pt)
        {
            return (pt.X + pt.Width) > TopLeft.X && pt.X < BottomRight.X;
        }

        public bool CollideFromY(SizedPoint pt)
        {
            return (pt.Y + pt.Height) > TopLeft.Y && pt.Y < BottomRight.Y;
        }

        public bool Collide(SizedPoint pt)
        {
            return CollideFromX(pt) && CollideFromY(pt);
        }

        public Point CheckCollide(SizedPoint currentPt, SizedPoint originalPt, bool? goLeft, bool? goUp)
        {
            if (Collide(currentPt))
            {
                return new Point(
                    goLeft.HasValue && !CollideFromX(originalPt) ? (goLeft == true ? BottomRight.X : (TopLeft.X - originalPt.Width)) : currentPt.X,
                    goUp.HasValue && !CollideFromY(originalPt) ? (goUp == true ? BottomRight.Y : (TopLeft.Y - originalPt.Height)) : currentPt.Y
                );
            }

            return new Point(-1, -1);
        }
    }
}
