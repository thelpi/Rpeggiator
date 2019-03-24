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
        // current number of ticks while activated
        private int _actionDelayCurrentCountTick;
        // Number of ticks before the activation ends.
        private readonly int _actionDelayMaxTickCount;

        /// <summary>
        /// Indicates if the trigger is currently activated.
        /// </summary>
        public bool IsActivated { get { return _actionDelayCurrentCountTick >= 0; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="actionDelayMaxTickCount">Number of ticks before the activation ends.</param>
        public FloorTrigger(double x, double y, double width, double height, int actionDelayMaxTickCount)
            : base(x, y, width, height)
        {
            _actionDelayMaxTickCount = actionDelayMaxTickCount;
            _actionDelayCurrentCountTick = -1;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="triggerJson">The json dynamic object.</param>
        public FloorTrigger(dynamic triggerJson) : base((object)triggerJson)
        {
            string jsonFormula = triggerJson.ActionDelayMaxTickCount;
            _actionDelayMaxTickCount = Tools.ComputeFormulaResult<int>(jsonFormula, Constants.SUBSTITUTE_FORMULA_FPS);
            _actionDelayCurrentCountTick = -1;
        }

        /// <summary>
        /// Overridden; behavior of the instance at ticking.
        /// </summary>
        /// <param name="engine">The <see cref="AbstractEngine"/>.</param>
        /// <param name="args">Other arguments.</param>
        public override void ComputeBehaviorAtTick(AbstractEngine engine, params object[] args)
        {
            if (engine.IsTriggered(this))
            {
                _actionDelayCurrentCountTick = 0;
            }
            else if (_actionDelayCurrentCountTick >= _actionDelayMaxTickCount)
            {
                _actionDelayCurrentCountTick = -1;
            }
            else if (_actionDelayCurrentCountTick >= 0)
            {
                _actionDelayCurrentCountTick += 1;
            }
        }
    }
}
