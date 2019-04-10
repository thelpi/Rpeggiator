using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RPG4
{
    internal static class Tools
    {
        private static Random _rdm = new Random(DateTime.Now.Millisecond);

        private static Dictionary<RpeggiatorLib.Sprites.Sprite, Brush> _toto =
            new Dictionary<RpeggiatorLib.Sprites.Sprite, Brush>();

        public static Brush GetRenderingBrush(RpeggiatorLib.Sprites.Sprite sprite)
        {
            if (!_toto.ContainsKey(sprite))
            {
                _toto.Add(sprite, new SolidColorBrush(Color.FromRgb((byte)_rdm.Next(0, byte.MaxValue + 1),
                    (byte)_rdm.Next(0, byte.MaxValue + 1),
                    (byte)_rdm.Next(0, byte.MaxValue + 1))));
            }

            return _toto[sprite];
            /*if (!_toto.ContainsKey(graphic))
            {
                if (graphic.GetType() == typeof(RpeggiatorLib.Graphic.ImageBrushGraphic))
                {
                    BitmapImage bitmapImage = new BitmapImage();

                    using (MemoryStream memory = new MemoryStream())
                    {
                        System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject((graphic as RpeggiatorLib.Graphic.ImageBrushGraphic).ImagePath);
                        bitmap.Save(memory, ImageFormat.Png);
                        memory.Position = 0;
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memory;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                    }

                    _toto.Add(graphic, new ImageBrush
                    {
                        ImageSource = bitmapImage,
                        Stretch = Stretch.Fill
                    });
                }
                else if (graphic.GetType() == typeof(RpeggiatorLib.Graphic.PlainBrushGraphic))
                {
                    _toto.Add(graphic, new SolidColorBrush((Color)ColorConverter.ConvertFromString((graphic as RpeggiatorLib.Graphic.PlainBrushGraphic).HexadecimalColor)));
                }
                else
                {
                    throw new System.NotImplementedException();
                }
            }
            return _toto[graphic];*/
        }
    }
}
