namespace RPG4.Abstractions
{
    public class WallTrigger : FloorTrigger
    {
        public int WallIndex { get; private set; }
        public bool ShowOnAction { get; private set; }

        public WallTrigger(double x, double y, double width, double height, int actionDelayMaxTickCount, int wallIndex, bool showOnAction)
            : base(x, y, width, height, actionDelayMaxTickCount)
        {
            WallIndex = wallIndex;
            ShowOnAction = showOnAction;
        }

        public WallTrigger(dynamic walltriggerJson) : base((object)walltriggerJson)
        {
            WallIndex = walltriggerJson.WallIndex;
            ShowOnAction = walltriggerJson.ShowOnAction;
        }

        public override void ComputeBehaviorAtTick(AbstractEngine engine, KeyPress keys)
        {
            base.ComputeBehaviorAtTick(engine, keys);

            engine.Walls[WallIndex].SetConcrete(engine, this);
        }
    }
}
