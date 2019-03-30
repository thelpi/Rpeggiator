using System;

namespace RPG4.Abstraction
{
    /// <summary>
    /// Lifetime event manager.
    /// </summary>
    public class Elapser
    {
        private DateTime _timestamp;
        private double _lifetime;

        /// <summary>
        /// Inferred; indicates if the instance is elapsed.
        /// </summary>
        public bool Elapsed
        {
            get
            {
                return (DateTime.Now - _timestamp).TotalMilliseconds >= _lifetime;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="lifetime">Lifetime, in milliseconds.</param>
        public Elapser(double lifetime)
        {
            _timestamp = DateTime.Now;
            _lifetime = lifetime;
        }
    }
}
