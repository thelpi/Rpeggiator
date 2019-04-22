using System;

namespace RpeggiatorLib
{
    /// <summary>
    /// Lifetime event manager.
    /// </summary>
    internal class Elapser
    {
        private readonly DateTime _timestamp;
        private readonly double _lifetime;
        private DateTime? _latestTimestamp;

        /// <summary>
        /// Inferred; indicates if the instance is elapsed.
        /// </summary>
        internal bool Elapsed
        {
            get
            {
                return (DateTime.Now - _timestamp).TotalMilliseconds.GreaterEqual(_lifetime);
            }
        }
        /// <summary>
        /// Inferred; indicates the number of milliseconds elapsed.
        /// </summary>
        internal int ElapsedMilliseconds
        {
            get
            {
                return Convert.ToInt32(Math.Floor((DateTime.Now - _timestamp).TotalMilliseconds));
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        internal Elapser() : this(double.PositiveInfinity) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="lifetime">Lifetime, in milliseconds.</param>
        internal Elapser(double lifetime)
        {
            _timestamp = DateTime.Now;
            _lifetime = lifetime;
        }

        /// <summary>
        /// Gets the distance between two calls.
        /// </summary>
        /// <param name="pixelsBySecond">Speed; pixels by second.</param>
        /// <returns>The distance, in pixels.</returns>
        internal double Distance(double pixelsBySecond)
        {
            double distance = (DateTime.Now - (_latestTimestamp ?? DateTime.Now)).TotalMilliseconds * (pixelsBySecond / 1000);
            _latestTimestamp = DateTime.Now;
            return distance;
        }

        /// <summary>
        /// Reset the instance.
        /// </summary>
        internal void Reset()
        {
            _latestTimestamp = null;
        }

        /// <summary>
        /// Computes the current step index considering a step delay.
        /// </summary>
        /// <param name="stepDelay">Step delay (in milliseconds).</param>
        /// <param name="maxStep">Maximal step index before reset to zero.</param>
        /// <returns>Step index.</returns>
        internal int GetStepIndex(double stepDelay, int maxStep)
        {
            int index = ElapsedMilliseconds / Convert.ToInt32(Math.Floor(stepDelay));
            while (index > maxStep)
            {
                index -= maxStep;
            }
            return index;
        }
    }
}
