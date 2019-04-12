using RpeggiatorLib.Render;

namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents a <see cref="FloorTrigger"/> which make appears or disappear <see cref="Gate"/>.
    /// </summary>
    public class GateTrigger : FloorTrigger
    {
        // Render when the ttrigger is ON.
        private ISpriteRender _renderOn;

        /// <summary>
        /// Indicates the index of the gate linked to this trigger in the <see cref="Gate"/> collection of <see cref="Engine"/>.
        /// </summary>
        public int GateIndex { get; private set; }
        /// <summary>
        /// Indicates if the <see cref="Gate"/> appears when the trigger is activated; otherwise, it disappear.
        /// </summary>
        public bool AppearOnActivation { get; private set; }
        /// <inheritdoc />
        public override ISpriteRender Render { get { return IsActivated ? _renderOn : _render; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gatetriggerJson">The json dynamic object.</param>
        internal GateTrigger(dynamic gatetriggerJson) : base((object)gatetriggerJson)
        {
            GateIndex = gatetriggerJson.GateIndex;
            AppearOnActivation = gatetriggerJson.AppearOnActivation;
            _render = new ImageRender("TriggerOff");
            _renderOn = new ImageRender("TriggerOn");
        }
    }
}
