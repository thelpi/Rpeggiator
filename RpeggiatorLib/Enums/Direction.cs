namespace RpeggiatorLib.Enums
{
    /// <summary>
    /// Enumeration of directions through screens
    /// </summary>
    /// <remarks>If the sum of two direction values (as <see cref="int"/>) is 9, then it's opposite directions.</remarks>
    public enum Direction
    {
        /// <summary>
        /// Right
        /// </summary>
        Right = 1,
        /// <summary>
        /// Top-right
        /// </summary>
        TopRight,
        /// <summary>
        /// Top
        /// </summary>
        Top,
        /// <summary>
        /// Top-left
        /// </summary>
        TopLeft,
        /// <summary>
        /// Bottom-right
        /// </summary>
        BottomRight,
        /// <summary>
        /// Bottom
        /// </summary>
        Bottom,
        /// <summary>
        /// Bottom-left
        /// </summary>
        BottomLeft,
        /// <summary>
        /// Left
        /// </summary>
        Left
    }
}
