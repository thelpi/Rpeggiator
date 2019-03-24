using System.Linq;

namespace RPG4.Abstractions
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
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="activated"><see cref="Activated"/></param>
        public Gate(double x, double y, double width, double height, bool activated)
            : base(x, y, width, height)
        {
            Activated = activated;
            _defaultActivated = activated;
        }

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
