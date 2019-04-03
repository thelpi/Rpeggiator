namespace RPG4.Models.Sprites
{
    /// <summary>
    /// Represents a <see cref="FloorTrigger"/> which make appears or disappear <see cref="Gate"/>.
    /// </summary>
    public class GateTrigger : FloorTrigger
    {
        /// <summary>
        /// Indicates the index of the gate linked to this trigger in the <see cref="Gate"/> collection of <see cref="Engine"/>.
        /// </summary>
        public int GateIndex { get; private set; }
        /// <summary>
        /// Indicates if the <see cref="Gate"/> appears when the trigger is activated; otherwise, it disappear.
        /// </summary>
        public bool AppearOnActivation { get; private set; }

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
