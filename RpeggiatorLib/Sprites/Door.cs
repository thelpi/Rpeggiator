using System.Linq;
using RpeggiatorLib.Render;

namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents a door.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class Door : Sprite
    {
        // The optionnal key identifier to unlock the door.
        private int? _keyId;
        // The screen identifier this door is connected.
        private int _screenId;
        // Render when locked.
        private ISpriteRender _renderLocked;

        /// <summary>
        /// Identifier.
        /// </summary>
        /// <remarks>A door has the same identifier in both screens.</remarks>
        public int Id { get; private set; }
        /// <summary>
        /// The <see cref="Player"/> X-axis position when he goes through the door and changes screen.
        /// </summary>
        public double PlayerGoThroughX { get; private set; }
        /// <summary>
        /// The <see cref="Player"/> Y-axis position when he goes through the door and changes screen.
        /// </summary>
        public double PlayerGoThroughY { get; private set; }
        /// <summary>
        /// Inferred; indicates if the door is locked (the <see cref="Player"/> doesn't have the key).
        /// </summary>
        public bool Locked { get { return !TryOpen().HasValue; } }
        /// <summary>
        /// The render, which changes regarding if the door is locked or not.
        /// </summary>
        public override ISpriteRender Render { get { return Locked ? _renderLocked : _render; } }

        /// <summary>
        /// Creates an instance from json datas.
        /// </summary>
        /// <param name="datas">Json datas.</param>
        /// <returns><see cref="Door"/></returns>
        internal static Door FromDynamic(dynamic datas)
        {
            Door d = new Door((double)datas.X, (double)datas.Y, (double)datas.Width, (double)datas.Height, (int?)datas.KeyId,
                (int)datas.ScreenId, (int)datas.Id, (double)datas.PlayerGoThroughX, (double)datas.PlayerGoThroughY,
                (string)datas.LockedRenderType, (string)datas.LockedRenderValue);
            d.SetRenderFromDynamic((object)datas);
            return d;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="keyId"><see cref="_keyId"/></param>
        /// <param name="screenId"><see cref="_screenId"/></param>
        /// <param name="id"><see cref="Id"/></param>
        /// <param name="playerGoThroughX"><see cref="PlayerGoThroughX"/></param>
        /// <param name="playerGoThroughY"><see cref="PlayerGoThroughY"/></param>
        /// <param name="lockedRenderType"><see cref="_renderLocked"/> render type.</param>
        /// <param name="lockedRenderValue"><see cref="_renderLocked"/> render value.</param>
        internal Door(double x, double y, double width, double height, int? keyId, int screenId, int id,
            double playerGoThroughX, double playerGoThroughY, string lockedRenderType, string lockedRenderValue)
            : base(x, y, width, height)
        {
            _keyId = keyId;
            _screenId = screenId;
            Id = id;
            PlayerGoThroughX = playerGoThroughX;
            PlayerGoThroughY = playerGoThroughY;
            switch (lockedRenderType)
            {
                case nameof(PlainRender):
                    _renderLocked = new PlainRender(lockedRenderValue);
                    break;
                case nameof(ImageRender):
                    _renderLocked = new ImageRender(lockedRenderValue);
                    break;
                case nameof(ImageMosaicRender):
                    _renderLocked = new ImageMosaicRender(lockedRenderValue, this);
                    break;
                default:
                    throw new System.NotImplementedException(Messages.NotImplementedGraphicExceptionMessage);
            }
        }

        /// <summary>
        /// Tries to open the door.
        /// </summary>
        /// <returns>The <see cref="Screen"/> identifier; <c>Null</c> if failure.</returns>
        internal int? TryOpen()
        {
            if (!_keyId.HasValue || Engine.Default.Player.Inventory.Keyring.Any(k => k == _keyId))
            {
                return _screenId;
            }

            return null;
        }
    }
}
