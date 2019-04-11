using System.Collections.Generic;
using System.Linq;

namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents a gate; basically, a wall with an activation state.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class Gate : Sprite
    {
        /// <summary>
        /// Indicates the value of <see cref="Activated"/> when no active trigger.
        /// </summary>
        private bool _defaultActivated;

        /// <summary>
        /// Indicates if the gate is currently activated.
        /// </summary>
        public bool Activated { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gateJson">The json dynamic object.</param>
        internal Gate(dynamic gateJson)
            : base((double)gateJson.X, (double)gateJson.Y, (double)gateJson.Width, (double)gateJson.Height)
        {
            Activated = gateJson.Activated;
            _defaultActivated = gateJson.Activated;
        }

        /// <inheritdoc />
        internal override void BehaviorAtNewFrame()
        {
            IReadOnlyCollection<GateTrigger> triggersOn = Engine.Default.CurrentScreen.GetTriggersForSpecifiedGate(this);

            if (triggersOn.Any())
            {
                // when several triggers activate the same gate, the most frequent value of "AppearOnActivation" is considered
                Activated = triggersOn.GroupBy(wt => wt.AppearOnActivation).OrderByDescending(wtGroup => wtGroup.Count()).First().Key;
            }
            else
            {
                // reset to default if no active trigger
                Activated = _defaultActivated;
            }
        }
    }
}
