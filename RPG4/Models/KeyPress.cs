using System.Linq;

namespace RPG4.Models
{
    /// <summary>
    /// Represents the keayboard input for a frame inside the <see cref="Engine"/>
    /// </summary>
    public class KeyPress
    {
        /// <summary>
        /// Indicates the direction if arrow keys are pressed.
        /// </summary>
        public DirectionEnum? Direction { get; private set; }
        /// <summary>
        /// Indicates if the hit button is pressed.
        /// </summary>
        public bool PressHit { get; private set; }
        /// <summary>
        /// Indicates if the action button is pressed.
        /// </summary>
        public bool PressAction { get; private set; }
        /// <summary>
        /// Indicates the inventory slot pressed.
        /// </summary>
        public int? InventorySlotId { get; private set; }
        /// <summary>
        /// Inferred; indicates if the general direction of pressed keys is left.
        /// </summary>
        /// <returns><c>True</c> if it's left; <c>False</c> otherwise.</returns>
        public bool GoLeft
        {
            get
            {
                return GoDirection(DirectionEnum.Left, DirectionEnum.TopLeft, DirectionEnum.BottomLeft);
            }
        }
        /// <summary>
        /// Inferred; indicates if the general direction of pressed keys is right.
        /// </summary>
        /// <returns><c>True</c> if it's right; <c>False</c> otherwise.</returns>
        public bool GoRight
        {
            get
            {
                return GoDirection(DirectionEnum.Right, DirectionEnum.TopRight, DirectionEnum.BottomRight);
            }
        }
        /// <summary>
        /// Inferred; indicates if the general direction of pressed keys is up.
        /// </summary>
        /// <returns><c>True</c> if it's up; <c>False</c> otherwise.</returns>
        public bool GoUp
        {
            get
            {
                return GoDirection(DirectionEnum.Top, DirectionEnum.TopLeft, DirectionEnum.TopRight);
            }
        }
        /// <summary>
        /// Inferred; indicates if the general direction of pressed keys is bottom.
        /// </summary>
        /// <returns><c>True</c> if it's bottom; <c>False</c> otherwise.</returns>
        public bool GoBottom
        {
            get
            {
                return GoDirection(DirectionEnum.Bottom, DirectionEnum.BottomLeft, DirectionEnum.BottomRight);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="up">Indicates if up key is pressed.</param>
        /// <param name="down">Indicates if down key is pressed.</param>
        /// <param name="right">Indicates if right key is pressed.</param>
        /// <param name="left">Indicates if left key is pressed.</param>
        /// <param name="hit"><see cref="PressHit"/></param>
        /// <param name="action"><see cref="PressAction"/></param>
        /// <param name="inventorySlotId"><see cref="InventorySlotId"/></param>
        public KeyPress(bool up, bool down, bool right, bool left, bool hit, bool action, int? inventorySlotId)
        {
            PressHit = hit;
            PressAction = action;
            InventorySlotId = inventorySlotId;

            // up and down both pressed cancel each other
            if (up && down)
            {
                down = false;
                up = false;
            }

            // right and left both pressed cancel each other
            if (right && left)
            {
                right = false;
                left = false;
            }

            if (up)
            {
                Direction = DirectionEnum.Top;
                if (right)
                {
                    Direction = DirectionEnum.TopRight;
                }
                else if (left)
                {
                    Direction = DirectionEnum.TopLeft;
                }
            }
            else if (down)
            {
                Direction = DirectionEnum.Bottom;
                if (right)
                {
                    Direction = DirectionEnum.BottomRight;
                }
                else if (left)
                {
                    Direction = DirectionEnum.BottomLeft;
                }
            }
            else if (right)
            {
                Direction = DirectionEnum.Right;
            }
            else if (left)
            {
                Direction = DirectionEnum.Left;
            }
        }

        // Indicates if the general direction is one of the input array.
        private bool GoDirection(params DirectionEnum[] dirs)
        {
            return Direction.HasValue && dirs.Contains(Direction.Value);
        }
    }
}
