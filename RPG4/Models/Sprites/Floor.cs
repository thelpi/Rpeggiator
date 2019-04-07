using RPG4.Models.Enums;
using RPG4.Models.Graphic;

namespace RPG4.Models.Sprites
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
        public Floor(dynamic floorJsonDatas) : base(
            (double)floorJsonDatas.X, (double)floorJsonDatas.Y,
            (double)floorJsonDatas.Width, (double)floorJsonDatas.Height)
        {
            SpeedRatio = 1;
            FloorType = floorJsonDatas.FloorType;
            switch (FloorType)
            {
                case FloorType.Lava:
                    Graphic = new PlainBrushGraphic(System.Windows.Media.Colors.Firebrick);
                    break;
                case FloorType.Water:
                    Graphic = new PlainBrushGraphic(System.Windows.Media.Colors.Blue);
                    SpeedRatio = Constants.FLOOR_WATER_SPEED_RATIO;
                    break;
                case FloorType.Ice:
                    Graphic = new PlainBrushGraphic(System.Windows.Media.Colors.LightBlue);
                    SpeedRatio = Constants.FLOOR_ICE_SPEED_RATIO;
                    break;
                default:
                    switch ((string)floorJsonDatas.GraphicType)
                    {
                        case nameof(ImageBrushGraphic):
                            Graphic = new ImageBrushGraphic((string)floorJsonDatas.ImagePath);
                            break;
                        case nameof(PlainBrushGraphic):
                            Graphic = new PlainBrushGraphic((string)floorJsonDatas.HexColor);
                            break;
                        default:
                            throw new System.NotImplementedException(Messages.NotImplementedGraphicExceptionMessage);
                    }
                    break;
            }
        }
    }
}
