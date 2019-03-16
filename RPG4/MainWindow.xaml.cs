using Newtonsoft.Json;
using RPG4.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RPG4
{
    public delegate KeyPress KeyPressHandler();

    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int REFRESH_DELAY_MS = 50;
        private const int DISTANCE_BY_SECOND = 100;
        private const int SPRITE_SIZE_X = 40;
        private const int SPRITE_SIZE_Y = 40;
        private const double KICK_SIZE_RATIO = 2;
        private const string KICK_TAG = "KICK";
        private const string PNG_TAG = "PNG";

        private readonly double KICK_THICK_X = ((KICK_SIZE_RATIO - 1) / 2) * SPRITE_SIZE_X;
        private readonly double KICK_THICK_Y = ((KICK_SIZE_RATIO - 1) / 2) * SPRITE_SIZE_Y;

        private Timer _timer;
        private volatile bool _timerIsIn;
        private AbstractEngine _model;
        //private volatile int _kickTimeCumulMs = -1;

        public MainWindow()
        {
            InitializeComponent();

            dynamic modelJson = JsonConvert.DeserializeObject(Properties.Resources.Screen1);

            double initialCanvasTop = (modelJson.AreaHeight / 2) - (SPRITE_SIZE_Y / 2);
            double initialCanvasLeft = (modelJson.AreaWidth / 2) - (SPRITE_SIZE_X / 2);

            _model = new AbstractEngine(
                new PlayerBehavior(initialCanvasLeft, initialCanvasTop, SPRITE_SIZE_X, SPRITE_SIZE_Y,
                    DISTANCE_BY_SECOND * (DISTANCE_BY_SECOND / (double)1000), KICK_SIZE_RATIO), modelJson);

            cvsMain.Height = modelJson.AreaHeight;
            cvsMain.Width = modelJson.AreaWidth;
            rctMe.Height = SPRITE_SIZE_Y;
            rctMe.Width = SPRITE_SIZE_X;

            RedrawMeSprite(initialCanvasTop, initialCanvasLeft, false);
            RedrawPngSprite(_model.Pngs);
            _model.Walls.ForEach(DrawWall);

            _timer = new Timer(REFRESH_DELAY_MS);
            _timer.Elapsed += OnTick;
            _timer.Start();
        }

        private void OnTick(object sender, ElapsedEventArgs e)
        {
            if (_timerIsIn)
            {
                return;
            }

            _timerIsIn = true;

            // détecte la direction courante du joueur
            var pressedKeys = (KeyPress)Dispatcher.Invoke(new KeyPressHandler(delegate()
            {
                return new KeyPress(
                    Keyboard.IsKeyDown(Key.Up),
                    Keyboard.IsKeyDown(Key.Down),
                    Keyboard.IsKeyDown(Key.Right),
                    Keyboard.IsKeyDown(Key.Left),
                    Keyboard.IsKeyDown(Key.Space)
                );
            }));

            // recalcule les positions joueur + pngs
            // actionne aussi le kick
            _model.CheckEngineAtTick(pressedKeys);

            // dessine le sprite joueur
            Dispatcher.Invoke(new Action<SizedPoint, bool>(delegate(SizedPoint pt, bool kick) { RedrawMeSprite(pt.Y, pt.X, kick); }), _model.Player, _model.IsKicking);

            // dessine les sprites png
            Dispatcher.Invoke(new Action<List<PngBehavior>>(delegate (List<PngBehavior> pngs) { RedrawPngSprite(pngs); }), _model.Pngs);

            // contrôle le contact joueur / png
            if (_model.MeCollideToPng)
            {
                Dispatcher.Invoke(new Action(delegate() { MessageBox.Show("You die !"); }));
                _timer.Stop();
            }

            _timerIsIn = false;
        }

        private void RedrawMeSprite(double top, double left, bool kick)
        {
            rctMe.SetValue(Canvas.TopProperty, top);
            rctMe.SetValue(Canvas.LeftProperty, left);

            // nettoie toute trace d'un précédent kick
            Panel.SetZIndex(rctMe, 0);
            ClearCanvasByTag(KICK_TAG);

            if (kick)
            {
                Rectangle kickShape = new Rectangle
                {
                    Width = SPRITE_SIZE_X * KICK_SIZE_RATIO,
                    Height = SPRITE_SIZE_Y * KICK_SIZE_RATIO,
                    Fill = Brushes.DarkViolet
                };
                kickShape.Uid = string.Concat(KICK_TAG, DateTime.Now.ToString("fffffff"));

                kickShape.SetValue(Canvas.TopProperty, top - KICK_THICK_Y);
                kickShape.SetValue(Canvas.LeftProperty, left - KICK_THICK_X);

                Panel.SetZIndex(rctMe, 1);

                cvsMain.Children.Add(kickShape);
            }
        }

        private void RedrawPngSprite(List<PngBehavior> pngs)
        {
            ClearCanvasByTag(PNG_TAG);

            foreach (var png in pngs)
            {
                Rectangle pngSprite = new Rectangle
                {
                    Fill = Brushes.Blue,
                    Width = png.Width,
                    Height = png.Height,
                    Uid = string.Concat(PNG_TAG, DateTime.Now.ToString("fffffff"))
                };
                pngSprite.SetValue(Canvas.LeftProperty, png.X);
                pngSprite.SetValue(Canvas.TopProperty, png.Y);

                cvsMain.Children.Add(pngSprite);
            }
        }

        private void ClearCanvasByTag(string tag)
        {
            // nettoie les précédents éléments pour ce tag
            for (int i = cvsMain.Children.Count - 1; i >= 0; i--)
            {
                if (cvsMain.Children[i].Uid.StartsWith(tag))
                {
                    cvsMain.Children.RemoveAt(i);
                }
            }
        }

        private void DrawWall(SizedPoint rectPt)
        {
            Rectangle rctWall = new Rectangle
            {
                Fill = Brushes.Black,
                Width = rectPt.BottomRightX - rectPt.X,
                Height = rectPt.BottomRightY - rectPt.Y
            };
            rctWall.SetValue(Canvas.TopProperty, rectPt.Y);
            rctWall.SetValue(Canvas.LeftProperty, rectPt.X);
            cvsMain.Children.Add(rctWall);
        }
    }
}
