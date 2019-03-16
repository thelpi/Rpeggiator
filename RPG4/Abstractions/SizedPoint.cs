namespace RPG4.Abstractions
{
    public class SizedPoint
    {
        public double X { get; protected set; }
        public double Y { get; protected set; }
        public double Width { get; protected set; }
        public double Height { get; protected set; }

        public SizedPoint(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public SizedPoint Copy(double x, double y)
        {
            return new SizedPoint(x, y, Width, Height);
        }

        public bool CollideWith(SizedPoint other)
        {
            bool xCase1 = X < other.X && X + Width > other.X;
            bool xCase2 = X < other.X + other.Width && X + Width > other.X + other.Width;
            bool xCase3 = X > other.X && X + Width < other.X + other.Width;
            bool xCase4 = X < other.X && X + Width > other.X + other.Width;

            bool yCase1 = Y < other.Y && Y + Height > other.Y;
            bool yCase2 = Y < other.Y + other.Height && Y + Height > other.Y + other.Height;
            bool yCase3 = Y > other.Y && Y + Height < other.Y + other.Height;
            bool yCase4 = Y < other.Y && Y + Height > other.Y + other.Height;

            return (xCase1 || xCase2 || xCase3 || xCase4) && (yCase1 || yCase2 || yCase3 || yCase4);
        }

        public virtual void ComputeNewPositionAtTick(AbstractEngine engine, KeyPress keys)
        {
            // todo something ?
        }
    }
}
