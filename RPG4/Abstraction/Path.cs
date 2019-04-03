﻿using RPG4.Abstraction.Sprites;
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
        private bool? _counterClockGetAround = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="points">Sorted permanent points collection.</param>
        public Path(params Point[] points)
        {
            _steps = points.Select(p => new PathStep(p, true)).ToList();
            _currentStepIndex = 1;
        }

        /// <summary>
        /// Computes, for a given <see cref="Sprite"/>, the next position.
        /// </summary>
        /// <param name="owner"><see cref="Sprite"/></param>
        /// <param name="distance">Distance, in pixels.</param>
        /// <returns><see cref="Point"/></returns>
        public Point ComputeNextPosition(Sprite owner, double distance)
        {
            // Gets the theoretical point. 
            Point newPoint = Tools.GetPointOnLine(owner.TopLeftCorner, _steps[_currentStepIndex].Point, distance, true);

            if (CheckNewPointValidityInContext(owner, newPoint))
            {
                return newPoint;
            }
            else
            {
                // Don't make any move this time.
                return new Point(owner.X, owner.Y);
            }
        }

        /// <summary>
        /// Reverses path steps.
        /// </summary>
        public void ReversePath()
        {
            _steps.Reverse();
            _currentStepIndex = _steps.Count - 1 - _currentStepIndex;
        }

        // Checks if the specified point is a valid movement.
        private bool CheckNewPointValidityInContext(Sprite owner, Point nextPt)
        {
            // Checks if the new point overlap a structure (or more) of the screen.
            var overlapStructs = Engine.Default.CurrentScreen.Structures.Where(s => s.Overlap(owner.CopyToPosition(nextPt))).ToList();
            if (overlapStructs.Count > 0)
            {
                if (overlapStructs.Count > 1 || !ComputeExtraStepToAvoidStructure(owner, nextPt, overlapStructs.First()))
                {
                    // There're more than one structure overlaped
                    // or unable to determinate a path.
                    ReversePath();
                    _currentStepIndex = GetNextStepIndex();
                }
                // In any case, no move this time.
                return false;
            }
            
            if (CheckCurrentStepIsCrossed(owner, nextPt.X, nextPt.Y))
            {
                _currentStepIndex = GetNextStepIndex();
                CleanTemporarySteps();
            }
            // The owner is an enemy and has the player in his line of sight
            else if (owner.GetType() == typeof(Enemy)
                && owner.CopyToPosition(nextPt).Overlap(Engine.Default.Player.ResizeToRatio(Constants.PLAYER_SIZE_RATIO_TO_TRIGGER_ENEMY)))
            {
                // Inserts a pursue step.
                _steps.Insert(_currentStepIndex, PathStep.CreatePursueStep(Engine.Default.Player));
                CleanTemporarySteps();
            }

            return true;
        }

        // Cleans every temporary steps, except the current one.
        private void CleanTemporarySteps()
        {
            PathStep previousStep = _steps[GetPreviousStepIndex()];
            PathStep step = _steps[_currentStepIndex];
            _steps.RemoveAll(ps => !ps.Permanent && !ps.Equals(step));
            _currentStepIndex = _steps.IndexOf(step);
            _counterClockGetAround = step.Permanent && previousStep.Permanent || step.Pursue ? null : _counterClockGetAround;
        }

        // Checks if the current step is reached / crossed.
        private bool CheckCurrentStepIsCrossed(Sprite owner, double nextX, double nextY)
        {
            return owner.CheckPointIsCrossed(_steps[_currentStepIndex].Point, nextX, nextY);
        }
        
        // Computes a (potential) new step to avoid a structure.
        private bool ComputeExtraStepToAvoidStructure(Sprite owner, Point newPosition, Sprite overlapStruct)
        {
            // Ensure a direction to get around.
            _counterClockGetAround = _counterClockGetAround ?? Tools.GetRandomNumber(0, 2) == 0;

            // Gets the direction the sprite comes from, relative to the structure.
            Directions generalDirection = overlapStruct.DirectionSourceOfOverlap(owner, newPosition).Value;
            AdjustOverlapDirectionFromGetAroundDirection(ref generalDirection);

            Point tmpPt = new Point();

            // tries two times, for each possible value of "_counterClockGetAround".
            bool validPath = false;
            int attemps = 0;
            while (!validPath && attemps < 2)
            {
                switch (generalDirection)
                {
                    case Directions.top:
                        tmpPt.X = _counterClockGetAround.Value ? overlapStruct.X - owner.Width : overlapStruct.BottomRightX;
                        tmpPt.Y = overlapStruct.Y - owner.Height;
                        break;
                    case Directions.bottom:
                        tmpPt.X = _counterClockGetAround.Value ? overlapStruct.BottomRightX : overlapStruct.X - owner.Width;
                        tmpPt.Y = overlapStruct.BottomRightY;
                        break;
                    case Directions.left:
                        tmpPt.X = overlapStruct.X - owner.Width;
                        tmpPt.Y = _counterClockGetAround.Value ? overlapStruct.BottomRightY : overlapStruct.Y - owner.Height;
                        break;
                    case Directions.right:
                        tmpPt.X = overlapStruct.BottomRightX;
                        tmpPt.Y = _counterClockGetAround.Value ? overlapStruct.Y - owner.Height : overlapStruct.BottomRightY;
                        break;
                }

                // The owner, at the path coordinates, should not cross the screen borders.
                validPath = Engine.Default.CurrentScreen.IsInside(owner.CopyToPosition(tmpPt));

                // If the path is invalid, retry in the opposite direction.
                if (!validPath)
                {
                    attemps++;
                    _counterClockGetAround = !_counterClockGetAround.Value;
                }
            }

            // if there's a path, the step is temporarely added to the list.
            if (validPath)
            {
                _steps.Insert(_currentStepIndex, new PathStep(tmpPt, false));
            }

            return validPath;
        }

        // Force a straightforward direction, depending on "_counterClockGetAround",  when the direction is a corner.
        private void AdjustOverlapDirectionFromGetAroundDirection(ref Directions direction)
        {
            if (!_counterClockGetAround.HasValue)
            {
                return;
            }

            switch (direction)
            {
                case Directions.bottom_left:
                    direction = _counterClockGetAround.Value ? Directions.bottom : Directions.left;
                    break;
                case Directions.bottom_right:
                    direction = _counterClockGetAround.Value ? Directions.right : Directions.bottom;
                    break;
                case Directions.top_left:
                    direction = _counterClockGetAround.Value ? Directions.left : Directions.top;
                    break;
                case Directions.top_right:
                    direction = _counterClockGetAround.Value ? Directions.top : Directions.right;
                    break;
            }
        }

        // Gets the next step index.
        private int GetNextStepIndex()
        {
            return _currentStepIndex + 1 == _steps.Count ? 0 : _currentStepIndex + 1;
        }

        // Gets the previous step index.
        private int GetPreviousStepIndex()
        {
            return _currentStepIndex - 1 < 0 ? _steps.Count - 1 : _currentStepIndex - 1;
        }
    }
}
