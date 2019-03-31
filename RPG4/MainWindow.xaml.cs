using RPG4.Abstraction;
using RPG4.Abstraction.Sprites;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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
    /// Logic interaction for the main window.
    /// </summary>
    /// <seealso cref="Window"/>
    public partial class MainWindow : Window
    {
        private const string _playerUid = "PlayerUid";
        private const string _shadowUid = "ShadowUid";

        private bool _hitKeyPressed;
        private int? _inventoryKeyPressed;

        private Engine _engine;
        private int _currentScreenIndex;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _engine = new Engine(Constants.FIRST_SCREEN_INDEX);

            // Size of the player never changes.
            rctPlayer.Height = _engine.Player.Height;
            rctPlayer.Width = _engine.Player.Width;
            rctPlayer.Uid = _playerUid;

            rctShadow.Uid = _shadowUid;

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
                        var pressedKeys = (KeyPress)Dispatcher.Invoke(new KeyPressHandler(delegate ()
                        {
                            var kp = new KeyPress(
                                Keyboard.IsKeyDown(Key.Up),
                                Keyboard.IsKeyDown(Key.Down),
                                Keyboard.IsKeyDown(Key.Right),
                                Keyboard.IsKeyDown(Key.Left),
                                _hitKeyPressed,
                                _inventoryKeyPressed
                            );
                            _inventoryKeyPressed = null;
                            _hitKeyPressed = false;
                            return kp;
                        }));

                        // recompute everything
                        _engine.CheckEngineAtNewFrame(pressedKeys);

                        (sender as BackgroundWorker).ReportProgress(0);

                        if (_engine.Player.CheckDeath(_engine.CurrentScreen))
                        {
                            e.Result = null;
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        e.Result = ex.Message;
                        return;
                    }
                    stopWatch.Stop();
                    double stayToElapse = Constants.MIN_DELAY_BETWEEN_FRAMES - stopWatch.Elapsed.TotalMilliseconds;
                    if (stayToElapse > 0)
                    {
                        Thread.Sleep((int)stayToElapse);
                    }
                }
            };
            worker.ProgressChanged += delegate (object sender, ProgressChangedEventArgs e)
            {
                if (_engine.CurrentScreenId != _currentScreenIndex)
                {
                    _currentScreenIndex = _engine.CurrentScreenId;

                    cvsMain.Background = (Brush)_engine.CurrentScreen.Graphic.GetRendering();
                    cvsMain.Height = _engine.CurrentScreen.Height;
                    cvsMain.Width = _engine.CurrentScreen.Width;

                    rctShadow.Height = _engine.CurrentScreen.Height;
                    rctShadow.Width = _engine.CurrentScreen.Width;
                    rctShadow.Opacity = _engine.CurrentScreen.ShadowOpacity;

                    ClearUnfixedChildren(true);
                    DrawWalls();
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

        // Draws each calls inside the main canvas
        private void DrawWalls()
        {
            foreach (var s in _engine.CurrentScreen.Walls)
            {
                DrawSizedPoint(s, fixedId: true);
            }
        }

        // Draws a sprite inside the main canvas.
        private void DrawSizedPoint(Sprite sp, int zIndex = 0, bool fixedId = false)
        {
            Brush render = (Brush)sp.Graphic.GetRendering();
            if (sp.GetType().IsSubclassOf(typeof(FloorTrigger)) && ((FloorTrigger)sp).IsActivated)
            {
                render = (Brush)((FloorTrigger)sp).ActivatedGraphic.GetRendering();
            }

            Rectangle rct = new Rectangle
            {
                Fill = render,
                Width = sp.Width,
                Height = sp.Height
            };
            if (fixedId)
            {
                rct.Uid = DateTime.Now.ToString(Constants.UNIQUE_TIMESTAMP_PATTERN);
            }
            rct.SetValue(Canvas.TopProperty, sp.Y);
            rct.SetValue(Canvas.LeftProperty, sp.X);

            Panel.SetZIndex(rct, zIndex);

            cvsMain.Children.Add(rct);
        }

        // Remove every children of the canvas without fixed uid.
        private void ClearUnfixedChildren(bool everythingExceptPlayerAndShadow = false)
        {
            for (int i = cvsMain.Children.Count - 1; i >= 0; i--)
            {
                string uid = cvsMain.Children[i].Uid;
                if (string.IsNullOrWhiteSpace(uid) || (everythingExceptPlayerAndShadow && uid != _playerUid && uid != _shadowUid))
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

            rctPlayer.SetValue(Canvas.TopProperty, _engine.Player.Y);
            rctPlayer.SetValue(Canvas.LeftProperty, _engine.Player.X);

            if (_engine.Player.IsRecovering)
            {
                rctPlayer.Fill = (Brush)_engine.Player.RecoveryGraphic.GetRendering();
            }
            else
            {
                rctPlayer.Fill = (Brush)_engine.Player.Graphic.GetRendering();
            }

            int zIndex = 0;
            if (_engine.Player.IsHitting)
            {
                zIndex = 1;
                DrawSizedPoint(_engine.Player.HitSprite);
            }

            Panel.SetZIndex(rctPlayer, zIndex);

            #endregion Player

            foreach (var sprite in _engine.CurrentScreen.AnimatedSprites)
            {
                zIndex = 0;
                if (sprite.GetType() == typeof(ActionnedBomb))
                {
                    var halo = (sprite as ActionnedBomb).ExplosionSprite;
                    if (halo != null)
                    {
                        DrawSizedPoint(halo);
                        zIndex = 1;
                    }
                }

                DrawSizedPoint(sprite, zIndex: zIndex);
            }

            #region Shadow

            if (_engine.Player.Inventory.LampIsOn)
            {
                var pt = new Point(_engine.Player.CenterPoint.X / _engine.CurrentScreen.Width,
                    _engine.Player.CenterPoint.Y / _engine.CurrentScreen.Height);

                var lampBrush = new RadialGradientBrush(Colors.Transparent, Colors.Black);
                lampBrush.Center = pt;
                lampBrush.GradientOrigin = pt;
                lampBrush.RadiusX = 0.2 * (_engine.CurrentScreen.Height / _engine.CurrentScreen.Width);
                lampBrush.RadiusY = 0.2;
                rctShadow.Fill = lampBrush;
            }
            else
            {
                rctShadow.Fill = Brushes.Black;
            }

            #endregion
        }

        // Refresh the menu status.
        private void RefreshMenu()
        {
            pgbPlayerLifePoints.Maximum = _engine.Player.MaximalLifePoints;
            pgbPlayerLifePoints.Value = _engine.Player.CurrentLifePoints;
            for (int i = 0; i < Inventory.SIZE; i++)
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
                    switch (item.BaseItem.Id)
                    {
                        case ItemIdEnum.Bomb:
                            itemSlotRct.Fill = Brushes.LightBlue;
                            break;
                        case ItemIdEnum.SmallLifePotion:
                            itemSlotRct.Fill = Brushes.LightGoldenrodYellow;
                            break;
                        case ItemIdEnum.Lamp:
                            itemSlotRct.Fill = Brushes.Yellow;
                            break;
                    }
                }
            }
        }

        // Happens on key press.
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    _hitKeyPressed = true;
                    break;
                case Key.D0:
                    _inventoryKeyPressed = Inventory.SIZE - 1;
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
    }
}
