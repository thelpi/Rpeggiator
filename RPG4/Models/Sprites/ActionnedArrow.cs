using System.Linq;
using System.Windows;
using RPG4.Models.Enums;

namespace RPG4.Models.Sprites
{
    /// <summary>
    /// Represents an <see cref="ItemType.Arrow"/> when throwed from a <see cref="ItemType.Bow"/>.
    /// </summary>
    /// <seealso cref="ActionnedItem"/>
    public class ActionnedArrow : ActionnedItem
    {
        private Direction _direction;
        private Elapser _elapser;
        private bool _hitOrAway;

        /// <inheritdoc />
        public override bool IsDone { get { return _hitOrAway; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="point">Starting <see cref="Point"/>.</param>
        /// <param name="direction"><see cref="Direction"/></param>
        public ActionnedArrow(Point point, Direction direction) : base(point.X, point.Y,
            Constants.Arrow.WIDTH, Constants.Arrow.HEIGHT,
            Constants.Arrow.GRAPHIC_RENDERING)
        {
            _direction = direction;
            _elapser = new Elapser();
            _hitOrAway = false;
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame()
        {
            base.BehaviorAtNewFrame();

            Point nextPos = Tools.ComputeMovementNextPointInDirection(X, Y, _elapser.Distance(Constants.Arrow.SPEED), _direction);
            X = nextPos.X;
            Y = nextPos.Y;

            // Checks if hit enemies.
            // Checks if hit player.
            // Checks if hit structures.
            // Checks if outside screen.
            _hitOrAway = Engine.Default.CurrentScreen.Enemies.Any(e => Overlap(e))
                || Overlap(Engine.Default.Player)
                || !IsInside(Engine.Default.CurrentScreen)
                || Engine.Default.CurrentScreen.Structures.Any(s => Overlap(s));
        }
    }
}
