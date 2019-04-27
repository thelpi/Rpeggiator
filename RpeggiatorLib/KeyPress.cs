using System.Linq;
using RpeggiatorLib.Enums;

namespace RpeggiatorLib
{
    /// <summary>
    /// Represents the keayboard input for a frame inside the <see cref="Engine"/>
    /// </summary>
    public class KeyPress
    {
        /// <summary>
        /// Indicates the direction if arrow keys are pressed.
        /// </summary>
        public Direction? Direction { get; private set; }
        /// <summary>
        /// Indicates if the hit button is pressed.
        /// </summary>
        public bool PressHit { get; private set; }
        /// <summary>
        /// Indicates if the action button is pressed.
        /// </summary>
        public bool PressAction { get; private set; }
        /// <summary>
        /// Indicates if the shield button is pressed.
        /// </summary>
        public bool PressShield { get; private set; }
        /// <summary>
        /// Indicates the inventory slot index pressed. Starts to one.
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
                return GoDirection(Enums.Direction.Left, Enums.Direction.TopLeft, Enums.Direction.BottomLeft);
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
                return GoDirection(Enums.Direction.Right, Enums.Direction.TopRight, Enums.Direction.BottomRight);
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
                return GoDirection(Enums.Direction.Top, Enums.Direction.TopLeft, Enums.Direction.TopRight);
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
                return GoDirection(Enums.Direction.Bottom, Enums.Direction.BottomLeft, Enums.Direction.BottomRight);
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
        /// <param name="shield"><see cref="PressShield"/></param>
        /// <param name="inventorySlotId"><see cref="InventorySlotId"/></param>
        /// <exception cref="System.ArgumentException"><see cref="Messages.InvalidInventorySlotIdExceptionMessage"/></exception>
        public KeyPress(bool up, bool down, bool right, bool left, bool hit, bool action, bool shield, int? inventorySlotId)
        {
            if (inventorySlotId.HasValue && (inventorySlotId.Value < 0 || inventorySlotId.Value > Constants.Inventory.SLOT_COUNT))
            {
                throw new System.ArgumentException(Messages.InvalidInventorySlotIdExceptionMessage, nameof(inventorySlotId));
            }

            PressHit = hit;
            PressAction = action;
            // Can't shield while hitting.
            PressShield = shield && !hit;
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
                Direction = Enums.Direction.Top;
                if (right)
                {
                    Direction = Enums.Direction.TopRight;
                }
                else if (left)
                {
                    Direction = Enums.Direction.TopLeft;
                }
            }
            else if (down)
            {
                Direction = Enums.Direction.Bottom;
                if (right)
                {
                    Direction = Enums.Direction.BottomRight;
                }
                else if (left)
                {
                    Direction = Enums.Direction.BottomLeft;
                }
            }
            else if (right)
            {
                Direction = Enums.Direction.Right;
            }
            else if (left)
            {
                Direction = Enums.Direction.Left;
            }
        }

        // Indicates if the general direction is one of the input array.
        private bool GoDirection(params Direction[] dirs)
        {
            return Direction.HasValue && dirs.Contains(Direction.Value);
        }
    }
}
