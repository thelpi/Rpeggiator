using System.Collections.Generic;
using System.Linq;

namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents a wall.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class Wall : Sprite
    {
        /// <summary>
        /// Indicates the value of <see cref="Concrete"/> when no active trigger.
        /// </summary>
        private readonly bool _defaultConcrete;

        /// <summary>
        /// Indicates if the wall is currently concrete.
        /// </summary>
        public bool Concrete { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="concrete"><see cref="Concrete"/></param>
        public Wall(double x, double y, double width, double height, bool concrete)
            : base(x, y, width, height)
        {
            Concrete = concrete;
            _defaultConcrete = concrete;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="wallJson">The json dynamic object.</param>
        public Wall(dynamic wallJson) : base((object)wallJson)
        {
            Concrete = wallJson.Concrete;
            _defaultConcrete = wallJson.Concrete;
        }

        /// <summary>
        /// Behavior of the instance at ticking.
        /// </summary>
        /// <param name="engine">The <see cref="AbstractEngine"/>.</param>
        /// <param name="args">Other arguments.</param>
        public override void ComputeBehaviorAtTick(AbstractEngine engine, params object[] args)
        {
            IEnumerable<WallTrigger> triggersOn = engine.WallTriggers.Where(wt => wt.WallIndex == engine.Walls.IndexOf(this) && wt.IsActivated);

            if (triggersOn.Any())
            {
                // when several triggers activate the same wall, the most frequent value of "AppearOnActivation" is considered
                Concrete = triggersOn.GroupBy(wt => wt.AppearOnActivation).OrderByDescending(wtGroup => wtGroup.Count()).First().Key;
            }
            else
            {
                // reset to default if no active trigger
                Concrete = _defaultConcrete;
            }
        }
    }
}
