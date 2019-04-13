using System.Linq;
using RpeggiatorLib.Enums;
using RpeggiatorLib.Render;

namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents an <see cref="ItemType.Arrow"/> when throwed from a <see cref="ItemType.Bow"/>.
    /// </summary>
    /// <seealso cref="ActionnedItem"/>
    public class ActionnedArrow : ActionnedItem
    {
        private Elapser _elapser;
        private bool _hitOrAway;
        private LifeSprite _thrownBy;

        /// <inheritdoc />
        public override bool IsDone { get { return _hitOrAway; } }
        /// <summary>
        /// <see cref="Direction"/> of the arrow.
        /// </summary>
        public Direction Direction { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="point">Starting <see cref="Point"/>.</param>
        /// <param name="direction"><see cref="Direction"/></param>
        /// <param name="thrownBy"><see cref="LifeSprite"/> who throws the arrow.</param>
        internal ActionnedArrow(Point point, Direction direction, LifeSprite thrownBy)
            : base(point.X, point.Y, Constants.Arrow.WIDTH, Constants.Arrow.HEIGHT)
        {
            Direction = direction;
            _elapser = new Elapser();
            _hitOrAway = false;
            _thrownBy = thrownBy;
            _render = new ImageDirectionRender("Arrow", this, nameof(Direction));
        }

        /// <inheritdoc />
        internal override void BehaviorAtNewFrame()
        {
            base.BehaviorAtNewFrame();

            Point nextPos = Tools.ComputeMovementNextPointInDirection(X, Y, _elapser.Distance(Constants.Arrow.SPEED), Direction);
            Move(nextPos.X, nextPos.Y);

            // Checks if hit enemies.
            // Checks if hit player.
            // Checks if outside screen.
            _hitOrAway = Engine.Default.CurrentScreen.Enemies.Any(e => e != _thrownBy && Overlap(e))
                || (Overlap(Engine.Default.Player) && Engine.Default.Player != _thrownBy)
                || !Engine.Default.CurrentScreen.IsInside(this);
        }

        /// <inheritdoc />
        internal override double GetLifePointsCost(DamageableSprite sprite)
        {
            return Overlap(sprite) && sprite != _thrownBy ? sprite.ArrowLifePointCost : 0;
        }
    }
}
