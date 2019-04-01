namespace RPG4.Abstraction
{
    /// <summary>
    /// Represents the keayboard input for a frame inside the <see cref="Engine"/>
    /// </summary>
    public class KeyPress
    {
        /// <summary>
        /// Indicates if the up button is pressed.
        /// </summary>
        public bool PressUp { get; private set; }
        /// <summary>
        /// Indicates if the down button is pressed.
        /// </summary>
        public bool PressDown { get; private set; }
        /// <summary>
        /// Indicates if the right button is pressed.
        /// </summary>
        public bool PressRight { get; private set; }
        /// <summary>
        /// Indicates if the left button is pressed.
        /// </summary>
        public bool PressLeft { get; private set; }
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
        /// Constructor.
        /// </summary>
        /// <param name="up"><see cref="PressUp"/></param>
        /// <param name="down"><see cref="PressDown"/></param>
        /// <param name="right"><see cref="PressRight"/></param>
        /// <param name="left"><see cref="PressLeft"/></param>
        /// <param name="hit"><see cref="PressHit"/></param>
        /// <param name="action"><see cref="PressAction"/></param>
        /// <param name="inventorySlotId"><see cref="InventorySlotId"/></param>
        public KeyPress(bool up, bool down, bool right, bool left, bool hit, bool action, int? inventorySlotId)
        {
            PressUp = up;
            PressDown = down;
            PressRight = right;
            PressLeft = left;
            PressHit = hit;
            PressAction = action;
            InventorySlotId = inventorySlotId;

            // up and down both pressed cancel each other
            if (PressUp && PressDown)
            {
                PressDown = false;
                PressUp = false;
            }

            // right and left both pressed cancel each other
            if (PressRight && PressLeft)
            {
                PressRight = false;
                PressLeft = false;
            }
        }
    }
}
