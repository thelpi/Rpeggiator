using RPG4.Abstraction.Sprites;
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

            Sprite overlapStruct = screen.Structures.FirstOrDefault(s => s.Overlap(ownerCopy));
            if (overlapStruct != null)
            {
                Point pt = new Point();
                SetNextStepIndex();
                _steps.Insert(_currentStepIndex, new PathStep(pt, screen.PermanentStructures.Contains(overlapStruct)));
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
