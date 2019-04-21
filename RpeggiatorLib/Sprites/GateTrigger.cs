using RpeggiatorLib.Renders;

namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents a <see cref="FloorTrigger"/> which make appears or disappear <see cref="Gate"/>.
    /// </summary>
    public class GateTrigger : FloorTrigger
    {
        // Render when the ttrigger is ON.
        private readonly Render _renderOn;

        /// <summary>
        /// Indicates the gate's identifier linked to this trigger in the <see cref="Gate"/> collection of <see cref="Engine"/>.
        /// </summary>
        public int GateId { get; private set; }
        /// <summary>
        /// Indicates if the <see cref="Gate"/> appears when the trigger is activated; otherwise, it disappear.
        /// </summary>
        public bool AppearOnActivation { get; private set; }
        /// <inheritdoc />
        public override Render Render { get { return IsActivated ? _renderOn : _render; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"><see cref="Sprite.Id"/></param>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="actionDuration"><see cref="FloorTrigger._actionDuration"/></param>
        /// <param name="gateId"><see cref="GateId"/></param>
        /// <param name="appearOnActivation"><see cref="AppearOnActivation"/></param>
        /// <param name="renderType"><see cref="Enums.RenderType"/></param>
        /// <param name="renderProperties">Datas required to initialize the <see cref="Renders.Render"/>.</param>
        /// <param name="onRenderType"><see cref="Enums.RenderType"/></param>
        /// <param name="onRenderProperties">Datas required to initialize <see cref="_renderOn"/>.</param>
        internal GateTrigger(int id, double x, double y, double width, double height,
            double actionDuration, int gateId, bool appearOnActivation,
            Enums.RenderType renderType, object[] renderProperties, Enums.RenderType onRenderType, object[] onRenderProperties)
            : base(id, x, y, width, height, actionDuration, renderType, renderProperties)
        {
            GateId = gateId;
            AppearOnActivation = appearOnActivation;
            _renderOn = GetRenderFromValues(onRenderType, onRenderProperties);
        }
    }
}
