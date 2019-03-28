using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace RPG4.Abstraction.Graphic
{
    /*
    /// <summary>
    /// Represents an image as a way to render a <see cref="Sprites.Sprite"/>.
    /// </summary>
    /// <seealso cref="ISpriteGraphic"/>
    public class ImageGraphic : ISpriteGraphic
    {
        private string _imagePath;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="imagePath">Image path.</param>
        public ImageGraphic(string imagePath)
        {
            _imagePath = imagePath;
        }

        /// <inheritdoc />
        public T TransformToGraphic<T>()
        {
            return (T)Convert.ChangeType(new Image
            {
                Source = new BitmapImage(new Uri(_imagePath, UriKind.Relative)),
                Stretch = System.Windows.Media.Stretch.Fill
            }, typeof(T));
        }
    }
    */
}
