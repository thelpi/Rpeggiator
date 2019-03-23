namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents a <see cref="FloorTrigger"/> which make appears or disappear <see cref="Gate"/>.
    /// </summary>
    public class GateTrigger : FloorTrigger
    {
        /// <summary>
        /// Indicates the index of the gate linked to this trigger in the <see cref="Gate"/> collection of <see cref="AbstractEngine"/>.
        /// </summary>
        public int GateIndex { get; private set; }
        /// <summary>
        /// Indicates if the <see cref="Gate"/> appears when the trigger is activated; otherwise, it disappear.
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
        /// <param name="gateIndex"><see cref="GateIndex"/></param>
        /// <param name="appearOnActivation"><see cref="AppearOnActivation"/></param>
        public GateTrigger(double x, double y, double width, double height, int actionDelayMaxTickCount, int gateIndex, bool appearOnActivation)
            : base(x, y, width, height, actionDelayMaxTickCount)
        {
            GateIndex = gateIndex;
            AppearOnActivation = appearOnActivation;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gatetriggerJson">The json dynamic object.</param>
        public GateTrigger(dynamic gatetriggerJson) : base((object)gatetriggerJson)
        {
            GateIndex = gatetriggerJson.GateIndex;
            AppearOnActivation = gatetriggerJson.AppearOnActivation;
        }
    }
}
