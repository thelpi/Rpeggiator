using RPG4.Models;
using RPG4.Models.Sprites;
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

        private bool _hitKeyPressed;
        private bool _actionKeyPressed;
        private int? _inventoryKeyPressed;
        
        private int _currentScreenIndex;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Size of the player never changes.
            rctPlayer.Height = Engine.Default.Player.Height;
            rctPlayer.Width = Engine.Default.Player.Width;
            rctPlayer.Uid = _playerUid;

            rctDarkness.Uid = _darknessUid;

            BackgroundWorker worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            worker.DoWork += delegate (object sender, DoWorkEventArgs e)
            {
                Stopwatch stopWatch = new Stopwatch();
                while (true)
                {
                    stopWatch.Restart();
                    try
                    {
                        // check pressed keys
                        KeyPress pressedKeys = (KeyPress)Dispatcher.Invoke(new KeyPressHandler(delegate ()
                        {
                            KeyPress kp = new KeyPress(
                                Keyboard.IsKeyDown(Key.Up),
                                Keyboard.IsKeyDown(Key.Down),
                                Keyboard.IsKeyDown(Key.Right),
                                Keyboard.IsKeyDown(Key.Left),
                                _hitKeyPressed,
                                _actionKeyPressed,
                                _inventoryKeyPressed
                            );
                            _inventoryKeyPressed = null;
                            _hitKeyPressed = false;
                            _actionKeyPressed = false;
                            return kp;
                        }));

                        // recompute everything
                        Engine.Default.CheckEngineAtNewFrame(pressedKeys);

                        (sender as BackgroundWorker).ReportProgress(0);

                        if (Engine.Default.Player.CheckDeath(Engine.Default.CurrentScreen))
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
                    double stayToElapse = Constants.MIN_DELAY_BETWEEN_FRAMES - stopWatch.Elapsed.TotalMilliseconds;
                    if (stayToElapse.Greater(0))
                    {
                        Thread.Sleep((int)stayToElapse);
                    }
                }
            };
            worker.ProgressChanged += delegate (object sender, ProgressChangedEventArgs e)
            {
                if (Engine.Default.CurrentScreenId != _currentScreenIndex)
                {
                    _currentScreenIndex = Engine.Default.CurrentScreenId;

                    cvsMain.Background = Engine.Default.CurrentScreen.Graphic.GetRenderingBrush();
                    cvsMain.Height = Engine.Default.CurrentScreen.Height;
                    cvsMain.Width = Engine.Default.CurrentScreen.Width;

                    rctDarkness.Height = Engine.Default.CurrentScreen.Height;
                    rctDarkness.Width = Engine.Default.CurrentScreen.Width;

                    ClearUnfixedChildren(true);
                    DrawPermanentStructures();
                }
                
                RefreshAnimatedSprites();
                RefreshMenu();
            };
            worker.RunWorkerCompleted += delegate (object sender, RunWorkerCompletedEventArgs e)
            {
                MessageBox.Show(e.Result == null ? "You die !" : e.Result.ToString());
            };
            worker.RunWorkerAsync();
        }

        // Draws each permanent structures inside the main canvas
        private void DrawPermanentStructures()
        {
            foreach (PermanentStructure s in Engine.Default.CurrentScreen.PermanentStructures)
            {
                DrawSizedPoint(s, fixedId: true);
            }
            foreach (Floor f in Engine.Default.CurrentScreen.Floors)
            {
                DrawSizedPoint(f, fixedId: true);
            }
        }

        // Draws a sprite inside the main canvas.
        private void DrawSizedPoint(Sprite sp, bool fixedId = false)
        {
            Rectangle rct = new Rectangle
            {
                Fill = sp.Graphic.GetRenderingBrush(),
                Width = sp.Width,
                Height = sp.Height
            };
            if (fixedId)
            {
                rct.Uid = DateTime.Now.ToString(Constants.UNIQUE_TIMESTAMP_PATTERN);
            }
            rct.SetValue(Canvas.TopProperty, sp.Y);
            rct.SetValue(Canvas.LeftProperty, sp.X);

            Panel.SetZIndex(rct, sp.ZIndex);

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

            rctPlayer.SetValue(Canvas.TopProperty, Engine.Default.Player.Y);
            rctPlayer.SetValue(Canvas.LeftProperty, Engine.Default.Player.X);

            rctPlayer.Fill = Engine.Default.Player.Graphic.GetRenderingBrush();

            if (Engine.Default.Player.IsHitting)
            {
                DrawSizedPoint(Engine.Default.Player.HitSprite);
            }

            Panel.SetZIndex(rctPlayer, Engine.Default.Player.ZIndex);

            #endregion Player

            foreach (Sprite sprite in Engine.Default.CurrentScreen.AnimatedSprites)
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
            pgbPlayerLifePoints.Maximum = Engine.Default.Player.MaximalLifePoints;
            pgbPlayerLifePoints.Value = Engine.Default.Player.CurrentLifePoints;
            for (int i = 0; i < Constants.Inventory.SIZE; i++)
            {
                Rectangle itemSlotRct = (Rectangle)FindName(string.Format("rctItem{0}", i));
                TextBlock itemSlotTxt = (TextBlock)FindName(string.Format("txbItem{0}", i));
                if (Engine.Default.Player.Inventory.DisplayableItems.Count < (i + 1))
                {
                    itemSlotRct.Fill = Brushes.AliceBlue;
                    itemSlotTxt.Text = "000";
                }
                else
                {
                    InventoryItem item = Engine.Default.Player.Inventory.DisplayableItems.ElementAt(i);
                    itemSlotTxt.Text = item.DisplayableQuantity.ToString().PadLeft(3, '0');
                    itemSlotRct.Fill = item.BaseItem.LootGraphic.GetRenderingBrush();
                }
            }
            txtCoins.Text = Engine.Default.Player.Inventory.Coins.ToString().PadLeft(3, '0');
            txtKeyring.Text = Engine.Default.Player.Inventory.Keyring.Count.ToString().PadLeft(3, '0');
        }

        // Happens on key press.
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    _actionKeyPressed = true;
                    break;
                case Key.Space:
                    _hitKeyPressed = true;
                    break;
                case Key.D0:
                    _inventoryKeyPressed = Constants.Inventory.SIZE - 1;
                    break;
                case Key.D1:
                    _inventoryKeyPressed = 0;
                    break;
                case Key.D2:
                    _inventoryKeyPressed = 1;
                    break;
                case Key.D3:
                    _inventoryKeyPressed = 2;
                    break;
                case Key.D4:
                    _inventoryKeyPressed = 3;
                    break;
                case Key.D5:
                    _inventoryKeyPressed = 4;
                    break;
                case Key.D6:
                    _inventoryKeyPressed = 5;
                    break;
                case Key.D7:
                    _inventoryKeyPressed = 6;
                    break;
                case Key.D8:
                    _inventoryKeyPressed = 7;
                    break;
                case Key.D9:
                    _inventoryKeyPressed = 8;
                    break;
            }
        }

        private void SetLightAndDarkness()
        {
            double currentHour = Engine.Default.Hour;

            double darknessOpacity = Engine.Default.CurrentScreen.DarknessOpacity;

            // after dawn, before dusk
            double dayTimeDarknessOpacity;
            if (currentHour.GreaterEqual(Constants.NIGHT_DAWN_HOUR) && currentHour.Lower(Constants.NIGHT_DUSK_HOUR))
            {
                dayTimeDarknessOpacity = 0;
            }
            // in the darkness peak
            else if (currentHour.GreaterEqual(Constants.NIGHT_PEAK_HOUR_BEGIN) || currentHour.Lower(Constants.NIGHT_PEAK_HOUR_END))
            {
                dayTimeDarknessOpacity = Constants.NIGHT_DARKNESS_OPACITY;
            }
            // between dusk and darkness peak
            else if (currentHour.GreaterEqual(Constants.NIGHT_DUSK_HOUR) && currentHour.Lower(Constants.NIGHT_PEAK_HOUR_BEGIN))
            {
                double nightProgressionRatio = (currentHour - Constants.NIGHT_DUSK_HOUR) / (Constants.NIGHT_PEAK_HOUR_BEGIN - Constants.NIGHT_DUSK_HOUR);
                dayTimeDarknessOpacity = Constants.NIGHT_DARKNESS_OPACITY * nightProgressionRatio;
            }
            // between darkness peak and dawn
            else
            {
                double nightProgressionRatio = 1 - ((currentHour - Constants.NIGHT_PEAK_HOUR_END) / (Constants.NIGHT_DAWN_HOUR - Constants.NIGHT_PEAK_HOUR_END));
                dayTimeDarknessOpacity = Constants.NIGHT_DARKNESS_OPACITY * nightProgressionRatio;
            }

            rctDarkness.Opacity = darknessOpacity.Greater(dayTimeDarknessOpacity) ? darknessOpacity : dayTimeDarknessOpacity;

            if (Engine.Default.Player.Inventory.LampIsOn)
            {
                Point pt = new Point(Engine.Default.Player.CenterPoint.X / Engine.Default.CurrentScreen.Width,
                    Engine.Default.Player.CenterPoint.Y / Engine.Default.CurrentScreen.Height);

                RadialGradientBrush lampBrush = new RadialGradientBrush(Colors.Transparent, Colors.Black);
                lampBrush.Center = pt;
                lampBrush.GradientOrigin = pt;
                lampBrush.RadiusX = 0.2 * (Engine.Default.CurrentScreen.Height / Engine.Default.CurrentScreen.Width);
                lampBrush.RadiusY = 0.2;
                rctDarkness.Fill = lampBrush;
            }
            else
            {
                rctDarkness.Fill = Brushes.Black;
            }
        }
    }
}
