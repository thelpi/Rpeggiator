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
        /// <param name="floorJsonDatas">Floor json datas.</param>
        internal Floor(dynamic floorJsonDatas)
            : base((double)floorJsonDatas.X, (double)floorJsonDatas.Y, (double)floorJsonDatas.Width, (double)floorJsonDatas.Height)
        {
            SpeedRatio = 1;
            FloorType = floorJsonDatas.FloorType;
        }
    }
}
