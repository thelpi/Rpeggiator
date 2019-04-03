using RPG4.Models.Graphic;

namespace RPG4.Models.Sprites
{
    /// <summary>
    /// Represents a floor.
    /// </summary>
    public class Floor : Sprite
    {
        /// <summary>
        /// <see cref="FloorTypeEnum"/>
        /// </summary>
        public FloorTypeEnum FloorType { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="floorJsonDatas">Floor json datas.</param>
        public Floor(dynamic floorJsonDatas)
            : base((double)floorJsonDatas.X, (double)floorJsonDatas.Y, (double)floorJsonDatas.Width, (double)floorJsonDatas.Height, null)
        {
            FloorType = floorJsonDatas.FloorType;
            switch (FloorType)
            {
                case FloorTypeEnum.Lava:
                    Graphic = new PlainBrushGraphic("#B22222");
                    break;
                case FloorTypeEnum.Water:
                    Graphic = new PlainBrushGraphic("#0000FF");
                    break;
                case FloorTypeEnum.Ice:
                    Graphic = new PlainBrushGraphic("#ADD8E6");
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
