﻿using System;

namespace RPG4.Models
{
    /// <summary>
    /// Lifetime event manager.
    /// </summary>
    public class Elapser
    {
        private DateTime _timestamp;
        private double _lifetime;
        private DateTime? _latestTimestamp;

        /// <summary>
        /// Inferred; indicates if the instance is elapsed.
        /// </summary>
        public bool Elapsed
        {
            get
            {
                return (DateTime.Now - _timestamp).TotalMilliseconds.GreaterEqual(_lifetime);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Elapser() : this(double.PositiveInfinity) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="lifetime">Lifetime, in milliseconds.</param>
        public Elapser(double lifetime)
        {
            _timestamp = DateTime.Now;
            _lifetime = lifetime;
        }

        /// <summary>
        /// Gets the distance between two calls.
        /// </summary>
        /// <param name="pixelsBySecond">Speed; pixels by second.</param>
        /// <returns>The distance, in pixels.</returns>
        public double Distance(double pixelsBySecond)
        {
            double distance = (DateTime.Now - (_latestTimestamp ?? DateTime.Now)).TotalMilliseconds * (pixelsBySecond / 1000);
            _latestTimestamp = DateTime.Now;
            return distance;
        }

        /// <summary>
        /// Reset the instance.
        /// </summary>
        public void Reset()
        {
            _latestTimestamp = null;
        }
    }
}
