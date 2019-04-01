using RPG4.Abstraction.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RPG4.Abstraction
{
    /// <summary>
    /// Represents a path.
    /// </summary>
    public class Path
    {
        private List<PathStep> _steps;
        private int _currentStepIndex;
        private bool _reverse;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="points">Sorted permanent points collection.</param>
        public Path(params Point[] points)
        {
            _steps = points.Select(p => new PathStep(p, true)).ToList();
            _currentStepIndex = 0;
            _reverse = false;
        }

        /// <summary>
        /// Computes the next step.
        /// </summary>
        /// <param name="owner">The path owner.</param>
        /// <param name="nextX">Next X position.</param>
        /// <param name="nextY">Next Y position.</param>
        /// <param name="screen">The relative <see cref="Screen"/>.</param>
        /// <returns><c>True</c> if the path owner can move to specified coordinates; <c>False</c> otherwise.</returns>
        public bool ComputeNextStep(Sprite owner, double nextX, double nextY, Screen screen)
        {
            Sprite ownerCopy = owner.CopyToPosition(new Point(nextX, nextY));

            var overlapStructs = screen.Structures.Where(s => s.Overlap(ownerCopy)).ToList();
            if (overlapStructs.Count > 0)
            {
                _reverse = true;
                SetNextStepIndex();
                /*if (overlapStructs.Count > 1)
                {
                    // If there're more than one structure overlaped
                    _reverse = true;
                    SetNextStepIndex();
                }
                else
                {
                    var overlapStruct = overlapStructs.First();

                    ComputeNewPath(owner, screen, ownerCopy, overlapStruct);
                }*/
                return false;
            }

            // Checks if the current step is crossed.
            if (owner.X < _steps[_currentStepIndex].Point.X && nextX >= _steps[_currentStepIndex].Point.X
                || owner.Y < _steps[_currentStepIndex].Point.Y && nextY >= _steps[_currentStepIndex].Point.Y
                || owner.X > _steps[_currentStepIndex].Point.X && nextX <= _steps[_currentStepIndex].Point.X
                || owner.Y > _steps[_currentStepIndex].Point.Y && nextY <= _steps[_currentStepIndex].Point.Y)
            {
                // Computes the next step index.
                int previousStepIndex = _currentStepIndex;
                SetNextStepIndex();

                // Remove the former step if it wasn't permanent.
                if (!_steps[previousStepIndex].Permanent)
                {
                    _steps.RemoveAt(previousStepIndex);
                    if (_currentStepIndex > previousStepIndex)
                    {
                        _currentStepIndex--;
                    }
                }
            }

            return true;
        }

        // Tries to compute a new path when overlaping a structure; doesn't work so far.
        private void ComputeNewPath(Sprite owner, Screen screen, Sprite ownerCopy, Sprite overlapStruct)
        {
            double distanceToX1 = Math.Abs(overlapStruct.X - owner.X);
            double distanceToX2 = Math.Abs(overlapStruct.BottomRightX - owner.X);
            double distanceToY1 = Math.Abs(overlapStruct.Y - owner.Y);
            double distanceToY2 = Math.Abs(overlapStruct.BottomRightY - owner.Y);

            Directions dir = overlapStruct.OverlapDirection(ownerCopy, owner).Value;
            Point pt = new Point();
            switch (dir)
            {
                case Directions.bottom:
                    pt = new Point(distanceToX1 < distanceToX2 ? overlapStruct.X : overlapStruct.BottomRightX, overlapStruct.BottomRightY);
                    break;
                case Directions.top:
                    pt = new Point(distanceToX1 < distanceToX2 ? overlapStruct.X : overlapStruct.BottomRightX, overlapStruct.Y);
                    break;
                case Directions.left:
                    pt = new Point(overlapStruct.X, distanceToY1 < distanceToY2 ? overlapStruct.Y : overlapStruct.BottomRightY);
                    break;
                case Directions.right:
                    pt = new Point(overlapStruct.BottomRightX, distanceToY1 < distanceToY2 ? overlapStruct.Y : overlapStruct.BottomRightY);
                    break;
                case Directions.bottom_left:
                    if (distanceToY1 < distanceToX2)
                    {
                        pt = new Point(overlapStruct.X, overlapStruct.Y);
                    }
                    else
                    {
                        pt = new Point(overlapStruct.BottomRightX, overlapStruct.BottomRightY);
                    }
                    break;
                case Directions.bottom_right:
                    if (distanceToY1 < distanceToX1)
                    {
                        pt = new Point(overlapStruct.BottomRightX, overlapStruct.Y);
                    }
                    else
                    {
                        pt = new Point(overlapStruct.X, overlapStruct.BottomRightY);
                    }
                    break;
                case Directions.top_left:
                    if (distanceToY2 < distanceToX2)
                    {
                        pt = new Point(overlapStruct.X, overlapStruct.BottomRightY);
                    }
                    else
                    {
                        pt = new Point(overlapStruct.BottomRightX, overlapStruct.Y);
                    }
                    break;
                case Directions.top_right:
                    if (distanceToY2 < distanceToX1)
                    {
                        pt = new Point(overlapStruct.BottomRightX, overlapStruct.BottomRightY);
                    }
                    else
                    {
                        pt = new Point(overlapStruct.X, overlapStruct.Y);
                    }
                    break;
            }
            SetNextStepIndex();
            _steps.Insert(_currentStepIndex, new PathStep(pt, screen.PermanentStructures.Contains(overlapStruct)));
        }

        // Computes and sets the next step index.
        private void SetNextStepIndex()
        {
            _currentStepIndex = _reverse ?
                (_currentStepIndex - 1 < 0 ? _steps.Count - 1 : _currentStepIndex - 1)
                : (_currentStepIndex + 1 == _steps.Count ? 0 : _currentStepIndex + 1);
        }

        /// <summary>
        /// Reverses <see cref="_reverse"/> value.
        /// </summary>
        public void ReversePath()
        {
            _reverse = !_reverse;
        }

        /// <summary>
        /// Gets the current step point.
        /// </summary>
        /// <returns><see cref="Point"/></returns>
        public Point GetCurrentStep()
        {
            return _steps[_currentStepIndex].Point;
        }
    }
}
