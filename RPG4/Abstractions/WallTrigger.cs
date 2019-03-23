namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents a <see cref="FloorTrigger"/> which make appears or disappear <see cref="Wall"/>.
    /// </summary>
    public class WallTrigger : FloorTrigger
    {
        /// <summary>
        /// Indicates the index of the wall linked to this trigger in the <see cref="AbstractEngine.Walls"/>.
        /// </summary>
        public int WallIndex { get; private set; }
        /// <summary>
        /// Indicates if the <see cref="Wall"/> appears when the trigger is activated; otherwise, it disappear.
        /// </summary>
        public bool AppearOnActivation { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="actionDelayMaxTickCount">Number of ticks before the activation ends.</param>
        /// <param name="wallIndex"><see cref="WallIndex"/></param>
        /// <param name="appearOnActivation"><see cref="AppearOnActivation"/></param>
        public WallTrigger(double x, double y, double width, double height, int actionDelayMaxTickCount, int wallIndex, bool appearOnActivation)
            : base(x, y, width, height, actionDelayMaxTickCount)
        {
            WallIndex = wallIndex;
            AppearOnActivation = appearOnActivation;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="walltriggerJson">The json dynamic object.</param>
        public WallTrigger(dynamic walltriggerJson) : base((object)walltriggerJson)
        {
            WallIndex = walltriggerJson.WallIndex;
            AppearOnActivation = walltriggerJson.AppearOnActivation;
        }
    }
}
