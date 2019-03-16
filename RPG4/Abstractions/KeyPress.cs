namespace RPG4.Abstractions
{
    public class KeyPress
    {
        public bool PressUp { get; private set; }
        public bool PressDown { get; private set; }
        public bool PressRight { get; private set; }
        public bool PressLeft { get; private set; }
        public bool PressKick { get; private set; }

        public KeyPress(bool up, bool down, bool right, bool left, bool kick)
        {
            PressUp = up;
            PressDown = down;
            PressRight = right;
            PressLeft = left;
            PressKick = kick;

            // haut et bas s'annulent
            if (PressUp && PressDown)
            {
                PressDown = false;
                PressUp = false;
            }

            // droite et gauche s'annulent
            if (PressRight && PressLeft)
            {
                PressRight = false;
                PressLeft = false;
            }
        }
    }
}
