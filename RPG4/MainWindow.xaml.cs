using Newtonsoft.Json;
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
        private const double KICK_SIZE_RATIO = 2;

        private const string KICK_TAG = "KICK";
        private const string ENEMY_TAG = "ENEMY";
        private const string WALL_TAG = "WALL";
        private const string WALL_TRIGGER_TAG = "WALL_TRIGGER";
        private const string UNIQUE_TIMESTAMP_PATTERN = "fffffff";

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
                new Player(initialCanvasLeft, initialCanvasTop, SPRITE_SIZE_X, SPRITE_SIZE_Y,
                    DISTANCE_BY_SECOND * (DISTANCE_BY_SECOND / (double)1000), KICK_SIZE_RATIO), modelJson);

            cvsMain.Height = modelJson.AreaHeight;
            cvsMain.Width = modelJson.AreaWidth;
            rctMe.Height = SPRITE_SIZE_Y;
            rctMe.Width = SPRITE_SIZE_X;

            RedrawMeSprite(initialCanvasTop, initialCanvasLeft, false);
            RedrawPngSprite(_model.Enemies);
            _model.ConcreteWalls.ForEach(DrawWall);

            _model.WallTriggers.ForEach(DrawWallTrigger);

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

            // dessine les sprites enemies
            Dispatcher.Invoke(new Action<List<Enemy>>(delegate (List<Enemy> pngs) { RedrawPngSprite(pngs); }), _model.Enemies);

            // contrôle le contact joueur / enemies
            if (_model.MeCollideToPng || _model.MeCollideToWall)
            {
                Dispatcher.Invoke(new Action(delegate() { MessageBox.Show("You die !"); }));
                _timer.Stop();
            }

            // redessine les murs
            Dispatcher.Invoke(new Action<List<Wall>>(delegate (List<Wall> walls)
            {
                ClearCanvasByTag(WALL_TAG);
                walls.ForEach(DrawWall);
            }), _model.ConcreteWalls);

            // redessine les triggers
            Dispatcher.Invoke(new Action<List<WallTrigger>>(delegate (List<WallTrigger> tw)
            {
                ClearCanvasByTag(WALL_TRIGGER_TAG);
                tw.ForEach(DrawWallTrigger);
            }), _model.WallTriggers);

            _timerIsIn = false;
        }

        private void RedrawMeSprite(double top, double left, bool kick)
        {
            rctMe.SetValue(Canvas.TopProperty, top);
            rctMe.SetValue(Canvas.LeftProperty, left);

            Panel.SetZIndex(rctMe, 0);

            // cleans previous kick
            ClearCanvasByTag(KICK_TAG);

            if (kick)
            {
                var sp = new SizedPoint(left - KICK_THICK_X,
                    top - KICK_THICK_Y,
                    SPRITE_SIZE_X * KICK_SIZE_RATIO,
                    SPRITE_SIZE_Y * KICK_SIZE_RATIO);

                DrawSizedPoint(sp, Brushes.DarkViolet, KICK_TAG, 1);
            }
        }

        private void RedrawPngSprite(List<Enemy> enemies)
        {
            ClearCanvasByTag(ENEMY_TAG);
            foreach (Enemy enemy in enemies)
            {
                DrawSizedPoint(enemy, Brushes.Blue, ENEMY_TAG);
            }
        }

        private void DrawWall(Wall wall)
        {
            if (wall.Concrete)
            {
                DrawSizedPoint(wall, Brushes.Black, WALL_TAG);
            }
        }

        private void DrawWallTrigger(WallTrigger wt)
        {
            DrawSizedPoint(wt, wt.CurrentlyOn ? Brushes.Yellow : Brushes.Orange, WALL_TRIGGER_TAG);
        }

        /// <summary>
        /// Draws a <see cref="SizedPoint"/> inside the main canvas
        /// </summary>
        /// <param name="sp"><see cref="SizedPoint"/></param>
        /// <param name="b"><see cref="Brush"/></param>
        /// <param name="tag">The tag, which depends on the subtype.</param>
        /// <param name="zIndex">Optionnal; Z index value.</param>
        private void DrawSizedPoint(SizedPoint sp, Brush b, string tag, int? zIndex = null)
        {
            Rectangle rct = new Rectangle
            {
                Fill = b,
                Width = sp.Width,
                Height = sp.Height,
                Uid = string.Concat(tag, DateTime.Now.ToString(UNIQUE_TIMESTAMP_PATTERN))
            };
            rct.SetValue(Canvas.TopProperty, sp.Y);
            rct.SetValue(Canvas.LeftProperty, sp.X);

            if (zIndex.HasValue)
            {
                Panel.SetZIndex(rctMe, zIndex.Value);
            }

            cvsMain.Children.Add(rct);
        }

        // cleans previous elements for the specified tag
        private void ClearCanvasByTag(string tag)
        {
            for (int i = cvsMain.Children.Count - 1; i >= 0; i--)
            {
                if (cvsMain.Children[i].Uid.StartsWith(tag))
                {
                    cvsMain.Children.RemoveAt(i);
                }
            }
        }
    }
}
