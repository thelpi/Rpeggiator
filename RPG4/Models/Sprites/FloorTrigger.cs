using RPG4.Models.Graphic;

namespace RPG4.Models.Sprites
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
        protected FloorTrigger(dynamic triggerJson) : base(
            (double)triggerJson.X, (double)triggerJson.Y,
            (double)triggerJson.Width, (double)triggerJson.Height)
        {
            _actionDuration = triggerJson.ActionDuration;
            base.Graphic = new ImageBrushGraphic("TriggerOff");
            _activatedGraphic = new ImageBrushGraphic("TriggerOn");
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
