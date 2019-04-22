using System.Windows.Media;

namespace RpeggiatorLib.Renders
{
    /// <summary>
    /// Represents a <see cref="Sprites.Sprite"/> render using several bitmap images displayed sequentially.
    /// </summary>
    /// <seealso cref="Render"/>
    public class ImageAnimatedRender : Render
    {
        // Current index in "_imagesPaths" and "_brushs" lists.
        private int _currentIndex;
        // List of images paths.
        private readonly string[] _imagesPaths;
        // List of brushes generated from each image.
        private readonly Brush[] _brushs;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="imagesPaths"><see cref="_imagesPaths"/></param>
        internal ImageAnimatedRender(params string[] imagesPaths)
        {
            _imagesPaths = imagesPaths;
            _brushs = new Brush[_imagesPaths.Length];
            _currentIndex = 0;
        }

        /// <inheritdoc />
        public override Brush GetRenderBrush()
        {
            if (_brushs[_currentIndex] == null)
            {
                //_brushs[_currentIndex] = ComputeImageBrush(_imagesPaths[_currentIndex]);
            }

            return _brushs[_currentIndex];
        }

        /// <summary>
        /// Moves the index to next image to display.
        /// </summary>
        internal void ForwardIndex()
        {
            _currentIndex = _currentIndex == _imagesPaths.Length - 1 ? 0 : _currentIndex++;
        }
    }
}
