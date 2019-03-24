using System;
using System.Linq;

namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents a trigger which can be activated by walking on it.
    /// </summary>
    /// <remarks>This kind of trigger might be activated by <see cref="Enemy"/>.</remarks>
    /// <seealso cref="Sprite"/>
    public class FloorTrigger : Sprite
    {
        // current number of frames while activated
        private int _actionDelayCurrentFrameCount;
        // Number of frames before the activation ends.
        private readonly int _actionDelayMaxFrameCount;

        /// <summary>
        /// Indicates if the trigger is currently activated.
        /// </summary>
        public bool IsActivated { get { return _actionDelayCurrentFrameCount >= 0; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="actionDelayMaxFrameCount">Number of frames before the activation ends.</param>
        public FloorTrigger(double x, double y, double width, double height, int actionDelayMaxFrameCount)
            : base(x, y, width, height)
        {
            _actionDelayMaxFrameCount = actionDelayMaxFrameCount;
            _actionDelayCurrentFrameCount = -1;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="triggerJson">The json dynamic object.</param>
        public FloorTrigger(dynamic triggerJson) : base((object)triggerJson)
        {
            string jsonFormula = triggerJson.ActionDelayMaxFrameCount;
            _actionDelayMaxFrameCount = Tools.ComputeFormulaResult<int>(jsonFormula, Constants.SUBSTITUTE_FORMULA_FPS);
            _actionDelayCurrentFrameCount = -1;
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame(AbstractEngine engine, params object[] args)
        {
            if (engine.IsTriggered(this))
            {
                _actionDelayCurrentFrameCount = 0;
            }
            else if (_actionDelayCurrentFrameCount >= _actionDelayMaxFrameCount)
            {
                _actionDelayCurrentFrameCount = -1;
            }
            else if (_actionDelayCurrentFrameCount >= 0)
            {
                _actionDelayCurrentFrameCount += 1;
            }
        }
    }
}
