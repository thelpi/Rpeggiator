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
                if (overlapStructs.Count > 1 || !ComputeStepsToAvoidStructure(owner, screen, ownerCopy, overlapStructs.First()))
                {
                    // There're more than one structure overlaped or unable to determinate a path.
                    _reverse = true;
                    _currentStepIndex = GetNextStepIndex();
                }
                return false;
            }

            if (CheckCurrentStepIsCrossed(owner, nextX, nextY))
            {
                _currentStepIndex = GetNextStepIndex();
                CleanTemporarySteps();
            }

            return true;
        }

        private void CleanTemporarySteps()
        {
            PathStep step = _steps[_currentStepIndex];
            if (step.Permanent)
            {
                _steps.RemoveAll(ps => !ps.Permanent);
                _currentStepIndex = _steps.IndexOf(step);
                _getAroundByRightAndBottom = null;
            }
        }

        private bool CheckCurrentStepIsCrossed(Sprite owner, double nextX, double nextY)
        {
            return owner.X < _steps[_currentStepIndex].Point.X && nextX >= _steps[_currentStepIndex].Point.X
                            || owner.Y < _steps[_currentStepIndex].Point.Y && nextY >= _steps[_currentStepIndex].Point.Y
                            || owner.X > _steps[_currentStepIndex].Point.X && nextX <= _steps[_currentStepIndex].Point.X
                            || owner.Y > _steps[_currentStepIndex].Point.Y && nextY <= _steps[_currentStepIndex].Point.Y;
        }
        
        private bool ComputeStepsToAvoidStructure(Sprite owner, Screen screen, Sprite ownerCopy, Sprite overlapStruct)
        {
            double distanceToX1 = Math.Abs(overlapStruct.X - owner.X);
            double distanceToX2 = Math.Abs(overlapStruct.BottomRightX - owner.X);
            double distanceToY1 = Math.Abs(overlapStruct.Y - owner.Y);
            double distanceToY2 = Math.Abs(overlapStruct.BottomRightY - owner.Y);

            Directions dir = overlapStruct.OverlapDirection(ownerCopy, owner).Value;
            bool validPath = false;
            int attemps = 0;
            double newX1 = 0, newX2 = 0, newY1 = 0, newY2 = 0;
            while (!validPath && attemps < 2)
            {
                newX1++;
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
                validPath = !(newX1 < 0 || newX2 > screen.Width || newY1 < 0 || newY2 > screen.Height);
                if (!validPath)
                {
                    _getAroundByRightAndBottom = !_getAroundByRightAndBottom.Value;
                }
            }
            if (validPath)
            {
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
            return validPath;
        }

        private int GetNextStepIndex()
        {
            return _reverse ?
                (_currentStepIndex - 1 < 0 ? _steps.Count - 1 : _currentStepIndex - 1) :
                (_currentStepIndex + 1 == _steps.Count ? 0 : _currentStepIndex + 1);
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
