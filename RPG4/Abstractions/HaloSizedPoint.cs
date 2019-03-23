namespace RPG4.Abstractions
{
    /// <summary>
    /// Represents a <see cref="Sprite"/> with an halo around it.
    /// </summary>
    public class HaloSizedPoint : Sprite
    {
        // ticks count with halo
        private int _haloTickCount;

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
        public bool DisplayHalo { get { return _haloTickCount >= 0; } }
        /// <summary>
        /// Tick count before the effect of a halo ends.
        /// </summary>
        public int HaloTickMaxCount { get; private set; }
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
        /// <param name="haloTickMaxCount"><see cref="HaloTickMaxCount"/></param>
        public HaloSizedPoint(double x, double y, double width, double height, double haloReachRatio, int haloTickMaxCount)
            : base(x, y, width, height)
        {
            HaloReachRatio = InitialPlayerStatus.INITIAL_HIT_HALO_SIZE_RATIO;
            _haloTickCount = -1;
            HaloTickMaxCount = haloTickMaxCount;
        }

        /// <summary>
        /// Instance behavior at tick.
        /// </summary>
        /// <param name="engine"><see cref="AbstractEngine"/></param>
        /// <param name="args">Other arguments; in this case a boolean which indicates if the halo should start.</param>
        public override void ComputeBehaviorAtTick(AbstractEngine engine, params object[] args)
        {
            if (_haloTickCount >= HaloTickMaxCount)
            {
                _haloTickCount = -1;
            }
            else if (_haloTickCount >= 0)
            {
                _haloTickCount += 1;
            }
            else if ((bool)args[0])
            {
                _haloTickCount = 0;
            }
        }
    }
}
