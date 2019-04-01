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
        // Action duration, in milliseconds.
        private readonly double _actionDuration;
        // Trigger time manager.
        private Elapser _triggerTimeManager;
        // Activated graphic.
        private ISpriteGraphic _activatedGraphic;

        /// <summary>
        /// Indicates if the trigger is currently activated.
        /// </summary>
        public bool IsActivated { get { return _triggerTimeManager?.Elapsed == false; } }
        /// <inheritdoc />
        public override ISpriteGraphic Graphic { get { return IsActivated ? _activatedGraphic : base.Graphic; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="triggerJson">The json dynamic object.</param>
        public FloorTrigger(dynamic triggerJson) : base((object)triggerJson)
        {
            _actionDuration = triggerJson.ActionDuration;
            switch ((string)triggerJson.GraphicType)
            {
                case nameof(ImageBrushGraphic):
                    _activatedGraphic = new ImageBrushGraphic((string)triggerJson.GraphicValueActivated);
                    break;
                case nameof(PlainBrushGraphic):
                    _activatedGraphic = new PlainBrushGraphic((string)triggerJson.GraphicValueActivated);
                    break;
                    // TODO : other types of ISpriteGraphic must be implemented here.
            }
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame()
        {
            if (Engine.Default.IsTriggered(this))
            {
                _triggerTimeManager = new Elapser(_actionDuration);
            }
            else if (_triggerTimeManager?.Elapsed == true)
            {
                _triggerTimeManager = null;
            }
        }
    }
}
