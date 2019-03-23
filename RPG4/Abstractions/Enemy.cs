﻿using System.Linq;

namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents an enemy.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class Enemy : Sprite
    {
        /// <summary>
        /// Distance in pixels by tick.
        /// </summary>
        public double Speed { get; private set; }
        /// <summary>
        /// Movement pattern.
        /// </summary>
        public Sprite Pattern { get; private set; }
        /// <summary>
        /// Indicates the current rotation on <see cref="Pattern"/>.
        /// </summary>
        public bool HourRotation { get; private set; }
        /// <summary>
        /// Macimal number of life points.
        /// </summary>
        public int MaximalLifePoints { get; private set; }
        /// <summary>
        /// Current number of life points.
        /// </summary>
        public int CurrentLifePoints { get; private set; }
        /// <summary>
        /// When kicking, indicates the life-points cost on the enemy.
        /// </summary>
        public int HitLifePointCost { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="speed"><see cref="Speed"/></param>
        /// <param name="pattern"><see cref="Pattern"/></param>
        /// <param name="hourRotation"><see cref="HourRotation"/></param>
        /// <param name="maximalLifePoints"><see cref="MaximalLifePoints"/></param>
        public Enemy(double x, double y, double width, double height, double speed, Sprite pattern, bool hourRotation, int maximalLifePoints)
            : base (x, y, width, height)
        {
            Speed = speed;
            Pattern = pattern;
            HourRotation = hourRotation;
            MaximalLifePoints = maximalLifePoints;
            CurrentLifePoints = maximalLifePoints;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pngJson">The json dynamic object.</param>
        public Enemy(dynamic pngJson) : base((object)pngJson)
        {
            Speed = pngJson.Speed;
            HourRotation = pngJson.HourRotation;
            MaximalLifePoints = pngJson.MaximalLifePoints;
            Pattern = new Sprite(pngJson.Pattern);
            CurrentLifePoints = MaximalLifePoints;
        }

        /// <summary>
        /// Behavior of the instance at ticking.
        /// </summary>
        /// <param name="engine"><see cref="AbstractEngine"/></param>
        /// <param name="args">Other arguments.</param>
        public override void ComputeBehaviorAtTick(AbstractEngine engine, params object[] args)
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

            if (engine.SolidStructures.Any(s => s.Overlap(Copy(nextX, nextY))))
            {
                HourRotation = !HourRotation;
            }
            else
            {
                X = nextX;
                Y = nextY;
            }
        }

        /// <summary>
        /// Checks if the instance has been hit and remaining life points.
        /// </summary>
        /// <param name="engine"><see cref="AbstractEngine"/></param>
        /// <returns><c>True</c> if the instance has no life points remaining; otherwise <c>False</c></returns>
        public bool CheckHitAndHealthStatus(AbstractEngine engine)
        {
            if (engine.Player.CheckHitReachEnemy(this))
            {
                HourRotation = !HourRotation;
                CurrentLifePoints -= engine.Player.HitLifePointCost;
            }

            return CurrentLifePoints <= 0;
        }
    }
}
