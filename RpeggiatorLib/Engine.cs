using RpeggiatorLib.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RpeggiatorLib
{
    /// <summary>
    /// Game engine.
    /// </summary>
    public class Engine
    {
        private static Engine _engine;

        /// <summary>
        /// Singleton access.
        /// </summary>
        public static Engine Default
        {
            get
            {
                if (_engine == null)
                {
                    _engine = new Engine(Constants.FIRST_SCREEN_INDEX);
                }
                return _engine;
            }
        }

        /// <summary>
        /// Ensures the creation of a new engine.
        /// </summary>
        public static void ResetEngine()
        {
            _engine = null;
        }

        private Screen _currentScreen;
        private DateTime _beginTimestamp;
        // List of each instancied screen.
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
                return (totalHoursFloored % 24) + (totalHours - totalHoursFloored);
            }
        }

        // Private constructor.
        private Engine(int screenId)
        {
            _beginTimestamp = DateTime.Now;
            Player = new Player();
            CurrentScreen = GetOrCreatetScreen(screenId);
        }

        /// <summary>
        /// Gets or creates a screen by its identifier.
        /// </summary>
        /// <param name="id"><see cref="Screen.Id"/></param>
        /// <returns><see cref="Screen"/></returns>
        public Screen GetOrCreatetScreen(int id)
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
        public void CheckEngineAtNewFrame(KeyPress keys)
        {
            KeyPress = keys;

            Player.BehaviorAtNewFrame();
            Player.CheckIfHasBeenHit();
            CollectPickableItems();
            CheckInventoryUse();

            CurrentScreen.BehaviorAtNewFrame();

            // At last, every possiblities to change screen.
            if (Player.NewScreenEntrance.HasValue)
            {
                CurrentScreen = GetOrCreatetScreen(CurrentScreen.GetNextScreenIdFromDirection(Player.NewScreenEntrance.Value));
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
                    CurrentScreen = GetOrCreatetScreen(newScreenByDoor.Value);
                }
                else
                {
                    Pit pit = CurrentScreen.Pits.FirstOrDefault(p => p.ScreenIndexEntrance.HasValue && p.CanFallIn(Player));
                    if (pit != null)
                    {
                        CurrentScreen = GetOrCreatetScreen(pit.ScreenIndexEntrance.Value);
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
        public bool IsTriggered(FloorTrigger trigger)
        {
            return trigger.Overlap(Player) || CurrentScreen.Enemies.Any(e => trigger.Overlap(e));
        }

        /// <summary>
        /// Checks hit(s) made by enemies on player.
        /// </summary>
        /// <returns>Potential cost in life points.</returns>
        public double CheckHitByEnemiesOnPlayer()
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

        /// <summary>
        /// Checks if a door to a new screen has been opened.
        /// </summary>
        /// <param name="doorId">ByRef; door identifier.</param>
        /// <returns>New <see cref="Screen"/> identifier.</returns>
        public int? CheckForDoorOpened(out int? doorId)
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
    }
}
