namespace RpeggiatorLib.Graphic
{
    /// <summary>
    /// Represents an image brush as a way to render a <see cref="Sprites.Sprite"/>.
    /// </summary>
    /// <seealso cref="ISpriteGraphic"/>
    public class ImageBrushGraphic : ISpriteGraphic
    {
        /// <summary>
        /// Path to image.
        /// </summary>
        public string ImagePath { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="imagePath">Image name without extension in resources folder.</param>
        public ImageBrushGraphic(string imagePath)
        {
            ImagePath = imagePath;
        }
    }
}
