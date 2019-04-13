using System;
using System.Collections.Generic;
using System.Linq;
using RpeggiatorLib.Sprites;

namespace RpeggiatorLib
{
    /// <summary>
    /// Game engine.
    /// </summary>
    public class Engine
    {
        /// <summary>
        /// Path to resources directory.
        /// </summary>
        internal static string ResourcesPath { get; private set; }

        /// <summary>
        /// Singleton access.
        /// </summary>
        internal static Engine Default { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="Engine"/>.
        /// </summary>
        /// <param name="resourcesPath"><see cref="ResourcesPath"/></param>
        /// <returns><see cref="Default"/></returns>
        public static Engine InitializeEngine(string resourcesPath)
        {
            if (!System.IO.Directory.Exists(resourcesPath))
            {
                throw new ArgumentException(Messages.InvalidResourcesPathExceptionMessage, nameof(resourcesPath));
            }

            ResourcesPath = string.Concat(resourcesPath, resourcesPath.Last() == '\\' ? string.Empty : "\\");
            Default = new Engine(Constants.FIRST_SCREEN_INDEX);
            return Default;
        }

        private Screen _currentScreen;
        private DateTime _beginTimestamp;
        private List<Screen> _screens = new List<Screen>();

        /// <summary>
        /// <see cref="KeyPress"/>
        /// </summary>
        public KeyPress KeyPress{ get; private set; }
        /// <summary>
        /// <see cref="Sprites.Player"/>
        /// </summary>
        public Player Player { get; private set; }
        /// <summary>
        /// Inferred; current screen identifier.
        /// </summary>
        public int CurrentScreenId { get { return CurrentScreen.Id; } }
        /// <summary>
        /// Current <see cref="Screen"/> (where's the player).
        /// </summary>
        public Screen CurrentScreen
        {
            get
            {
                return _currentScreen;
            }
            set
            {
                _currentScreen?.FreezeEnemies();
                _currentScreen = value;
            }
        }
        /// <summary>
        /// Inferred; current in-game day number.
        /// </summary>
        public int Day { get { return (int)Math.Floor((DateTime.Now - _beginTimestamp).TotalDays * Constants.TIME_RATIO); } }
        /// <summary>
        /// Inferred; current in-game hour.
        /// </summary>
        public double Hour
        {
            get
            {
                double totalHours = (DateTime.Now - _beginTimestamp).TotalHours * Constants.TIME_RATIO;
                int totalHoursFloored = (int)Math.Floor(totalHours);
                return (totalHoursFloored % 24) + (totalHours - totalHoursFloored) + Constants.FIRST_DAY_HOUR_START;
            }
        }
        /// <summary>
        /// Gets the <see cref="Render.ISpriteRender"/> for inventory coins.
        /// </summary>
        public Render.ISpriteRender CoinMenuRender { get { return Render.ImageRender.CoinMenuRender(); } }
        /// <summary>
        /// Gets the <see cref="Render.ISpriteRender"/> for inventory keyring.
        /// </summary>
        public Render.ISpriteRender KeyringMenuRender { get { return Render.ImageRender.KeyringMenuRender(); } }

        // Private constructor.
        private Engine(int screenId)
        {
            _beginTimestamp = DateTime.Now;
            Player = new Player();
            CurrentScreen = GetOrCreateScreen(screenId);
        }

        // Gets or creates a screen by its identifier.
        private Screen GetOrCreateScreen(int id)
        {
            if (_screens.Any(s => s.Id == id))
            {
                return _screens.First(s => s.Id == id);
            }

            dynamic screenJsonDatas = Tools.GetScreenDatasFromIndex(id);

            Screen screen = new Screen(id, screenJsonDatas);

            _screens.Add(screen);

            return screen;
        }

        /// <summary>
        /// Refresh the status of every components at new frame.
        /// </summary>
        /// <param name="keys"><see cref="KeyPress"/></param>
        /// <exception cref="ArgumentNullException">The argument <paramref name="keys"/> is <c>Null</c>.</exception>
        public void CheckEngineAtNewFrame(KeyPress keys)
        {
            KeyPress = keys ?? throw new ArgumentNullException(nameof(keys));

            Player.BehaviorAtNewFrame();
            Player.CheckIfHasBeenHit();
            CollectPickableItems();
            CheckInventoryUse();

            CurrentScreen.BehaviorAtNewFrame();

            // At last, every possiblities to change screen.
            if (Player.NewScreenEntrance.HasValue)
            {
                CurrentScreen = GetOrCreateScreen(CurrentScreen.GetNextScreenIdFromDirection(Player.NewScreenEntrance.Value));
            }
            else
            {
                int? doorId;
                int? newScreenByDoor = CheckForDoorOpened(out doorId);
                if (newScreenByDoor.HasValue)
                {
                    // The position must be set before to get to the screen.
                    // Otherwise, the door got in the first method will be from the new screen.
                    Player.SetPositionRelativeToDoorGoThrough(doorId.Value);
                    CurrentScreen = GetOrCreateScreen(newScreenByDoor.Value);
                }
                else
                {
                    Pit pit = CurrentScreen.Pits.FirstOrDefault(p => p.ScreenIndexEntrance.HasValue && p.CanFallIn(Player));
                    if (pit != null)
                    {
                        CurrentScreen = GetOrCreateScreen(pit.ScreenIndexEntrance.Value);
                    }
                }
            }
        }

        // Checks inventory use.
        private void CheckInventoryUse()
        {
            if (KeyPress.InventorySlotId.HasValue)
            {
                ActionnedItem itemDropped = Player.Inventory.UseItem();
                if (itemDropped != null)
                {
                    CurrentScreen.AddDroppedItem(itemDropped);
                }
            }
        }

        // Checks for items to pick on the current position.
        private void CollectPickableItems()
        {
            List<PickableItem> items = CurrentScreen.PickableItems.Where(it => it.Overlap(Player)).ToList();
            items.ForEach(i => i.Pick());
            CurrentScreen.CheckPickableItemsQuantities();
        }

        /// <summary>
        /// Checks if the specified <see cref="FloorTrigger"/> has been triggered by the environnement.
        /// </summary>
        /// <param name="trigger"><see cref="FloorTrigger"/></param>
        /// <returns><c>True</c> if triggered; <c>False</c> otherwise.</returns>
        internal bool IsTriggered(FloorTrigger trigger)
        {
            return trigger.Overlap(Player) || CurrentScreen.Enemies.Any(e => trigger.Overlap(e));
        }

        /// <summary>
        /// Checks hit(s) made by enemies on player.
        /// </summary>
        /// <returns>Potential cost in life points.</returns>
        internal double CheckHitByEnemiesOnPlayer()
        {
            // checks hit (for each enemy, life points lost is cumulable)
            double cumuledLifePoints = 0;
            foreach (Enemy enemy in CurrentScreen.Enemies)
            {
                if (Player.Overlap(enemy))
                {
                    cumuledLifePoints += enemy.HitLifePointCost;
                }
            }
            return cumuledLifePoints;
        }

        // Checks if a door to a new screen has been opened.
        private int? CheckForDoorOpened(out int? doorId)
        {
            doorId = null;

            if (!KeyPress.PressAction)
            {
                return null;
            }

            foreach (Door door in CurrentScreen.Doors)
            {
                if (door.Overlap(Player.ResizeToRatio(Constants.Player.ACTION_RANGE)) && door.PlayerIsLookingTo())
                {
                    int? screenId = door.TryOpen();
                    if (screenId.HasValue)
                    {
                        doorId = door.Id;
                        return screenId;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the <see cref="CurrentScreen"/> opacity, depending on the day / hour and the <see cref="Screen"/> proeprties.
        /// </summary>
        /// <returns>The opacity value.</returns>
        public double GetCurrentScreenOpacity()
        {
            double currentHour = Hour;

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

            // Shortcut.
            double darknessOpacity = CurrentScreen.DarknessOpacity;

            return darknessOpacity.Greater(dayTimeDarknessOpacity) ? darknessOpacity : dayTimeDarknessOpacity;
        }

        /// <summary>
        /// CHecks if the <see cref="Player"/> is dead.
        /// </summary>
        /// <returns><c>True</c> if dead; <c>False</c> otherwise.</returns>
        public bool PlayerIsDead()
        {
            return Player.CheckDeath(CurrentScreen);
        }
    }
}
