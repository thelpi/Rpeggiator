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
        /// Creates an instance from json datas.
        /// </summary>
        /// <param name="datas">Json datas.</param>
        /// <returns><see cref="GateTrigger"/></returns>
        public static GateTrigger FromDynamic(dynamic datas)
        {
            GateTrigger gt = new GateTrigger((double)datas.X, (double)datas.Y, (double)datas.Width, (double)datas.Height,
                (double)datas.ActionDuration, (int)datas.GateIndex, (bool)datas.AppearOnActivation,
                (string)datas.OnRenderType, (string)datas.OnRenderValue);
            gt.SetRenderFromDynamic((object)datas);
            return gt;
        }

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
        /// <param name="onRenderType"><see cref="_renderOn"/> render type.</param>
        /// <param name="onRenderValue"><see cref="_renderOn"/> render value.</param>
        internal GateTrigger(double x, double y, double width, double height,
            double actionDuration, int gateIndex, bool appearOnActivation, string onRenderType, string onRenderValue)
            : base(x, y, width, height, actionDuration)
        {
            GateIndex = gateIndex;
            AppearOnActivation = appearOnActivation;
            switch (onRenderType)
            {
                case nameof(ImageMosaicRender):
                    _renderOn = new ImageMosaicRender(onRenderValue, this);
                    break;
                case nameof(ImageRender):
                    _renderOn = new ImageRender(onRenderValue);
                    break;
                case nameof(PlainRender):
                    _renderOn = new PlainRender(onRenderValue);
                    break;
                default:
                    throw new System.NotImplementedException(Messages.NotImplementedGraphicExceptionMessage);
            }
        }
    }
}
