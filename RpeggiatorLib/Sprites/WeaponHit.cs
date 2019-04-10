﻿namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents the weapon hit sprite.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class WeaponHit : Sprite
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        internal WeaponHit(double x, double y, double width, double height)
            : base(x, y, width, height) { }
    }
}
