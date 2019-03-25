﻿namespace RPG4.Abstractions
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
        public virtual bool IsDone { get { throw new System.InvalidOperationException("Should be implemented in derived class !"); } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        public ActionnedItem(double x, double y, double width, double height) : base(x, y, width, height) { }
    }
}