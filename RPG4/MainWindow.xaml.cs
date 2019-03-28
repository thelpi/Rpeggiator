using RPG4.Abstraction;
using RPG4.Abstraction.Sprites;
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
    /// Logic interaction for the main window.
    /// </summary>
    /// <seealso cref="Window"/>
    public partial class MainWindow : Window
    {
        private const string _playerUid = "PlayerUid";
        private const string _shadowUid = "ShadowUid";
        private const string UNIQUE_TIMESTAMP_PATTERN = "fffffff";

        private Timer _timer;
        private volatile bool _timerIsIn;
        private AbstractEngine _engine;
        private int _currentScreenIndex;
        private ulong _missedFrames;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _engine = new AbstractEngine(Constants.FIRST_SCREEN_INDEX);

            // Size of the player never changes.
            rctPlayer.Height = _engine.Player.Height;
            rctPlayer.Width = _engine.Player.Width;
            rctPlayer.Uid = _playerUid;

            rctShadow.Uid = _shadowUid;

            _missedFrames = 0;
            _timer = new Timer(1000 / Constants.FPS);
            _timer.Elapsed += NewFrame;
            _timer.Start();
        }

        private void NewFrame(object sender, ElapsedEventArgs e)
        {
            if (_timerIsIn)
            {
                _missedFrames++;
                return;
            }

            _timerIsIn = true;

            // check pressed keys
            var pressedKeys = (KeyPress)Dispatcher.Invoke(new KeyPressHandler(delegate()
            {
                int? inventorySlotId = null;
                for (int i = 0; i < Constants.INVENTORY_SIZE; i++)
                {
                    if (Keyboard.IsKeyDown((Key)Enum.Parse(typeof(Key), string.Format("D{0}", i))))
                    {
                        inventorySlotId = i == 0 ? Constants.INVENTORY_SIZE : (i - 1);
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
                if (_engine.CurrentScreenId != _currentScreenIndex)
                {
                    _currentScreenIndex = _engine.CurrentScreenId;

                    cvsMain.Background = (Brush)_engine.ScreenGraphic.GetRendering();
                    cvsMain.Height = _engine.AreaHeight;
                    cvsMain.Width = _engine.AreaWidth;

                    rctShadow.Height = _engine.AreaHeight;
                    rctShadow.Width = _engine.AreaWidth;
                    rctShadow.Opacity = _engine.AreaShadowOpacity;

                    ClearUnfixedChildren(true);
                    DrawWalls();
                }

                RefreshAnimatedSprites();
                RefreshMenu();

                if (_engine.Player.CheckDeath(_engine))
                {
                    MessageBox.Show("You die !");
                    _timer.Stop();
                }
            }));

            _timerIsIn = false;
        }

        // Draws each calls inside the main canvas
        private void DrawWalls()
        {
            foreach (var s in _engine.Walls)
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
                rct.Uid = DateTime.Now.ToString(UNIQUE_TIMESTAMP_PATTERN);
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

            foreach (var sprite in _engine.AnimatedSprites)
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
                var pt = new Point(_engine.Player.CenterPoint.X / _engine.AreaWidth,
                    _engine.Player.CenterPoint.Y / _engine.AreaHeight);

                var lampBrush = new RadialGradientBrush(Colors.Transparent, Colors.Black);
                lampBrush.Center = pt;
                lampBrush.GradientOrigin = pt;
                lampBrush.RadiusX = 0.2 * (_engine.AreaHeight / _engine.AreaWidth);
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
            for (int i = 0; i < Constants.INVENTORY_SIZE; i++)
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
    }
}
