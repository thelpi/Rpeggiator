namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents a sprite which can be damaged.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class DamageableSprite : Sprite
    {
        /// <summary>
        /// Current number of life points.
        /// </summary>
        public double CurrentLifePoints { get; protected set; }
        /// <summary>
        /// <see cref="Enums.ItemType.Bomb"/> explosion life points cost.
        /// </summary>
        public double ExplosionLifePointCost { get; protected set; }
        /// <summary>
        /// <see cref="Enums.ItemType.Arrow"/> targeting life points cost.
        /// </summary>
        public double ArrowLifePointCost { get; protected set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="damageableSpriteJson">The json dynamic object.</param>
        public DamageableSprite(dynamic damageableSpriteJson) : base((object)damageableSpriteJson) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="lifepoints"><see cref="CurrentLifePoints"/></param>
        public DamageableSprite(double x, double y, double width, double height, double lifepoints) 
            : base(x, y, width, height)
        {
            CurrentLifePoints = lifepoints;
        }
    }
}
