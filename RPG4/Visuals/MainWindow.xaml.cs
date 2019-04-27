using RpeggiatorLib;
using RpeggiatorLib.Sprites;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RPG4.Visuals
{
    /// <summary>
    /// Delegate to pass the pressed keys of the keayboard to the engine.
    /// </summary>
    /// <returns>A method which returns pressed keyboard's keys.</returns>
    public delegate KeyPress KeyPressHandler();

    /// <summary>
    /// Logic interaction for the main window.
    /// </summary>
    /// <seealso cref="Window"/>
    public partial class MainWindow : Window
    {
        private const string _playerUid = "PlayerUid";
        private const string _darknessUid = "DarknessUid";
        private const string UNIQUE_TIMESTAMP_PATTERN = "fffffff";
        private const double MIN_DELAY_BETWEEN_FRAMES = 10;

        private bool _hitKeyPressed;
        private bool _actionKeyPressed;
        private bool _pauseKeyPressed;
        private int? _inventoryKeyPressed;
        
        private int _currentScreenId;
        private Engine _engine;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            rctPlayer.Uid = _playerUid;
            rctDarkness.Uid = _darknessUid;

            BackgroundWorker worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            worker.DoWork += delegate (object sender, DoWorkEventArgs e)
            {
                _engine = Engine.InitializeEngine(Properties.Settings.Default.ResourcesPath);

                Stopwatch stopWatch = new Stopwatch();
                while (true)
                {
                    stopWatch.Restart();
                    try
                    {
                        if (_pauseKeyPressed)
                        {
                            _engine.Pause();
                            while (_pauseKeyPressed)
                            {
                                Thread.Sleep(500);
                            }
                        }

                        // check pressed keys
                        KeyPress pressedKeys = (KeyPress)Dispatcher.Invoke(new KeyPressHandler(delegate ()
                        {
                            KeyPress kp = new KeyPress(
                                Keyboard.IsKeyDown(Key.Z),
                                Keyboard.IsKeyDown(Key.S),
                                Keyboard.IsKeyDown(Key.D),
                                Keyboard.IsKeyDown(Key.Q),
                                _hitKeyPressed,
                                _actionKeyPressed,
                                Keyboard.IsKeyDown(Key.NumPad6),
                                _inventoryKeyPressed
                            );
                            _inventoryKeyPressed = null;
                            _hitKeyPressed = false;
                            _actionKeyPressed = false;
                            _pauseKeyPressed = false;
                            return kp;
                        }));

                        // recompute everything
                        _engine.CheckEngineAtNewFrame(pressedKeys);

                        (sender as BackgroundWorker).ReportProgress(0);

                        if (_engine.PlayerIsDead())
                        {
                            e.Result = null;
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        e.Result = string.Concat(ex.Message, "\r\n", ex.StackTrace);
                        return;
                    }
                    stopWatch.Stop();
                    double stayToElapse = MIN_DELAY_BETWEEN_FRAMES - stopWatch.Elapsed.TotalMilliseconds;
                    if (stayToElapse > 0)
                    {
                        Thread.Sleep((int)stayToElapse);
                    }
                }
            };
            worker.ProgressChanged += delegate (object sender, ProgressChangedEventArgs e)
            {
                if (_engine.CurrentScreenId != _currentScreenId)
                {
                    _currentScreenId = _engine.CurrentScreenId;

                    rctCoin.Fill = _engine.CoinMenuRender.GetRenderBrush();
                    rctKeyring.Fill = _engine.KeyringMenuRender.GetRenderBrush();

                    cvsMain.Background = _engine.CurrentScreen.Render.GetRenderBrush();
                    cvsMain.Height = _engine.CurrentScreen.Height;
                    cvsMain.Width = _engine.CurrentScreen.Width;

                    rctDarkness.Height = _engine.CurrentScreen.Height;
                    rctDarkness.Width = _engine.CurrentScreen.Width;

                    ClearUnfixedChildren(true);
                    DrawPermanentStructures();
                }
                
                RefreshAnimatedSprites();
                RefreshMenu();
            };
            worker.RunWorkerCompleted += delegate (object sender, RunWorkerCompletedEventArgs e)
            {
                MessageBox.Show(e.Result == null ? "You die !" : e.Result.ToString());
                Close();
            };
            worker.RunWorkerAsync();
        }

        // Draws each permanent structures inside the main canvas
        private void DrawPermanentStructures()
        {
            foreach (PermanentStructure s in _engine.CurrentScreen.PermanentStructures)
            {
                DrawSizedPoint(s, fixedId: true);
            }
            foreach (Floor f in _engine.CurrentScreen.Floors)
            {
                DrawSizedPoint(f, fixedId: true);
            }
        }

        // Draws a sprite inside the main canvas.
        private void DrawSizedPoint(Sprite sp, bool fixedId = false)
        {
            Rectangle rct = new Rectangle
            {
                Fill = sp.Render.GetRenderBrush(),
                Width = sp.Width,
                Height = sp.Height
            };
            if (fixedId)
            {
                rct.Uid = DateTime.Now.ToString(UNIQUE_TIMESTAMP_PATTERN);
            }
            rct.SetValue(Canvas.TopProperty, sp.Y);
            rct.SetValue(Canvas.LeftProperty, sp.X);

            Panel.SetZIndex(rct, sp.Z);

            cvsMain.Children.Add(rct);
        }

        // Remove every children of the canvas without fixed uid.
        private void ClearUnfixedChildren(bool everythingExceptPlayerAndDarkness = false)
        {
            for (int i = cvsMain.Children.Count - 1; i >= 0; i--)
            {
                string uid = cvsMain.Children[i].Uid;
                if (string.IsNullOrWhiteSpace(uid) || (everythingExceptPlayerAndDarkness && uid != _playerUid && uid != _darknessUid))
                {
                    cvsMain.Children.RemoveAt(i);
                }
            }
        }

        // Refresh every animated sprites.
        private void RefreshAnimatedSprites()
        {
            ClearUnfixedChildren();

            #region Player

            rctPlayer.Height = _engine.Player.Height;
            rctPlayer.Width = _engine.Player.Width;

            rctPlayer.SetValue(Canvas.TopProperty, _engine.Player.Y);
            rctPlayer.SetValue(Canvas.LeftProperty, _engine.Player.X);

            rctPlayer.Fill = _engine.Player.Render.GetRenderBrush();

            if (_engine.Player.IsHitting)
            {
                DrawSizedPoint(_engine.Player.SwordHitSprite);
            }

            Panel.SetZIndex(rctPlayer, _engine.Player.Z);

            #endregion Player

            foreach (Sprite sprite in _engine.CurrentScreen.AnimatedSprites)
            {
                if (sprite.GetType() == typeof(ActionnedBomb))
                {
                    BombExplosion halo = (sprite as ActionnedBomb).ExplosionSprite;
                    if (halo != null)
                    {
                        DrawSizedPoint(halo);
                    }
                }

                DrawSizedPoint(sprite);
            }

            SetLightAndDarkness();
        }

        // Refresh the menu status.
        private void RefreshMenu()
        {
            pgbPlayerLifePoints.Maximum = _engine.Player.MaximalLifePoints;
            pgbPlayerLifePoints.Value = _engine.Player.CurrentLifePoints;
            for (int i = 0; i < _engine.Player.Inventory.ActiveSlotCount; i++)
            {
                Rectangle itemSlotRct = (Rectangle)FindName(string.Format("rctItem{0}", i));
                TextBlock itemSlotTxt = (TextBlock)FindName(string.Format("txbItem{0}", i));
                if (_engine.Player.Inventory.DisplayableItems.Count < (i + 1))
                {
                    itemSlotRct.Fill = Brushes.AliceBlue;
                    itemSlotTxt.Text = "000";
                }
                else
                {
                    InventoryItem item = _engine.Player.Inventory.DisplayableItems.ElementAt(i);
                    itemSlotTxt.Text = item.DisplayableQuantity.ToString().PadLeft(3, '0');
                    itemSlotRct.Fill = item.Render.GetRenderBrush();
                }
            }
            txtCoins.Text = _engine.Player.Inventory.Coins.ToString().PadLeft(3, '0');
            txtKeyring.Text = _engine.Player.Inventory.Keyring.Count.ToString().PadLeft(3, '0');
        }

        // Happens on key press.
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    _pauseKeyPressed = true;
                    Hide();
                    new InventoryWindow().ShowDialog();
                    _pauseKeyPressed = false;
                    ShowDialog();
                    break;
                case Key.NumPad4:
                    _hitKeyPressed = true;
                    break;
                case Key.NumPad5:
                    _actionKeyPressed = true;
                    break;
                // TODO : link this to _engine.Player.Inventory.ActiveSlotCount
                case Key.NumPad7:
                    _inventoryKeyPressed = 1;
                    break;
                case Key.NumPad8:
                    _inventoryKeyPressed = 2;
                    break;
                case Key.NumPad9:
                    _inventoryKeyPressed = 3;
                    break;
            }
        }

        private void SetLightAndDarkness()
        {
            rctDarkness.Opacity = _engine.GetCurrentScreenOpacity();

            if (_engine.Player.Inventory.LampIsOn)
            {
                Point pt = new Point(
                    _engine.Player.CenterPointX / _engine.CurrentScreen.Width,
                    _engine.Player.CenterPointY / _engine.CurrentScreen.Height);

                RadialGradientBrush lampBrush = new RadialGradientBrush(Colors.Transparent, Colors.Black)
                {
                    Center = pt,
                    GradientOrigin = pt,
                    RadiusX = 0.2 * (_engine.CurrentScreen.Height / _engine.CurrentScreen.Width),
                    RadiusY = 0.2
                };
                rctDarkness.Fill = lampBrush;
            }
            else
            {
                rctDarkness.Fill = Brushes.Black;
            }
        }
    }
}
