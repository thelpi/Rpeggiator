using System.Collections.Generic;
using System.Linq;
using RpeggiatorLib.Renders;

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
        private readonly bool _defaultActivated;

        /// <summary>
        /// Indicates if the gate is currently activated.
        /// </summary>
        public bool Activated { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"><see cref="Sprite.Id"/></param>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="activated"><see cref="Activated"/></param>
        /// <param name="renderType"><see cref="Enums.RenderType"/></param>
        /// <param name="renderProperties">Datas required to initialize the <see cref="IRender"/>.</param>
        internal Gate(int id, double x, double y, double width, double height,
            bool activated, Enums.RenderType renderType, string[] renderProperties)
            : base(id, x, y, width, height, renderType, renderProperties)
        {
            Activated = activated;
            _defaultActivated = activated;
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
