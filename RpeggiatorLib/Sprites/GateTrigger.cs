using RpeggiatorLib.Render;

namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents a <see cref="FloorTrigger"/> which make appears or disappear <see cref="Gate"/>.
    /// </summary>
    public class GateTrigger : FloorTrigger
    {
        // Render when the ttrigger is ON.
        private ISpriteRender _renderOn;

        /// <summary>
        /// Indicates the index of the gate linked to this trigger in the <see cref="Gate"/> collection of <see cref="Engine"/>.
        /// </summary>
        public int GateIndex { get; private set; }
        /// <summary>
        /// Indicates if the <see cref="Gate"/> appears when the trigger is activated; otherwise, it disappear.
        /// </summary>
        public bool AppearOnActivation { get; private set; }
        /// <inheritdoc />
        public override ISpriteRender Render { get { return IsActivated ? _renderOn : _render; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="actionDuration"><see cref="FloorTrigger._actionDuration"/></param>
        /// <param name="gateIndex"><see cref="GateIndex"/></param>
        /// <param name="appearOnActivation"><see cref="AppearOnActivation"/></param>
        /// <param name="renderType"><see cref="ISpriteRender"/> subtype name.</param>
        /// <param name="renderProperties">Datas required to initialize the <see cref="ISpriteRender"/>.</param>
        /// <param name="onRenderType"><see cref="_renderOn"/> subtype name.</param>
        /// <param name="onRenderProperties">Datas required to initialize <see cref="_renderOn"/>.</param>
        internal GateTrigger(double x, double y, double width, double height,
            double actionDuration, int gateIndex, bool appearOnActivation,
            string renderType, object[] renderProperties, string onRenderType, object[] onRenderProperties)
            : base(x, y, width, height, actionDuration, renderType, renderProperties)
        {
            GateIndex = gateIndex;
            AppearOnActivation = appearOnActivation;
            _renderOn = GetRenderFromValues(onRenderType, onRenderProperties);
        }
    }
}
