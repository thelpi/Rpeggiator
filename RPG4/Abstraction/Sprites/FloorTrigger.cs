using RPG4.Abstraction.Graphic;

namespace RPG4.Abstraction.Sprites
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
        /// Graphic rendering when activated.
        /// </summary>
        public ISpriteGraphic ActivatedGraphic { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="triggerJson">The json dynamic object.</param>
        public FloorTrigger(dynamic triggerJson) : base((object)triggerJson)
        {
            _actionDelayMaxFrameCount = Tools.ComputeFormulaResult<int>((string)triggerJson.ActionDelayMaxFrameCount, Constants.SUBSTITUTE_FORMULA_FPS);
            _actionDelayCurrentFrameCount = -1;
            switch ((string)triggerJson.GraphicType)
            {
                case nameof(ImageBrushGraphic):
                    ActivatedGraphic = new ImageBrushGraphic((string)triggerJson.GraphicValueActivated);
                    break;
                case nameof(PlainBrushGraphic):
                    ActivatedGraphic = new PlainBrushGraphic((string)triggerJson.GraphicValueActivated);
                    break;
                    // TODO : other types of ISpriteGraphic must be implemented here.
            }
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
