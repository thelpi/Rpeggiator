using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RPG4.Models.Graphic
{
    /// <summary>
    /// Represents an image brush as a way to render a <see cref="Sprites.Sprite"/>.
    /// </summary>
    /// <seealso cref="ISpriteGraphic"/>
    public class ImageBrushGraphic : ISpriteGraphic
    {
        // Path to image.
        private string _imagePath;
        // Image brush.
        private ImageBrush _brush = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="imagePath">Image name without extension in resources folder.</param>
        public ImageBrushGraphic(string imagePath)
        {
            _imagePath = imagePath;
        }

        /// <inheritdoc />
        public Brush GetRenderingBrush()
        {
            if (_brush == null)
            {
                BitmapImage bitmapImage = new BitmapImage();

                using (MemoryStream memory = new MemoryStream())
                {
                    var bitmap = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject(_imagePath);
                    bitmap.Save(memory, ImageFormat.Png);
                    memory.Position = 0;
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                }

                _brush = new ImageBrush
                {
                    ImageSource = bitmapImage,
                    Stretch = Stretch.Fill
                };
            }

            return _brush;
        }
    }
}
