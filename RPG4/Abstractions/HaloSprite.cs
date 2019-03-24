namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents a <see cref="Sprite"/> with an halo around it.
    /// </summary>
    public class HaloSprite : Sprite
    {
        // Frames count with halo.
        private int _haloFrameCount;

        /// <summary>
        /// Ratio of halo reach depending to the instance size.
        /// </summary>
        public double HaloReachRatio { get; private set; }
        /// <summary>
        /// Inferred; the X-axis size of an halo reach depending on <see cref="Sprite.Width"/>.
        /// </summary>
        public double HaloWidth { get { return ((HaloReachRatio - 1) / 2) * Width; } }
        /// <summary>
        /// Inferred; the Y-axis size of an halo reach depending on <see cref="Sprite.Height"/>.
        /// </summary>
        public double HaloHeight { get { return ((HaloReachRatio - 1) / 2) * Height; } }
        /// <summary>
        /// Inferred; indicates if currently displaying halo.
        /// </summary>
        public bool DisplayHalo { get { return _haloFrameCount >= 0; } }
        /// <summary>
        /// Frames count before the effect of a halo ends.
        /// </summary>
        public int HaloFrameMaxCount { get; private set; }
        /// <summary>
        /// Inferred; represents the halo itself.
        /// </summary>
        /// <remarks><c>Null</c> if <see cref="DisplayHalo"/> is <c>False</c>.</remarks>
        public Sprite Halo
        {
            get
            {
                if (!DisplayHalo)
                {
                    return null;
                }

                var haloX = X - (((HaloReachRatio - 1) / 2) * Width);
                var haloY = Y - (((HaloReachRatio - 1) / 2) * Height);

                return !DisplayHalo ? null : new Sprite(haloX, haloY, Width * HaloReachRatio, Height * HaloReachRatio);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="haloReachRatio"><see cref="HaloReachRatio"/></param>
        /// <param name="haloFrameMaxCount"><see cref="HaloFrameMaxCount"/></param>
        public HaloSprite(double x, double y, double width, double height, double haloReachRatio, int haloFrameMaxCount)
            : base(x, y, width, height)
        {
            HaloReachRatio = InitialPlayerStatus.INITIAL_HIT_HALO_SIZE_RATIO;
            _haloFrameCount = -1;
            HaloFrameMaxCount = haloFrameMaxCount;
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame(AbstractEngine engine, params object[] args)
        {
            if (_haloFrameCount >= HaloFrameMaxCount)
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
        }

        /// <summary>
        /// Adjust the halo position, based on <see cref="Player"/> position.
        /// </summary>
        /// <param name="player">The player.</param>
        public void AdjustToPlayer(Player player)
        {
            X = player.X;
            Y = player.Y;
        }
    }
}
