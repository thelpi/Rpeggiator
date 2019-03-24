namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents a <see cref="Sprite"/> with an halo around it.
    /// </summary>
    public class HaloSprite : Sprite
    {
        // Frames count with halo.
        private int _haloFrameCount;
        // Frames count before the effect of a halo ends.
        private int _haloFrameMaxCount;
        // Ratio of halo reach depending to the instance size.
        private double _haloReachRatio;

        /// <summary>
        /// Inferred; indicates if the halo is currently active.
        /// </summary>
        public bool Active { get { return _haloFrameCount >= 0; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/> of the carrier.</param>
        /// <param name="y"><see cref="Sprite.Y"/> of the carrier.</param>
        /// <param name="width"><see cref="Sprite.Width"/> of the carrier.</param>
        /// <param name="height"><see cref="Sprite.Height"/> of the carrier.</param>
        /// <param name="haloReachRatio"><see cref="_haloReachRatio"/></param>
        /// <param name="haloFrameMaxCount"><see cref="_haloFrameMaxCount"/></param>
        public HaloSprite(double x, double y, double width, double height, double haloReachRatio, int haloFrameMaxCount)
            : base(x - (((haloReachRatio - 1) / 2) * width),
                  y - (((haloReachRatio - 1) / 2) * height),
                  width * haloReachRatio,
                  height * haloReachRatio)
        {
            _haloReachRatio = haloReachRatio;
            _haloFrameCount = -1;
            _haloFrameMaxCount = haloFrameMaxCount;
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame(AbstractEngine engine, params object[] args)
        {
            if (_haloFrameCount >= _haloFrameMaxCount)
            {
                _haloFrameCount = -1;
            }
            else if (_haloFrameCount >= 0)
            {
                _haloFrameCount += 1;
            }
            else if ((bool)args[0])
            {
                _haloFrameCount = 0;
            }

            // Adjust the halo position and size, based on the <see cref="Sprite"/> current position and size.
            Sprite sprite = args[1] as Sprite;
            X = sprite.X - (((_haloReachRatio - 1) / 2) * sprite.Width);
            Y = sprite.Y - (((_haloReachRatio - 1) / 2) * sprite.Height);
            Width = sprite.Width * _haloReachRatio;
            Height = sprite.Height * _haloReachRatio;
        }
    }
}
