using System;
using RpeggiatorLib.Enums;
using RpeggiatorLib.Render;

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

            System.Windows.Media.Color c;
            switch (FloorType)
            {
                case FloorType.Ground:
                    c = System.Windows.Media.Colors.Tan;
                    break;
                case FloorType.Ice:
                    c = System.Windows.Media.Colors.PaleTurquoise;
                    SpeedRatio = Constants.FLOOR_ICE_SPEED_RATIO;
                    break;
                case FloorType.Lava:
                    c = System.Windows.Media.Colors.Red;
                    break;
                case FloorType.Water:
                    c = System.Windows.Media.Colors.Blue;
                    SpeedRatio = Constants.FLOOR_WATER_SPEED_RATIO;
                    break;
                default:
                    throw new NotImplementedException(Messages.NotImplementedGraphicExceptionMessage);
            }

            _render = new PlainRender(Tools.HexFromColor(c));
        }
    }
}
