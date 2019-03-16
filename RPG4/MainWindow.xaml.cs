using RPG4.Abstractions;
using System;
using System.Collections.Generic;
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
        private const int CANVAS_HEIGHT = 480;
        private const int CANVAS_WIDTH = 640;
        private const int KICK_DELAY_MS = 100;
        private const double KICK_SIZE_RATIO = 2;
        private const string KICK_TAG = "KICK";
        private const string PNG_TAG = "PNG";

        private readonly double KICK_THICK_X = ((KICK_SIZE_RATIO - 1) / 2) * SPRITE_SIZE_X;
        private readonly double KICK_THICK_Y = ((KICK_SIZE_RATIO - 1) / 2) * SPRITE_SIZE_Y;

        private Timer _timer;
        private volatile bool _timerIsIn;
        private AbstractEngine _model;
        private volatile int _kickTimeCumulMs = -1;

        public MainWindow()
        {
            InitializeComponent();
            
            double initialCanvasTop = (CANVAS_HEIGHT / 2) - (SPRITE_SIZE_Y / 2);
            double initialCanvasLeft = (CANVAS_WIDTH / 2) - (SPRITE_SIZE_X / 2);

            // à gérer différemment
            const double PNG_INITIAL_X_Y = 50;
            const int PNG_KICK_TOLERANCE = 3;

            var pngs = new List<PngBehavior>();
            pngs.Add(new PngBehavior(PNG_INITIAL_X_Y, PNG_INITIAL_X_Y, SPRITE_SIZE_X, SPRITE_SIZE_Y, DISTANCE_BY_SECOND / 2,
                new SizedPoint(PNG_INITIAL_X_Y, PNG_INITIAL_X_Y, 600 - PNG_INITIAL_X_Y, 450 - PNG_INITIAL_X_Y), true, PNG_KICK_TOLERANCE));

            // experimental
            var wall1 = new SizedPoint(50, 300, 300 - 50, 350 - 300);
            var wall2 = new SizedPoint(100, 260, 150 - 100, 400 - 260);

            _model = new AbstractEngine(
                new PlayerBehavior(initialCanvasLeft, initialCanvasTop, SPRITE_SIZE_X, SPRITE_SIZE_Y,
                    DISTANCE_BY_SECOND * (DISTANCE_BY_SECOND / (double)1000), KICK_SIZE_RATIO),
                CANVAS_WIDTH, CANVAS_HEIGHT, new List<SizedPoint> { wall1, wall2 }, pngs);

            cvsMain.Height = CANVAS_HEIGHT;
            cvsMain.Width = CANVAS_WIDTH;
            rctMe.Height = SPRITE_SIZE_Y;
            rctMe.Width = SPRITE_SIZE_X;

            RedrawMeSprite(initialCanvasTop, initialCanvasLeft, false);
            RedrawPngSprite(pngs);
            DrawWall(wall1);
            DrawWall(wall2);

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
                    _kickTimeCumulMs >= 0
                );
            }));

            // recalcule les positions joueur + pngs
            // actionne aussi le kick
            _model.CheckEngineAtTick(pressedKeys);

            // dessine le sprite joueur
            Dispatcher.Invoke(new Action<SizedPoint, bool>(delegate(SizedPoint pt, bool kick) { RedrawMeSprite(pt.Y, pt.X, kick); }), _model.Player, _kickTimeCumulMs > 0);

            // dessine les sprites png
            Dispatcher.Invoke(new Action<List<PngBehavior>>(delegate (List<PngBehavior> pngs) { RedrawPngSprite(pngs); }), _model.Pngs);

            // contrôle le contact joueur / png
            if (_model.MeCollideToPng)
            {
                Dispatcher.Invoke(new Action(delegate() { MessageBox.Show("You die !"); }));
                _timer.Stop();
            }

            // gère le temps d'effet du kick
            if (_kickTimeCumulMs >= KICK_DELAY_MS)
            {
                _kickTimeCumulMs = -1;
            }
            else if (_kickTimeCumulMs >= 0)
            {
                _kickTimeCumulMs += REFRESH_DELAY_MS;
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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && _kickTimeCumulMs < 0)
            {
                _kickTimeCumulMs = 0;
            }
        }
    }
}
