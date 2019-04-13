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
        /// Creates an instance from json datas.
        /// </summary>
        /// <param name="datas">Json datas.</param>
        /// <returns><see cref="Floor"/></returns>
        internal static Floor FromDynamic(dynamic datas)
        {
            Floor f = new Floor((double)datas.X, (double)datas.Y, (double)datas.Width, (double)datas.Height, (FloorType)datas.FloorType);
            f.SetRenderFromDynamic((object)datas);
            return f;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="floorType"><see cref="FloorType"/></param>
        internal Floor(double x, double y, double width, double height, FloorType floorType)
            : base(x, y, width, height)
        {
            FloorType = floorType;
            SpeedRatio = Constants.FLOOR_SPEED_RATIO[FloorType];
        }
    }
}
