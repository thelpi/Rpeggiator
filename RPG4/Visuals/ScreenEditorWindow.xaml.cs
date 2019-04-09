using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;

namespace RPG4.Visuals
{
    /// <summary>
    /// Logique d'interaction pour ScreenEditorWindow.xaml
    /// </summary>
    public partial class ScreenEditorWindow : Window
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ScreenEditorWindow()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            System.Drawing.Color black = System.Drawing.Color.FromArgb(Colors.Black.A, Colors.Black.R, Colors.Black.G, Colors.Black.B);

            string testPath = @"C:\Users\LPI\Desktop\souk\app\test_screen.png";

            List<Rien> rectMaj = new List<Rien>();

            Bitmap bitmap = new Bitmap(testPath);
            for (int y = 0; y < bitmap.Height; y++)
            {
                List<Rien> rect = new List<Rien>();
                bool isINRect = false;
                for (int x = 0; x < bitmap.Width; x++)
                {
                    System.Drawing.Color c = bitmap.GetPixel(x, y);
                    if (c == black)
                    {
                        if (!isINRect)
                        {
                            rect.Add(new Rien
                            {
                                p1 = new System.Windows.Point(x, y),
                                p2 = new System.Windows.Point(x, y)
                            });
                        }
                        else
                        {
                            rect.Last().p2 = new System.Windows.Point(x, y);
                        }
                        isINRect = true;
                    }
                    else
                    {
                        isINRect = false;
                    }
                }

                foreach (var r in rect)
                {
                    var toMerge = rectMaj.Where(rr => rr.p2.Y == y - 1 && rr.p1.X == r.p1.X && rr.p2.X == r.p2.X);
                    if (toMerge.Any())
                    {
                        if (toMerge.Count() > 1)
                            throw new Exception();
                        else
                            toMerge.ElementAt(0).p2 = new System.Windows.Point(toMerge.ElementAt(0).p2.X, r.p2.Y);
                    }
                    else
                    {
                        rectMaj.Add(r);
                    }
                }
            }

            int screenId = 4;
            int bottomScreenId = 1;
            int topScreenId = 1;
            int leftScreenId = 1;
            int rightScreenId = 1;
            Models.Enums.FloorType floorTYpe = Models.Enums.FloorType.Ground;
            string graphicType = nameof(Models.Graphic.PlainBrushGraphic);
            System.Windows.Media.Color backgroundColor = Colors.PapayaWhip;
            double darknessOpacity = 0;
            List<string> otherProperties = new List<string>
            {
                "Floors", "Doors",  "Chests", "Gates", "Enemies",
                "GateTriggers", "Items", "Rifts", "Pits"
            };
            string permStructgraphicType = nameof(Models.Graphic.PlainBrushGraphic);
            System.Windows.Media.Color permStructColor = Colors.Black;

            StringBuilder fullContent = new StringBuilder();

            using (StringWriter sw = new StringWriter(fullContent))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    writer.Formatting = Formatting.Indented;

                    writer.WriteStartObject();
                    writer.WritePropertyName("X");
                    writer.WriteValue(0);
                    writer.WritePropertyName("Y");
                    writer.WriteValue(0);
                    writer.WritePropertyName("Width");
                    writer.WriteValue(bitmap.Width);
                    writer.WritePropertyName("Height");
                    writer.WriteValue(bitmap.Height);
                    writer.WritePropertyName("FloorType");
                    writer.WriteValue(Enum.GetName(typeof(Models.Enums.FloorType), floorTYpe));
                    writer.WritePropertyName("GraphicType");
                    writer.WriteValue(graphicType);
                    writer.WritePropertyName("HexColor");
                    writer.WriteValue(Models.Tools.HexFromColor(backgroundColor));
                    writer.WritePropertyName("AreaDarknessOpacity");
                    writer.WriteValue(darknessOpacity);
                    writer.WritePropertyName("PermanentStructures");
                    writer.WriteStartArray();
                    foreach (Rien rect in rectMaj)
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("X");
                        writer.WriteValue(rect.p1.X);
                        writer.WritePropertyName("Y");
                        writer.WriteValue(rect.p1.Y);
                        writer.WritePropertyName("Width");
                        writer.WriteValue(rect.p2.X - rect.p1.X);
                        writer.WritePropertyName("Height");
                        writer.WriteValue(rect.p2.Y - rect.p1.Y);
                        writer.WritePropertyName("GraphicType");
                        writer.WriteValue(permStructgraphicType);
                        writer.WritePropertyName("HexColor");
                        writer.WriteValue(Models.Tools.HexFromColor(permStructColor));
                        writer.WriteEndObject();
                    }
                    writer.WriteEnd();
                    foreach (string propName in otherProperties)
                    {
                        writer.WritePropertyName(propName);
                        writer.WriteStartArray();
                        writer.WriteEnd();
                    }
                    writer.WritePropertyName("NeighboringScreens");
                    writer.WriteStartObject();
                    writer.WritePropertyName("Right");
                    writer.WriteValue(rightScreenId);
                    writer.WritePropertyName("Left");
                    writer.WriteValue(leftScreenId);
                    writer.WritePropertyName("Top");
                    writer.WriteValue(topScreenId);
                    writer.WritePropertyName("Bottom");
                    writer.WriteValue(bottomScreenId);
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }
            }

            string folderPath = @"C:\Users\LPI\Desktop\souk\app\json\";

            using (StreamWriter sw = new StreamWriter(folderPath + "Screen"  + screenId.ToString() +  ".json"))
            {
                sw.WriteLine(fullContent.ToString());
            }
        }
    }

    public class Rien
    {
        public System.Windows.Point p1 { get; set; }
        public System.Windows.Point p2 { get; set; }
    }
}
