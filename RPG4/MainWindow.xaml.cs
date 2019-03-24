using RPG4.Abstractions;
using System;
using System.Linq;
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
        private const string GATE_TAG = "GATE";
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
            rctPlayer.Height = _engine.Player.Height;
            rctPlayer.Width = _engine.Player.Width;
            
            DrawWalls();
            RefreshSprites();
            RefreshMenu();

            _timer = new Timer(1000 / Constants.FPS);
            _timer.Elapsed += NewFrame;
            _timer.Start();
        }

        private void NewFrame(object sender, ElapsedEventArgs e)
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
            _engine.CheckEngineAtNewFrame(pressedKeys);

            Dispatcher.Invoke((delegate()
            {
                if (_engine.Player.NewScreenEntrance.HasValue)
                {
                    cvsMain.Height = _engine.AreaHeight;
                    cvsMain.Width = _engine.AreaWidth;
                    DrawWalls();
                }

                RefreshSprites();
                RefreshMenu();

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
            rctPlayer.SetValue(Canvas.TopProperty, _engine.Player.Y);
            rctPlayer.SetValue(Canvas.LeftProperty, _engine.Player.X);

            rctPlayer.Fill = _engine.Player.IsRecovering ? Brushes.Brown : Brushes.Red;

            // cleans previous kick
            ClearCanvasByTag(KICK_TAG);

            if (_engine.Player.HitHalo.Active)
            {
                Panel.SetZIndex(rctPlayer, 1);
                DrawSizedPoint(_engine.Player.HitHalo, Brushes.DarkViolet, KICK_TAG, 0);
            }
            else
            {
                Panel.SetZIndex(rctPlayer, 0);
            }
        }

        // draws walls
        private void DrawWalls()
        {
            ClearCanvasByTag(WALL_TAG);
            foreach (var s in _engine.Walls)
            {
                DrawSizedPoint(s, Brushes.Black, WALL_TAG);
            }
        }

        // draws a SizedPoint inside the main canvas
        private void DrawSizedPoint(Sprite sp, Brush b, string tag, int zIndex = 0)
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

            Panel.SetZIndex(rct, zIndex);

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

        private void RefreshSprites()
        {
            DrawPlayer();

            ClearCanvasByTag(ITEM_TAG);
            ClearCanvasByTag(GATE_TRIGGER_TAG);
            ClearCanvasByTag(ENEMY_TAG);
            ClearCanvasByTag(BOMB_TAG);
            ClearCanvasByTag(GATE_TAG);
            foreach (var sprite in _engine.Sprites)
            {
                string tag = null;
                Brush brush = null;
                HaloSprite halo = null;
                string haloTag = null;
                Brush haloBrush = null;

                if (sprite.GetType() == typeof(Enemy))
                {
                    tag = ENEMY_TAG;
                    brush = Brushes.Blue;
                }
                else if (sprite.GetType() == typeof(Bomb))
                {
                    tag = BOMB_TAG;
                    brush = Brushes.LightBlue;
                    halo = (sprite as Bomb).ExplosionHalo;
                    haloTag = BOMB_HALO_TAG;
                    haloBrush = Brushes.SandyBrown;
                }
                else if (sprite.GetType() == typeof(Gate))
                {
                    tag = GATE_TAG;
                    brush = Brushes.DarkGray;
                }
                else if (sprite.GetType() == typeof(GateTrigger))
                {
                    tag = GATE_TRIGGER_TAG;
                    brush = (sprite as GateTrigger).IsActivated ? Brushes.Yellow : Brushes.Orange;
                }
                else if (sprite.GetType() == typeof(FloorItem))
                {
                    tag = ITEM_TAG;
                    brush = Brushes.LightBlue;
                }

                DrawSizedPoint(sprite, brush, tag, halo != null && halo.Active ? 1 : 0);
                if (halo != null && halo.Active)
                {
                    DrawSizedPoint(halo, haloBrush, haloTag, 0);
                }
            }
        }

        private void RefreshMenu()
        {
            pgbPlayerLifePoints.Maximum = _engine.Player.MaximalLifePoints;
            pgbPlayerLifePoints.Value = _engine.Player.CurrentLifePoints;
            for (int i = 0; i < 10; i++)
            {
                var itemSlotRct = (Rectangle)FindName(string.Format("rctItem{0}", i));
                var itemSlotTxt = (TextBlock)FindName(string.Format("txbItem{0}", i));
                if (_engine.Player.Inventory.Items.Count < (i + 1))
                {
                    itemSlotRct.Fill = Brushes.AliceBlue;
                    itemSlotTxt.Text = "000";
                }
                else
                {
                    var item = _engine.Player.Inventory.Items.ElementAt(i);
                    itemSlotTxt.Text = item.Quantity.ToString().PadLeft(3, '0');
                    switch (item.ItemId)
                    {
                        case ItemIdEnum.Bomb:
                            itemSlotRct.Fill = Brushes.LightBlue;
                            break;
                        case ItemIdEnum.SmallLifePotion:
                            itemSlotRct.Fill = Brushes.LightGoldenrodYellow;
                            break;
                    }
                }
            }
        }
    }
}
