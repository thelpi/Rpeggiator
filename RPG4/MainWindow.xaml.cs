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
        private const string KICK_TAG = "KICK";
        private const string ENEMY_TAG = "ENEMY";
        private const string WALL_TAG = "WALL";
        private const string WALL_TRIGGER_TAG = "WALL_TRIGGER";
        private const string UNIQUE_TIMESTAMP_PATTERN = "fffffff";

        private Timer _timer;
        private volatile bool _timerIsIn;
        private AbstractEngine _engine;

        public MainWindow()
        {
            InitializeComponent();

            double initialCanvasTop = (Constants.AREA_HEIGHT / 2) - (Constants.SPRITE_SIZE_Y / 2);
            double initialCanvasLeft = (Constants.AREA_WIDTH / 2) - (Constants.SPRITE_SIZE_X / 2);

            Player player = new Player(initialCanvasLeft, initialCanvasTop, Constants.SPRITE_SIZE_X, Constants.SPRITE_SIZE_Y,
                Constants.PLAYER_SPEED * (Constants.PLAYER_SPEED / (double)1000), Constants.KICK_SIZE_RATIO);

            _engine = new AbstractEngine(player, 1);

            cvsMain.Height = Constants.AREA_HEIGHT;
            cvsMain.Width = Constants.AREA_WIDTH;
            rctMe.Height = Constants.SPRITE_SIZE_Y;
            rctMe.Width = Constants.SPRITE_SIZE_X;

            RedrawMeSprite(initialCanvasTop, initialCanvasLeft, false);
            RedrawPngSprite(_engine.Enemies);
            _engine.ConcreteWalls.ForEach(DrawWall);

            _engine.WallTriggers.ForEach(DrawWallTrigger);

            _timer = new Timer(Constants.REFRESH_DELAY_MS);
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
            _engine.CheckEngineAtTick(pressedKeys);

            Dispatcher.Invoke((delegate()
            {
                // dessine le sprite joueur
                RedrawMeSprite(_engine.Player.Y, _engine.Player.X, _engine.Player.IsHitting);
                // dessine les sprites enemies
                RedrawPngSprite(_engine.Enemies);
                // contrôle le contact joueur / enemies
                if (_engine.MeCollideToPng || _engine.MeCollideToWall)
                {
                    MessageBox.Show("You die !");
                    _timer.Stop();
                }
                // redessine les murs
                ClearCanvasByTag(WALL_TAG);
                _engine.ConcreteWalls.ForEach(DrawWall);
                // redessine les triggers
                ClearCanvasByTag(WALL_TRIGGER_TAG);
                _engine.WallTriggers.ForEach(DrawWallTrigger);
            }));

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
                var sp = new SizedPoint(left - Constants.KICK_THICK_X,
                    top - Constants.KICK_THICK_Y,
                    Constants.SPRITE_SIZE_X * Constants.KICK_SIZE_RATIO,
                    Constants.SPRITE_SIZE_Y * Constants.KICK_SIZE_RATIO);

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
            DrawSizedPoint(wt, wt.IsActivated ? Brushes.Yellow : Brushes.Orange, WALL_TRIGGER_TAG);
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
