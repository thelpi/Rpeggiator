using System.Collections.Generic;
using System.Linq;
using RpeggiatorLib.Render;

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
        /// Creates an instance from json datas.
        /// </summary>
        /// <param name="datas">Json datas.</param>
        /// <returns><see cref="Gate"/></returns>
        internal static Gate FromDynamic(dynamic datas)
        {
            Gate g = new Gate((double)datas.X, (double)datas.Y, (double)datas.Width, (double)datas.Height, (bool)datas.Activated);
            g.SetRenderFromDynamic((object)datas);
            return g;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="activated"><see cref="Activated"/></param>
        internal Gate(double x, double y, double width, double height, bool activated)
            : base(x, y, width, height)
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
