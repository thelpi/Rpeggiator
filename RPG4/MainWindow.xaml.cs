using RPG4.Abstractions;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RPG4
{
    /// <summary>
    /// Delegate to pass the pressed keys of the keayboard to the engine.
    /// </summary>
    /// <returns>A method which returns pressed keyboard's keys.</returns>
    public delegate KeyPress KeyPressHandler();

    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string KICK_TAG = "KICK";
        private const string ENEMY_TAG = "ENEMY";
        private const string WALL_TAG = "WALL";
        private const string GATE_TRIGGER_TAG = "GATE_TRIGGER";
        private const string ITEM_TAG = "ITEM";
        private const string BOMB_TAG = "BOMB";
        private const string BOMB_HALO_TAG = "BOMB_HALO_TAG";
        private const string UNIQUE_TIMESTAMP_PATTERN = "fffffff";

        private Timer _timer;
        private volatile bool _timerIsIn;
        private AbstractEngine _engine;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _engine = new AbstractEngine(1);

            cvsMain.Height = _engine.AreaHeight;
            cvsMain.Width = _engine.AreaWidth;

            // size of the player never change
            rctMe.Height = _engine.Player.Height;
            rctMe.Width = _engine.Player.Width;

            DrawPlayer();
            DrawEnemies();
            DrawWalls();
            DrawWallTriggers();
            DrawItems();

            _timer = new Timer(1000 / Constants.FPS);
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

            // check pressed keys
            var pressedKeys = (KeyPress)Dispatcher.Invoke(new KeyPressHandler(delegate()
            {
                int? inventorySlotId = null;
                for (int i = 0; i < 10; i++)
                {
                    if (Keyboard.IsKeyDown((Key)Enum.Parse(typeof(Key), string.Format("D{0}", i))))
                    {
                        inventorySlotId = i;
                        break;
                    }
                }
                return new KeyPress(
                    Keyboard.IsKeyDown(Key.Up),
                    Keyboard.IsKeyDown(Key.Down),
                    Keyboard.IsKeyDown(Key.Right),
                    Keyboard.IsKeyDown(Key.Left),
                    Keyboard.IsKeyDown(Key.Space),
                    inventorySlotId
                );
            }));

            // recompute everything
            _engine.CheckEngineAtTick(pressedKeys);

            Dispatcher.Invoke((delegate()
            {
                if (_engine.Player.NewScreenEntrance.HasValue)
                {
                    cvsMain.Height = _engine.AreaHeight;
                    cvsMain.Width = _engine.AreaWidth;
                }

                DrawPlayer();
                DrawEnemies();
                DrawWalls();
                DrawWallTriggers();
                DrawItems();
                DrawBombs();
                
                if (_engine.Player.CheckDeath(_engine))
                {
                    MessageBox.Show("You die !");
                    _timer.Stop();
                }
            }));

            _timerIsIn = false;
        }

        // draws the player
        private void DrawPlayer()
        {
            rctMe.SetValue(Canvas.TopProperty, _engine.Player.Y);
            rctMe.SetValue(Canvas.LeftProperty, _engine.Player.X);

            rctMe.Fill = _engine.Player.IsRecovering ? Brushes.Brown : Brushes.Red;

            Panel.SetZIndex(rctMe, 0);

            // cleans previous kick
            ClearCanvasByTag(KICK_TAG);

            if (_engine.Player.HitHalo.DisplayHalo)
            {
                DrawSizedPoint(_engine.Player.HitHalo.Halo, Brushes.DarkViolet, KICK_TAG, 1);
            }
        }

        // draws enemies
        private void DrawEnemies()
        {
            ClearCanvasByTag(ENEMY_TAG);
            foreach (Enemy enemy in _engine.Enemies)
            {
                DrawSizedPoint(enemy, Brushes.Blue, ENEMY_TAG);
            }
        }

        // draws walls
        private void DrawWalls()
        {
            ClearCanvasByTag(WALL_TAG);
            foreach (var s in _engine.SolidStructures)
            {
                DrawSizedPoint(s, Brushes.Black, WALL_TAG);
            }
        }

        // draws wall triggers
        private void DrawWallTriggers()
        {
            ClearCanvasByTag(GATE_TRIGGER_TAG);
            foreach (var gt in _engine.GateTriggers)
            {
                DrawSizedPoint(gt, gt.IsActivated ? Brushes.Yellow : Brushes.Orange, GATE_TRIGGER_TAG);
            }
        }

        // draws items
        private void DrawItems()
        {
            ClearCanvasByTag(ITEM_TAG);
            foreach (var it in _engine.Items)
            {
                DrawSizedPoint(it, Brushes.LightBlue, ITEM_TAG);
            }
        }

        private void DrawBombs()
        {
            ClearCanvasByTag(BOMB_TAG);
            foreach (var bo in _engine.Bombs)
            {
                DrawSizedPoint(bo, Brushes.LightBlue, BOMB_TAG);

                if (bo.DisplayHalo)
                {
                    DrawSizedPoint(bo.Halo, Brushes.SandyBrown, BOMB_HALO_TAG, 1);
                }
            }
        }

        // draws a SizedPoint inside the main canvas
        private void DrawSizedPoint(Sprite sp, Brush b, string tag, int? zIndex = null)
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
