using Newtonsoft.Json;
using RPG4.Abstraction.Sprites;
using RPG4.Properties;
using System;
using System.Windows;

namespace RPG4
{
    /// <summary>
    /// Tool methods. 
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// Gets the screen datas by its index.
        /// </summary>
        /// <param name="screenIndex">Screen index.</param>
        /// <returns>Dynamic screen datas.</returns>
        public static dynamic GetScreenDatasFromIndex(int screenIndex)
        {
            return JsonConvert.DeserializeObject(Resources.ResourceManager.GetString(string.Concat("Screen", screenIndex)));
        }

        /// <summary>
        /// Computes the distance made in diagonal relatively to the distance made straightforward.
        /// </summary>
        /// <param name="frameDistance">Distance made straightforward.</param>
        /// <returns>Diagonal distance.</returns>
        public static double FrameDiagonalDistance(double frameDistance)
        {
            return Math.Sqrt((frameDistance * frameDistance) / 2);
        }

        /// <summary>
        /// Resolves a quadratic equation.
        /// </summary>
        /// <param name="a">First known number.</param>
        /// <param name="b">Second known number.</param>
        /// <param name="c">Third known number.</param>
        /// <returns>A tuple of solutions; or <c>Null</c> if no solution.</returns>
        public static Tuple<double, double> ResolveQuadraticEquation(double a, double b, double c)
        {
            Tuple<double, double> result = null;

            double delta = Math.Sqrt(b) - (4 * a * c);

            if (delta < 0)
            {
                return result;
            }

            return new Tuple<double, double>(
                ((-1 * b) + Math.Sqrt(delta)) / (2 * a),
                ((-1 * b) - Math.Sqrt(delta)) / (2 * a)
            );
        }

        /// <summary>
        /// Gets the properties of a linear function from two points.
        /// </summary>
        /// <param name="p1">First point.</param>
        /// <param name="p2">Second point.</param>
        /// <returns>Slope and initial ordinate; <c>Null</c> if both points have the same abscissa.</returns>
        public static Tuple<double, double> GetLinearFunctionFromPoints(Point p1, Point p2)
        {
            if (p2.X == p1.X)
            {
                return null;
            }

            return new Tuple<double, double>((p2.Y - p1.Y) / (p2.X - p1.X), ((p2.X * p1.Y) - (p1.X * p2.Y)) / (p2.X - p1.X));
        }

        /// <summary>
        /// Gets the coordinates of a point on the line made by two points, at a specific distance from <paramref name="pStart"/>.
        /// </summary>
        /// <param name="pStart">The starting <see cref="Point"/></param>
        /// <param name="pDest">The destination <see cref="Point"/></param>
        /// <param name="distance">The distance, in pixels.</param>
        /// <param name="adjust"><c>True</c> to not go further than <paramref name="pDest"/>; <c>False</c> otherwise.</param>
        /// <returns>Coordinates of the point.</returns>
        public static Point GetIntermediatePointBetweenTwoSprite(Point pStart, Point pDest, double distance, bool adjust)
        {
            if (distance == 0)
            {
                return pStart;
            }

            var linearProperties = GetLinearFunctionFromPoints(pStart, pDest);

            if (linearProperties == null)
            {
                double totalDistance = pDest.Y - pStart.Y;
                if (Math.Sign(totalDistance) != Math.Sign(distance))
                {
                    distance *= -1;
                }
                if (adjust && Math.Abs(totalDistance) < Math.Abs(distance))
                {
                    distance = totalDistance;
                }

                return new Point(pStart.X, pStart.Y + distance);
            }

            double a = linearProperties.Item1;
            double b = linearProperties.Item2;
            double x1 = pStart.X;
            double y1 = pStart.Y;
            var quadraticSolution = ResolveQuadraticEquation(a+1, -1*((2*x1)+(2*a*b)-(2*a*y1)), (x1*x1)+(b*b)-(2*b*y1)+(y1*y1)-(distance*distance));

            double xn = quadraticSolution.Item1 < 0 ? quadraticSolution.Item2 : quadraticSolution.Item1;
            double yn = (a * xn) + b;

            return new Point(xn, yn);
        }
    }
}
