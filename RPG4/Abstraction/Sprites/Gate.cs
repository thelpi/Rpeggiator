using System.Linq;

namespace RPG4.Abstraction.Sprites
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
        private readonly bool _defaultActivated;

        /// <summary>
        /// Indicates if the gate is currently activated.
        /// </summary>
        public bool Activated { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gateJson">The json dynamic object.</param>
        public Gate(dynamic gateJson) : base((object)gateJson)
        {
            Activated = gateJson.Activated;
            _defaultActivated = gateJson.Activated;
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame(AbstractEngine engine, params object[] args)
        {
            var triggersOn = engine.GetTriggersForSpecifiedGate(this);

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
