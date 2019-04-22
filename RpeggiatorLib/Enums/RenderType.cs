namespace RpeggiatorLib.Enums
{
    /// <summary>
    /// <see cref="Renders.Render"/> sub-types enumeration.
    /// </summary>
    public enum RenderType
    {
        /// <summary>
        /// Plain color.
        /// </summary>
        Plain = 1,
        /// <summary>
        /// Static image.
        /// </summary>
        Image,
        /// <summary>
        /// Image used as mosaic.
        /// </summary>
        ImageMosaic,
        /// <summary>
        /// Image which follows <see cref="Sprites.Sprite"/> <see cref="Direction"/>.
        /// </summary>
        ImageDirection,
        /// <summary>
        /// Combination of <see cref="ImageDirection"/> and <see cref="ImageMosaic"/>
        /// </summary>
        ImageMosaicDirection,
        /// <summary>
        /// Several images displayed sequentially.
        /// </summary>
        ImageAnimated
    }
}
