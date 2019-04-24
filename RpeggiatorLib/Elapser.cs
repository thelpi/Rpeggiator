using System;
using System.Collections.Generic;

namespace RpeggiatorLib
{
    /// <summary>
    /// Lifetime event manager.
    /// </summary>
    internal class Elapser
    {
        // List of every instances. One by ElapserUse and owner.
        private static readonly List<Elapser> _instances = new List<Elapser>();

        /// <summary>
        /// <see cref="_instances"/>
        /// </summary>
        public static IReadOnlyCollection<Elapser> Instances
        {
            get
            {
                return _instances;
            }
        }

        private readonly DateTime _timestamp;
        private readonly double _lifetime;
        private DateTime? _latestTimestamp;

        /// <summary>
        /// Instance owner.
        /// Most of the time a <see cref="Sprites.Sprite"/>, but not always (<see cref="InventoryItem"/> for example).
        /// </summary>
        internal object Owner { get; private set; }
        /// <summary>
        /// <see cref="Enums.ElapserUse"/>
        /// </summary>
        internal Enums.ElapserUse UseId { get; private set; }
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
        /// <param name="owner"><see cref="Owner"/></param>
        /// <param name="useId"><see cref="UseId"/></param>
        internal Elapser(object owner, Enums.ElapserUse useId) : this(owner, useId, double.PositiveInfinity) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="owner"><see cref="Owner"/></param>
        /// <param name="useId"><see cref="UseId"/></param>
        /// <param name="lifetime">Lifetime, in milliseconds.</param>
        internal Elapser(object owner, Enums.ElapserUse useId, double lifetime)
        {
            _timestamp = DateTime.Now;
            _lifetime = lifetime;
            UseId = useId;
            Owner = owner;
            AddOrReplaceInInstances();
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

        /// <summary>
        /// Adds or replaces this instance in <see cref="Instances"/>.
        /// </summary>
        private void AddOrReplaceInInstances()
        {
            int indexOf = _instances.FindIndex(x => x != null && x.UseId == UseId && x.Owner == Owner);
            if (indexOf >= 0)
            {
                _instances[indexOf] = this;
            }
            else
            {
                _instances.Add(this);
            }
        }
    }
}
