using RPG4.Models.Sprites;
using System;
using System.Collections.Generic;
using System.Windows;

namespace RPG4.Models.Exceptions
{
    /// <summary>
    /// Exception thrown when the detection of overlaps for a <see cref="Sprite"/> is stuck in an infinite loop.
    /// </summary>
    /// <seealso cref="Exception"/>
    public class InfiniteOverlapCheckException : Exception
    {
        /// <summary>
        /// <see cref="Sprite"/> which thrown the exception. 
        /// </summary>
        public Sprite CallerSprite { get; private set; }
        /// <summary>
        /// List of <see cref="Sprite"/> to avoid.
        /// </summary>
        public IReadOnlyCollection<Sprite> Structures { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="callerSprite"><see cref="CallerSprite"/></param>
        /// <param name="structures"><see cref="Structures"/></param>
        /// <param name="newPosition"><paramref name="callerSprite"/> new position.</param>
        public InfiniteOverlapCheckException(Sprite callerSprite, IReadOnlyCollection<Sprite> structures, Point newPosition)
            : this(callerSprite.CopyToPosition(newPosition), structures) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="callerSprite"><see cref="CallerSprite"/></param>
        /// <param name="structures"><see cref="Structures"/></param>
        public InfiniteOverlapCheckException(Sprite callerSprite, IReadOnlyCollection<Sprite> structures)
            : base(Messages.InfiniteOverlapCheckExceptionMessage)
        {
            CallerSprite = callerSprite;
            Structures = structures;
        }
    }
}
