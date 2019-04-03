using Newtonsoft.Json;
using RPG4.Properties;
using System;
using System.Text;
using System.Windows;

namespace RPG4
{
    /// <summary>
    /// Tool methods. 
    /// </summary>
    public static class Tools
    {
        private static Random _random = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// Gets the screen datas by its index.
        /// </summary>
        /// <param name="screenIndex">Screen index.</param>
        /// <returns>Dynamic screen datas.</returns>
        public static dynamic GetScreenDatasFromIndex(int screenIndex)
        {
            var jsonBytes = (byte[])Resources.ResourceManager.GetObject(string.Format("Screen{0}", screenIndex));
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(jsonBytes));
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
            double delta = (b*b)-(4*a*c);

            return delta < 0 ? null : new Tuple<double, double>(
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
        /// <param name="d">The distance, in pixels.</param>
        /// <param name="shrinkToDestination">Optionnal; <c>True</c> to not go further than <paramref name="pDest"/>; <c>False</c> otherwise.</param>
        /// <returns>Coordinates of the point.</returns>
        public static Point GetPointOnLine(Point pStart, Point pDest, double d, bool shrinkToDestination = false)
        {
            if (d == 0)
            {
                return pStart;
            }

            var linearProperties = GetLinearFunctionFromPoints(pStart, pDest);

            if (linearProperties == null)
            {
                double totalDistance = pDest.Y - pStart.Y;
                if (Math.Sign(totalDistance) != Math.Sign(d))
                {
                    d *= -1;
                }
                if (shrinkToDestination && Math.Abs(totalDistance) < Math.Abs(d))
                {
                    d = totalDistance;
                }

                return new Point(pStart.X, pStart.Y + d);
            }

            double a = linearProperties.Item1;
            double b = linearProperties.Item2;
            double x1 = pStart.X;
            double y1 = pStart.Y;
            var quadraticSolution = ResolveQuadraticEquation(
                1 + a * a,
                -2 * x1 + 2 * a * b - 2 * a * y1,
                x1 * x1 + b * b - 2 * b * y1 + y1 * y1 - d * d
            );

            if (quadraticSolution == null)
            {
                // TODO : log this case
                return pStart;
            }

            double xn_s1 = quadraticSolution.Item1;
            double yn_s1 = (a * xn_s1) + b;

            double xn_s2 = quadraticSolution.Item2;
            double yn_s2 = (a * xn_s2) + b;

            double x = xn_s1 >= 0 ? xn_s1 : xn_s2;
            double y = xn_s1 >= 0 ? yn_s1 : yn_s2;
            if (xn_s1 >= 0 && xn_s2 >= 0)
            {
                if (yn_s1 < 0 || yn_s2 < 0)
                {
                    x = yn_s1 < 0 ? xn_s2 : xn_s1;
                    y = yn_s1 < 0 ? yn_s2 : yn_s1;
                }
                else
                {
                    double gapX1 = Math.Abs(xn_s1 - pDest.X);
                    double gapX2 = Math.Abs(xn_s2 - pDest.X);
                    double gapY1 = Math.Abs(yn_s1 - pDest.Y);
                    double gapY2 = Math.Abs(yn_s2 - pDest.Y);
                    if (gapX1 + gapY1 < gapX2 + gapY2)
                    {
                        x = xn_s1;
                        y = yn_s1;
                    }
                    else
                    {
                        x = xn_s2;
                        y = yn_s2;
                    }
                }
            }

            if (shrinkToDestination)
            {
                if ((x > pDest.X && pDest.X > pStart.X) || (x < pDest.X && pDest.X < pStart.X))
                {
                    x = pDest.X;
                }
                if ((y > pDest.X && pDest.Y > pStart.Y) || (y < pDest.Y && pDest.Y < pStart.Y))
                {
                    y = pDest.Y;
                }
            }

            return new Point(x, y);
        }

        /// <summary>
        /// Gets a random <see cref="int"/> inside the specified range.
        /// </summary>
        /// <param name="minIncluded">Minimal value (included).</param>
        /// <param name="maxExcluded">Maximal value (excluded).</param>
        /// <returns>Random <see cref="int"/>.</returns>
        public static int GetRandomNumber(int minIncluded, int maxExcluded)
        {
            return _random.Next(minIncluded, maxExcluded);
        }

        /// <summary>
        /// Gets a random <see cref="double"/> between 0 (included) and 1 (excluded).
        /// </summary>
        /// <returns>Random <see cref="double"/>.</returns>
        public static double GetRandomNumber()
        {
            return _random.NextDouble();
        }
    }
}
