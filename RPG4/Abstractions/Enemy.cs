using System.Linq;

namespace RPG4.Abstractions
{
    public class Enemy : SizedPoint
    {
        public double Speed { get; private set; } // distance in pixels by tick
        public SizedPoint Pattern { get; private set; }
        public bool HourRotation { get; private set; }
        public int KickTolerance { get; private set; }
        public int KickCount { get; private set; }

        public Enemy(double x, double y, double width, double height, double distanceByTick, SizedPoint movePattern, bool hourRotation, int kickTolerance)
            : base (x, y, width, height)
        {
            Speed = distanceByTick;
            Pattern = movePattern;
            HourRotation = hourRotation;
            KickTolerance = kickTolerance;
        }

        public Enemy(dynamic pngJson) : base((object)pngJson)
        {
            Speed = pngJson.Speed;
            HourRotation = pngJson.HourRotation;
            KickTolerance = pngJson.KickTolerance;
            Pattern = new SizedPoint(pngJson.Pattern);
        }

        public override void ComputeBehaviorAtTick(AbstractEngine engine, KeyPress keys)
        {
            double nextX = X;
            double nextY = Y;

            bool isRight = X + Width >= Pattern.BottomRightX;
            bool isDown = Y + Height >= Pattern.BottomRightY;
            bool isLeft = X <= Pattern.X;
            bool isUp = Y <= Pattern.Y;

            // si à droite
            if (isRight)
            {
                // si en bas (horaire) ou en haut (antihoraire)
                if ((HourRotation && isDown) || (!HourRotation && isUp))
                {
                    // vers la gauche
                    nextX += Speed * -1;
                }
                else
                {
                    // vers le bas (horaire) ou vers le haut (antihoraire)
                    nextY += Speed * (HourRotation ? 1 : -1);
                }
            }
            // si à gauche
            else if (isLeft)
            {
                // si en haut (horaire) ou en bas (antihoraire)
                if ((HourRotation && isUp) || (!HourRotation && isDown))
                {
                    // vers la droite
                    nextX += Speed;
                }
                else
                {
                    // vers le haut (horaire) ou vers le bas (antihoraire)
                    nextY += Speed * (HourRotation ? -1 : 1);
                }
            }
            // sinon
            else
            {
                // si en bas
                if (isDown)
                {
                    // vers la gauche (horaire) ou la droite (antihoraire)
                    nextX += Speed * (HourRotation ? -1 : 1);
                }
                // si en haut
                else if (isUp)
                {
                    // vers la droite (horaire) ou la gauche (antihoraire)
                    nextX += Speed * (HourRotation ? 1 : -1);
                }
            }

            // ajustement au bord (pertinence ?)
            if (nextX < Pattern.X)
            {
                nextX = Pattern.X;
            }
            else if (nextX + Width > Pattern.BottomRightX)
            {
                nextX = Pattern.BottomRightX - Width;
            }
            if (nextY < Pattern.Y)
            {
                nextY = Pattern.Y;
            }
            else if (nextY + Height > Pattern.BottomRightY)
            {
                nextY = Pattern.BottomRightY - Height;
            }

            if (engine.ConcreteWalls.Any(w => w.Overlap(Copy(nextX, nextY))))
            {
                HourRotation = !HourRotation;
            }
            else
            {
                X = nextX;
                Y = nextY;
            }
        }

        public void ApplyKick()
        {
            HourRotation = !HourRotation;
            KickCount++;
        }
    }
}
