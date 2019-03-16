using System.Linq;

namespace RPG4.Abstractions
{
    public class FloorTrigger : SizedPoint
    {
        private int _actionDelayCurrentCountTick;

        public int ActionDelayMaxTickCount { get; private set; }
        public bool CurrentlyOn { get { return _actionDelayCurrentCountTick >= 0; } }

        public FloorTrigger(double x, double y, double width, double height, int actionDelayMaxTickCount)
            : base(x, y, width, height)
        {
            ActionDelayMaxTickCount = actionDelayMaxTickCount;
            _actionDelayCurrentCountTick = -1;
        }

        public FloorTrigger(dynamic triggerJson) : base((object)triggerJson)
        {
            ActionDelayMaxTickCount = triggerJson.ActionDelayMaxTickCount;
            _actionDelayCurrentCountTick = -1;
        }

        public override void ComputeBehaviorAtTick(AbstractEngine engine, KeyPress keys)
        {
            // notice that enemies also walks on the trigger
            bool overlap = Overlap(engine.Player) || engine.Enemies.Any(e => Overlap(e));
            
            if (overlap)
            {
                _actionDelayCurrentCountTick = 0;
            }
            else if (_actionDelayCurrentCountTick >= ActionDelayMaxTickCount)
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
