using System.Collections.Generic;
using System.Linq;

namespace RPG4.Abstractions
{
    public class PngBehavior : SizedPoint
    {
        public double DistanceByTick { get; private set; }
        public RectByPoint MovePattern { get; private set; }
        public bool HourRotation { get; private set; }
        public int KickCount { get; private set; }
        public int KickTolerance { get; private set; }

        public PngBehavior(double x, double y, double width, double height, double distanceByTick, RectByPoint movePattern, bool hourRotation, int kickTolerance)
            : base (x, y, width, height)
        {
            DistanceByTick = distanceByTick;
            MovePattern = movePattern;
            HourRotation = hourRotation;
            KickTolerance = kickTolerance;
        }

        public override void ComputeNewPositionAtTick(AbstractEngine engine, KeyPress keys)
        {
            double nextX = X;
            double nextY = Y;

            bool isRight = X + Width >= MovePattern.BottomRight.X;
            bool isDown = Y + Height >= MovePattern.BottomRight.Y;
            bool isLeft = X <= MovePattern.TopLeft.X;
            bool isUp = Y <= MovePattern.TopLeft.Y;

            // si à droite
            if (isRight)
            {
                // si en bas (horaire) ou en haut (antihoraire)
                if ((HourRotation && isDown) || (!HourRotation && isUp))
                {
                    // vers la gauche
                    nextX += DistanceByTick * -1;
                }
                else
                {
                    // vers le bas (horaire) ou vers le haut (antihoraire)
                    nextY += DistanceByTick * (HourRotation ? 1 : -1);
                }
            }
            // si à gauche
            else if (isLeft)
            {
                // si en haut (horaire) ou en bas (antihoraire)
                if ((HourRotation && isUp) || (!HourRotation && isDown))
                {
                    // vers la droite
                    nextX += DistanceByTick;
                }
                else
                {
                    // vers le haut (horaire) ou vers le bas (antihoraire)
                    nextY += DistanceByTick * (HourRotation ? -1 : 1);
                }
            }
            // sinon
            else
            {
                // si en bas
                if (isDown)
                {
                    // vers la gauche (horaire) ou la droite (antihoraire)
                    nextX += DistanceByTick * (HourRotation ? -1 : 1);
                }
                // si en haut
                else if (isUp)
                {
                    // vers la droite (horaire) ou la gauche (antihoraire)
                    nextX += DistanceByTick * (HourRotation ? 1 : -1);
                }
            }

            // ajustement au bord (pertinence ?)
            if (nextX < MovePattern.TopLeft.X)
            {
                nextX = MovePattern.TopLeft.X;
            }
            else if (nextX + Width > MovePattern.BottomRight.X)
            {
                nextX = MovePattern.BottomRight.X - Width;
            }
            if (nextY < MovePattern.TopLeft.Y)
            {
                nextY = MovePattern.TopLeft.Y;
            }
            else if (nextY + Height > MovePattern.BottomRight.Y)
            {
                nextY = MovePattern.BottomRight.Y - Height;
            }

            if (engine.Walls.Any(w => w.Collide(Copy(nextX, nextY))))
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
