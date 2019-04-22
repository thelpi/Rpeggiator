namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents a trigger which can be activated by walking on it.
    /// </summary>
    /// <remarks>This kind of trigger might be activated by <see cref="Enemy"/>.</remarks>
    /// <seealso cref="Sprite"/>
    public abstract class FloorTrigger : Sprite
    {
        // Action duration, in milliseconds.
        private readonly double _actionDuration;
        // Trigger time manager.
        private Elapser _triggerTimeManager;

        /// <summary>
        /// Indicates if the trigger is currently activated.
        /// </summary>
        public bool IsActivated { get { return _triggerTimeManager?.Elapsed == false; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"><see cref="Sprite.Id"/></param>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="actionDuration"><see cref="_actionDuration"/></param>
        /// <param name="renderType"><see cref="Enums.RenderType"/></param>
        /// <param name="renderProperties">Datas required to initialize the <see cref="Renders.IRender"/>.</param>
        protected FloorTrigger(int id, double x, double y, double width, double height
            , double actionDuration, Enums.RenderType renderType, params string[] renderProperties)
            : base(id, x, y, width, height, renderType, renderProperties)
        {
            _actionDuration = actionDuration;
        }

        /// <inheritdoc />
        internal override void BehaviorAtNewFrame()
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
