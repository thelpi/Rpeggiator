using System.Collections.Generic;
using System.Linq;

namespace RPG4.Abstractions
{
    public class PngBehavior : SizedPoint
    {
        public double DistanceByTick { get; private set; }
        public SizedPoint MovePattern { get; private set; }
        public bool HourRotation { get; private set; }
        public int KickCount { get; private set; }
        public int KickTolerance { get; private set; }

        public PngBehavior(double x, double y, double width, double height, double distanceByTick, SizedPoint movePattern, bool hourRotation, int kickTolerance)
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

            bool isRight = X + Width >= MovePattern.BottomRightX;
            bool isDown = Y + Height >= MovePattern.BottomRightY;
            bool isLeft = X <= MovePattern.X;
            bool isUp = Y <= MovePattern.Y;

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
            if (nextX < MovePattern.X)
            {
                nextX = MovePattern.X;
            }
            else if (nextX + Width > MovePattern.BottomRightX)
            {
                nextX = MovePattern.BottomRightX - Width;
            }
            if (nextY < MovePattern.Y)
            {
                nextY = MovePattern.Y;
            }
            else if (nextY + Height > MovePattern.BottomRightY)
            {
                nextY = MovePattern.BottomRightY - Height;
            }

            if (engine.Walls.Any(w => w.CollideWith(Copy(nextX, nextY))))
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
