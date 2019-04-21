﻿using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RpeggiatorLib.Renders
{
    /// <summary>
    /// Interface for <see cref="Sprites.Sprite"/> graphic render.
    /// </summary>
    public abstract class Render
    {
        /// <summary>
        /// Gets the render of a <see cref="Sprites.Sprite"/>.
        /// </summary>
        /// <returns><see cref="Brush"/></returns>
        public abstract Brush GetRenderBrush();

        /// <summary>
        /// Computes the <see cref="ImageBrush"/>, optionally depending on a <see cref="Enums.Direction"/>.
        /// </summary>
        /// <param name="imageName">Image name.</param>
        /// <param name="direction">Optionnal <see cref="Enums.Direction"/>.</param>
        /// <returns><see cref="ImageBrush"/></returns>
        protected ImageBrush ComputeImageBrush(string imageName, Enums.Direction direction = Enums.Direction.Right)
        {
            BitmapImage bitmapImage = new BitmapImage();

            string resourcePath = string.Format("{0}Images\\{1}.png", Engine.ResourcesPath, imageName);

            using (FileStream stream = new FileStream(resourcePath, FileMode.Open, FileAccess.Read))
            {
                stream.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }

            double angle = 0;
            bool flip = false;
            switch (direction)
            {
                case Enums.Direction.Bottom:
                    angle = 90;
                    break;
                case Enums.Direction.BottomLeft:
                    angle = 45;
                    flip = true;
                    break;
                case Enums.Direction.BottomRight:
                    angle = 45;
                    break;
                case Enums.Direction.Left:
                    flip = true;
                    break;
                case Enums.Direction.Right:
                    break;
                case Enums.Direction.Top:
                    angle = -90;
                    break;
                case Enums.Direction.TopLeft:
                    angle = -45;
                    flip = true;
                    break;
                case Enums.Direction.TopRight:
                    angle = -45;
                    break;
            }

            return new ImageBrush
            {
                ImageSource = bitmapImage,
                Stretch = Stretch.UniformToFill,
                RelativeTransform = new TransformGroup
                {
                    Children = new TransformCollection
                    {
                        new RotateTransform
                        {
                            CenterX = 0.5,
                            CenterY = 0.5,
                            Angle = angle
                        }, new ScaleTransform
                        {
                            CenterX = 0.5,
                            CenterY = 0.5,
                            ScaleX = flip ? -1 : 1
                        }
                    }
                }
            };
        }
    }
}
