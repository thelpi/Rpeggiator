namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents an item dropped on the floor.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class ActionnedItem : Sprite
    {
        /// <summary>
        /// Indicates the item use is finished.
        /// </summary>
        public virtual bool IsDone { get { throw new System.NotImplementedException(Messages.NotImplementedVirtualExceptionMessage); } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        protected ActionnedItem(double x, double y, double width, double height)
            : base(x, y, width, height) { }

        /// <summary>
        /// Overriden; Gets the lifepoints cost for the specified <paramref name="sprite"/>, if this instance hits it.
        /// </summary>
        /// <param name="sprite"><see cref="DamageableSprite"/></param>
        /// <returns>Life points cost.</returns>
        internal virtual double GetLifePointsCost(DamageableSprite sprite)
        {
            return 0;
        }
    }
}
