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
        private bool? _getAroundByRightAndBottom = null;

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
                if (overlapStructs.Count > 1)
                {
                    // If there're more than one structure overlaped
                    _reverse = true;
                    _currentStepIndex = GetNextStepIndex();
                }
                else
                {
                    ComputeNewPath(owner, screen, ownerCopy, overlapStructs.First());
                }
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
                _currentStepIndex = GetNextStepIndex();

                // If a step "original" is started
                PathStep step = _steps[_currentStepIndex];
                if (step.Permanent)
                {
                    _steps.RemoveAll(ps => !ps.Permanent);
                    _currentStepIndex = _steps.IndexOf(step);
                    _getAroundByRightAndBottom = null;
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
            double newX1 = 0, newX2 = 0, newY1 = 0, newY2 = 0;
            if (!_getAroundByRightAndBottom.HasValue)
            {
                _getAroundByRightAndBottom = Tools.GetRandomNumber(0, 2) == 0;
            }
            switch (dir)
            {
                case Directions.bottom:
                    newX1 = _getAroundByRightAndBottom.Value ?
                        overlapStruct.BottomRightX + Constants.NPC_GETAROUND_MOVE_MARGIN
                        : overlapStruct.X - owner.Width - Constants.NPC_GETAROUND_MOVE_MARGIN;
                    newX2 = newX1;
                    newY1 = overlapStruct.BottomRightY + Constants.NPC_GETAROUND_MOVE_MARGIN;
                    newY2 = overlapStruct.Y - owner.Height - Constants.NPC_GETAROUND_MOVE_MARGIN;
                    break;
                case Directions.top:
                    newX1 = _getAroundByRightAndBottom.Value ?
                        overlapStruct.BottomRightX + Constants.NPC_GETAROUND_MOVE_MARGIN
                        : overlapStruct.X - owner.Width - Constants.NPC_GETAROUND_MOVE_MARGIN;
                    newX2 = newX1;
                    newY1 = overlapStruct.Y - owner.Height - Constants.NPC_GETAROUND_MOVE_MARGIN;
                    newY2 = overlapStruct.BottomRightY + Constants.NPC_GETAROUND_MOVE_MARGIN;
                    break;
                case Directions.left:
                    newX1 = overlapStruct.X - owner.Width - Constants.NPC_GETAROUND_MOVE_MARGIN;
                    newY1 = _getAroundByRightAndBottom.Value ?
                        overlapStruct.BottomRightY + Constants.NPC_GETAROUND_MOVE_MARGIN
                        : overlapStruct.Y - owner.Height - Constants.NPC_GETAROUND_MOVE_MARGIN;
                    newX2 = overlapStruct.BottomRightX + Constants.NPC_GETAROUND_MOVE_MARGIN;
                    newY2 = newY1;
                    break;
                case Directions.right:
                    newX1 = overlapStruct.BottomRightX + Constants.NPC_GETAROUND_MOVE_MARGIN;
                    newY1 = _getAroundByRightAndBottom.Value ?
                        overlapStruct.BottomRightY + Constants.NPC_GETAROUND_MOVE_MARGIN
                        : overlapStruct.Y - owner.Height - Constants.NPC_GETAROUND_MOVE_MARGIN;
                    newX2 = overlapStruct.X - owner.Width - Constants.NPC_GETAROUND_MOVE_MARGIN;
                    newY2 = newY1;
                    break;
            }
            if (!_reverse)
            {
                _steps.Insert(_currentStepIndex, new PathStep(new Point(newX1, newY1), false));
                _steps.Insert(_currentStepIndex + 1, new PathStep(new Point(newX2, newY2), false));
            }
            else
            {
                _steps.Insert(_currentStepIndex, new PathStep(new Point(newX2, newY2), false));
                _steps.Insert(_currentStepIndex + 1, new PathStep(new Point(newX1, newY1), false));
                _currentStepIndex++;
            }
        }

        /// <summary>
        /// Computes the next step index.
        /// </summary>
        /// <param name="fromStep">Optionnal; previous step index.
        /// Takes <see cref="_currentStepIndex"/> if not specified or less than 0.</param>
        /// <returns>Next step index.</returns>
        private int GetNextStepIndex(int fromStep = -1)
        {
            fromStep = fromStep < 0 ? _currentStepIndex : fromStep;

            return _reverse ?
                (fromStep - 1 < 0 ? _steps.Count - 1 : fromStep - 1) :
                (fromStep + 1 == _steps.Count ? 0 : fromStep + 1);
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
