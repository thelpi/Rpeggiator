namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents a wall.
    /// </summary>
    /// <seealso cref="SizedPoint"/>
    public class Wall : SizedPoint
    {
        /// <summary>
        /// Indicates if the wall is currently concrete.
        /// </summary>
        public bool Concrete { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="base.X"/></param>
        /// <param name="y"><see cref="base.Y"/></param>
        /// <param name="width"><see cref="base.Width"/></param>
        /// <param name="height"><see cref="base.Height"/></param>
        /// <param name="concrete"><see cref="Concrete"/></param>
        public Wall(double x, double y, double width, double height, bool concrete)
            : base(x, y, width, height)
        {
            Concrete = concrete;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="wallJson">The json dynamic object.</param>
        public Wall(dynamic wallJson) : base((object)wallJson)
        {
            Concrete = wallJson.Concrete;
        }

        /// <summary>
        /// Checks the <see cref="Concrete"/> value.
        /// </summary>
        /// <param name="engine">The <see cref="AbstractEngine"/>, which contains the current instance.</param>
        /// <param name="trigger">The <see cref="WallTrigger"/> which allegedly triggers the concrete status change.</param>
        public void SetConcrete(AbstractEngine engine, WallTrigger trigger)
        {
            if (engine.Walls[trigger.WallIndex] == this)
            {
                Concrete = trigger.ShowOnAction == trigger.CurrentlyOn;
            }
        }
    }
}
