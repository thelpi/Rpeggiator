using RpeggiatorLib.Enums;

namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents a floor.
    /// </summary>
    public class Floor : Sprite
    {
        /// <summary>
        /// <see cref="Enums.FloorType"/>
        /// </summary>
        public FloorType FloorType { get; private set; }
        /// <summary>
        /// Influence on speed (ratio).
        /// </summary>
        public double SpeedRatio { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"><see cref="Sprite.Id"/></param>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="floorType"><see cref="FloorType"/></param>
        /// <param name="renderType"><see cref="Render.ISpriteRender"/> subtype name.</param>
        /// <param name="renderProperties">Datas required to initialize the <see cref="Render.ISpriteRender"/>.</param>
        internal Floor(int id, double x, double y, double width, double height,
            FloorType floorType, string renderType, object[] renderProperties)
            : base(id, x, y, width, height, renderType, renderProperties)
        {
            FloorType = floorType;
            SpeedRatio = Constants.FLOOR_SPEED_RATIO[FloorType];
        }
    }
}
