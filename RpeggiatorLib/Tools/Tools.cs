﻿using System;
using System.IO;
using System.Windows.Media;
using Newtonsoft.Json;

namespace RpeggiatorLib
{
    /// <summary>
    /// Tool methods. 
    /// </summary>
    internal static class Tools
    {
        private static Random _random = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// Gets the full path of an image from its name without extension and the specified resource directory.
        /// </summary>
        /// <param name="resourcesDirectory">Resources directory.</param>
        /// <param name="imageNameWithoutExtension">Image name without extension.</param>
        /// <returns>Image full path.</returns>
        internal static string GetImagePath(string resourcesDirectory, string imageNameWithoutExtension)
        {
            return string.Format("{0}Images\\{1}.png", resourcesDirectory, imageNameWithoutExtension);
        }

        /// <summary>
        /// Transforms a <see cref="Color"/> to its hexadecimal representation.
        /// </summary>
        /// <param name="color"><see cref="Color"/></param>
        /// <returns>hexadecimal color value.</returns>
        internal static string HexFromColor(Color color)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// Computes the distance made in diagonal relatively to the distance made straightforward.
        /// </summary>
        /// <param name="frameDistance">Distance made straightforward.</param>
        /// <returns>Diagonal distance.</returns>
        internal static double FrameDiagonalDistance(double frameDistance)
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
        internal static Tuple<double, double> ResolveQuadraticEquation(double a, double b, double c)
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
        internal static Tuple<double, double> GetLinearFunctionFromPoints(Point p1, Point p2)
        {
            if (p2.X.Equal(p1.X))
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
        internal static Point GetPointOnLine(Point pStart, Point pDest, double d, bool shrinkToDestination = false)
        {
            if (d == 0)
            {
                return pStart;
            }

            Tuple<double, double> linearProperties = GetLinearFunctionFromPoints(pStart, pDest);

            if (linearProperties == null)
            {
                double totalDistance = pDest.Y - pStart.Y;
                if (Math.Sign(totalDistance) != Math.Sign(d))
                {
                    d *= -1;
                }
                if (shrinkToDestination && Math.Abs(totalDistance).Lower(Math.Abs(d)))
                {
                    d = totalDistance;
                }

                return new Point(pStart.X, pStart.Y + d);
            }

            double a = linearProperties.Item1;
            double b = linearProperties.Item2;
            double x1 = pStart.X;
            double y1 = pStart.Y;
            Tuple<double, double> quadraticSolution = ResolveQuadraticEquation(
                1 + a * a,
                -2 * x1 + 2 * a * b - 2 * a * y1,
                x1 * x1 + b * b - 2 * b * y1 + y1 * y1 - d * d
            );

            if (quadraticSolution == null)
            {
                //throw new Exceptions.NoQuadraticSolutionException(pStart, pDest, d);
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
                if ((x.Greater(pDest.X) && pDest.X.Greater(pStart.X)) || (x.Lower(pDest.X) && pDest.X.Lower(pStart.X)))
                {
                    x = pDest.X;
                }
                if ((y.Greater(pDest.Y) && pDest.Y.Greater(pStart.Y)) || (y.Lower(pDest.Y) && pDest.Y.Lower(pStart.Y)))
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
        internal static int GetRandomNumber(int minIncluded, int maxExcluded)
        {
            return _random.Next(minIncluded, maxExcluded);
        }

        /// <summary>
        /// Checks equality between two <see cref="double"/> (with the margin of error <see cref="Constants.TYPE_DOUBLE_COMPARISON_TOLERANCE"/>).
        /// </summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <returns><c>True</c> if equals; <c>False</c> otherwise.</returns>
        internal static bool Equal(this double a, double b)
        {
            return Math.Abs(a - b) < Constants.TYPE_DOUBLE_COMPARISON_TOLERANCE;
        }

        /// <summary>
        /// Checks if a <see cref="double"/> is lower than another (with the margin of error <see cref="Constants.TYPE_DOUBLE_COMPARISON_TOLERANCE"/>).
        /// </summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <returns><c>True</c> if <paramref name="a"/> is lower; <c>False</c> otherwise.</returns>
        internal static bool Lower(this double a, double b)
        {
            return a < b && Math.Abs(a - b) >= Constants.TYPE_DOUBLE_COMPARISON_TOLERANCE;
        }

        /// <summary>
        /// Checks if a <see cref="double"/> is greater than another (with the margin of error <see cref="Constants.TYPE_DOUBLE_COMPARISON_TOLERANCE"/>).
        /// </summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <returns><c>True</c> if <paramref name="a"/> is greater; <c>False</c> otherwise.</returns>
        internal static bool Greater(this double a, double b)
        {
            return a > b && Math.Abs(a - b) >= Constants.TYPE_DOUBLE_COMPARISON_TOLERANCE;
        }

        /// <summary>
        /// Checks if a <see cref="double"/> is lower or equals than another (with the margin of error <see cref="Constants.TYPE_DOUBLE_COMPARISON_TOLERANCE"/>).
        /// </summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <returns><c>True</c> if <paramref name="a"/> is lower or equals; <c>False</c> otherwise.</returns>
        internal static bool LowerEqual(this double a, double b)
        {
            return a.Equal(b) || a.Lower(b);
        }

        /// <summary>
        /// Checks if a <see cref="double"/> is greater or equals than another (with the margin of error <see cref="Constants.TYPE_DOUBLE_COMPARISON_TOLERANCE"/>).
        /// </summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <returns><c>True</c> if <paramref name="a"/> is greater or equals; <c>False</c> otherwise.</returns>
        internal static bool GreaterEqual(this double a, double b)
        {
            return a.Equal(b) || a.Greater(b);
        }

        /// <summary>
        /// Computes, for a given <see cref="Enums.Direction"/>, distance, and starting position, the next position.
        /// </summary>
        /// <param name="x">Starting position X-axis.</param>
        /// <param name="y">Starting position Y-axis.</param>
        /// <param name="distance">Distance to do (pixels).</param>
        /// <param name="direction"><see cref="Enums.Direction"/>; can be <c>Null</c> (no move).</param>
        /// <returns>The next position.</returns>
        internal static Point ComputeMovementNextPointInDirection(double x, double y, double distance, Enums.Direction? direction)
        {
            // Shortcut.
            KeyPress keys = Engine.Default.KeyPress;

            switch (direction)
            {
                case Enums.Direction.Bottom:
                    y += distance;
                    break;
                case Enums.Direction.BottomLeft:
                    y += FrameDiagonalDistance(distance);
                    x -= FrameDiagonalDistance(distance);
                    break;
                case Enums.Direction.BottomRight:
                    y += FrameDiagonalDistance(distance);
                    x += FrameDiagonalDistance(distance);
                    break;
                case Enums.Direction.Top:
                    y -= distance;
                    break;
                case Enums.Direction.TopLeft:
                    y -= FrameDiagonalDistance(distance);
                    x -= FrameDiagonalDistance(distance);
                    break;
                case Enums.Direction.TopRight:
                    y -= FrameDiagonalDistance(distance);
                    x += FrameDiagonalDistance(distance);
                    break;
                case Enums.Direction.Left:
                    x -= distance;
                    break;
                case Enums.Direction.Right:
                    x += distance;
                    break;
            }

            return new Point(x, y);
        }

        /// <summary>
        /// Checks if two <see cref="Enums.Direction"/> are opposite.
        /// </summary>
        /// <param name="dir1"><see cref="Enums.Direction"/></param>
        /// <param name="dir2"><see cref="Enums.Direction"/></param>
        /// <returns><c>True</c> if specified directions are opposite; <c>False</c> otherwise.</returns>
        internal static bool AreOppositeDirections(Enums.Direction dir1, Enums.Direction dir2)
        {
            return (int)dir1 + (int)dir2 == 9;
        }

        /// <summary>
        /// Checks if two <see cref="Enums.Direction"/> are close.
        /// </summary>
        /// <param name="dir1"><see cref="Enums.Direction"/></param>
        /// <param name="dir2"><see cref="Enums.Direction"/></param>
        /// <returns><c>True</c> if specified directions are close; <c>False</c> otherwise.</returns>
        internal static bool AreCloseDirections(Enums.Direction dir1, Enums.Direction dir2)
        {
            if (dir1 == dir2)
            {
                return true;
            }

            switch (dir1)
            {
                case Enums.Direction.Bottom:
                    return dir2 == Enums.Direction.BottomLeft || dir2 == Enums.Direction.BottomRight;
                case Enums.Direction.BottomLeft:
                    return dir2 == Enums.Direction.Bottom || dir2 == Enums.Direction.Left;
                case Enums.Direction.BottomRight:
                    return dir2 == Enums.Direction.Bottom || dir2 == Enums.Direction.Right;
                case Enums.Direction.Left:
                    return dir2 == Enums.Direction.TopLeft || dir2 == Enums.Direction.BottomLeft;
                case Enums.Direction.Right:
                    return dir2 == Enums.Direction.TopRight || dir2 == Enums.Direction.BottomRight;
                case Enums.Direction.Top:
                    return dir2 == Enums.Direction.TopLeft || dir2 == Enums.Direction.TopRight;
                case Enums.Direction.TopLeft:
                    return dir2 == Enums.Direction.Top || dir2 == Enums.Direction.Left;
                case Enums.Direction.TopRight:
                    return dir2 == Enums.Direction.Top || dir2 == Enums.Direction.Right;
            }
            return false;
        }
    }
}
