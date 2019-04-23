namespace RpeggiatorLib.Enums
{
    /// <summary>
    /// <see cref="Renders.IRender"/> sub-types enumeration.
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
        /// Several images displayed sequentially.
        /// </summary>
        ImageAnimated,
        /// <summary>
        /// Combination of <see cref="ImageAnimated"/> and <see cref="ImageMosaic"/>.
        /// </summary>
        ImageMosaicAnimated
    }
}
