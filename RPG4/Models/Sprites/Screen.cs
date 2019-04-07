using RPG4.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace RPG4.Models.Sprites
{
    /// <summary>
    /// Represents a screen.
    /// </summary>
    /// <seealso cref="Floor"/>
    public class Screen : Floor
    {
        // List of each instancied screen.
        private static List<Screen> _screens = new List<Screen>();

        private Dictionary<Direction, int> _neighboringScreens;
        private List<PermanentStructure> _permanentStructures;
        private List<Gate> _gates;
        private List<Rift> _rifts;
        private List<Enemy> _enemies;
        private List<GateTrigger> _gateTriggers;
        private List<PickableItem> _pickableItems;
        private List<ActionnedItem> _actionnedItems;
        private List<Pit> _pits;
        private List<Chest> _chests;
        private List<Door> _doors;
        private List<Floor> _floors;

        /// <summary>
        /// Current screen identifier..
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// Current screen darkness; opacity ratio between 0 (no) and 1 (full).
        /// </summary>
        public double DarknessOpacity { get; private set; }
        /// <summary>
        /// List of permanent structures.
        /// </summary>
        public IReadOnlyCollection<PermanentStructure> PermanentStructures { get { return _permanentStructures; } }
        /// <summary>
        /// List of <see cref="Door"/>.
        /// </summary>
        public IReadOnlyCollection<Door> Doors { get { return _doors; } }
        /// <summary>
        /// List of <see cref="Floor"/>.
        /// </summary>
        public IReadOnlyCollection<Floor> Floors { get { return _floors; } }
        /// <summary>
        /// List of <see cref="Pit"/>.
        /// </summary>
        public IReadOnlyCollection<Pit> Pits { get { return _pits; } }
        /// <summary>
        /// Inferred; list of closed <see cref="Chest"/>.
        /// </summary>
        public IReadOnlyCollection<Chest> ClosedChests { get { return _chests.Where(ch => !ch.IsOpen).ToList(); } }
        /// <summary>
        /// List of <see cref="Enemy"/>.
        /// </summary>
        public IReadOnlyCollection<Enemy> Enemies { get { return _enemies; } }
        /// <summary>
        /// List of <see cref="PickableItem"/>.
        /// </summary>
        public IReadOnlyCollection<PickableItem> PickableItems { get { return _pickableItems; } }
        /// <summary>
        /// Inferred; list of <see cref="Sprite"/> which can't be crossed.
        /// </summary>
        public IReadOnlyCollection<Sprite> Structures
        {
            get
            {
                List<Sprite> sprites = new List<Sprite>();
                sprites.AddRange(_permanentStructures);
                sprites.AddRange(_rifts);
                sprites.AddRange(_gates.Where(g => g.Activated));
                sprites.AddRange(_chests);
                sprites.AddRange(_doors);
                return sprites;
            }
        }
        /// <summary>
        /// Inferred; list of every <see cref="Sprite"/> which requires a display management at each frame.
        /// </summary>
        /// <remarks>Doesn't include <see cref="PermanentStructures"/>.</remarks>
        public IReadOnlyCollection<Sprite> AnimatedSprites
        {
            get
            {
                List<Sprite> sprites = new List<Sprite>();
                sprites.AddRange(_pickableItems);
                sprites.AddRange(_rifts);
                sprites.AddRange(_pits);
                sprites.AddRange(_actionnedItems);
                sprites.AddRange(_gateTriggers);
                sprites.AddRange(_enemies);
                sprites.AddRange(_gates.Where(g => g.Activated));
                sprites.AddRange(_chests);
                sprites.AddRange(_doors);
                return sprites;
            }
        }

        /// <summary>
        /// Gets a screen by its identifier.
        /// </summary>
        /// <param name="id"><see cref="Id"/></param>
        /// <returns><see cref="Screen"/></returns>
        public static Screen GetScreen(int id)
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
        /// Private constructor.
        /// </summary>
        /// <param name="id"><see cref="Id"/></param>
        /// <param name="screenJsonDatas">Screen json datas.</param>
        private Screen(int id, dynamic screenJsonDatas) : base((object)screenJsonDatas)
        {
            Id = id;
            _permanentStructures = new List<PermanentStructure>();
            _doors = new List<Door>();
            _floors = new List<Floor>();
            _enemies = new List<Enemy>();
            _gateTriggers = new List<GateTrigger>();
            _gates = new List<Gate>();
            _rifts = new List<Rift>();
            _pits = new List<Pit>();
            _chests = new List<Chest>();
            _pickableItems = new List<PickableItem>();
            _actionnedItems = new List<ActionnedItem>();
            DarknessOpacity = screenJsonDatas.AreaDarknessOpacity;
            foreach (dynamic structureJson in screenJsonDatas.PermanentStructures)
            {
                _permanentStructures.Add(new PermanentStructure(structureJson));
            }
            foreach (dynamic gateJson in screenJsonDatas.Gates)
            {
                _gates.Add(new Gate(gateJson));
            }
            foreach (dynamic riftJson in screenJsonDatas.Rifts)
            {
                _rifts.Add(new Rift(riftJson));
            }
            foreach (dynamic pitJson in screenJsonDatas.Pits)
            {
                _pits.Add(new Pit(pitJson));
            }
            foreach (dynamic chestJson in screenJsonDatas.Chests)
            {
                _chests.Add(new Chest(chestJson));
            }
            foreach (dynamic doorJson in screenJsonDatas.Doors)
            {
                _doors.Add(new Door(doorJson));
            }
            foreach (dynamic floorJson in screenJsonDatas.Floors)
            {
                _floors.Add(new Floor(floorJson));
            }
            foreach (dynamic enemyJson in screenJsonDatas.Enemies)
            {
                _enemies.Add(new Enemy(enemyJson));
            }
            foreach (dynamic gatetriggerJson in screenJsonDatas.GateTriggers)
            {
                _gateTriggers.Add(new GateTrigger(gatetriggerJson));
            }
            foreach (dynamic itemJson in screenJsonDatas.Items)
            {
                _pickableItems.Add(new PickableItem(itemJson));
            }
            dynamic neighboringScreens = screenJsonDatas.NeighboringScreens;
            _neighboringScreens = new Dictionary<Direction, int>
            {
                { Direction.Bottom, (int)neighboringScreens.Bottom },
                { Direction.Left, (int)neighboringScreens.Left },
                { Direction.Right, (int)neighboringScreens.Right },
                { Direction.Top, (int)neighboringScreens.Top }
            };
        }

        /// <summary>
        /// Checks quantity of each <see cref="PickableItem"/>.
        /// </summary>
        public void CheckPickableItemsQuantities()
        {
            _pickableItems.RemoveAll(pi => pi.Quantity <= 0);
        }

        /// <inheritdoc />
        public override void BehaviorAtNewFrame()
        {
            _enemies.ForEach(enemy => enemy.BehaviorAtNewFrame());
            _gateTriggers.ForEach(gt => gt.BehaviorAtNewFrame());
            _gates.ForEach(g => g.BehaviorAtNewFrame());
            _rifts.ForEach(r => r.BehaviorAtNewFrame());
            _actionnedItems.ForEach(di => di.BehaviorAtNewFrame());
            _enemies.ForEach(e => e.CheckIfHasBeenHit());
            _enemies.RemoveAll(e =>
            {
                bool death = e.CheckDeath(this);
                if (death && e.LootQuantity > 0)
                {
                    _pickableItems.Add(PickableItem.Loot(e, e.LootItemId, e.LootQuantity));
                }
                return death;
            });
            _rifts.RemoveAll(r => r.LifePoints.LowerEqual(0));
            _actionnedItems.RemoveAll(di => di.IsDone || Structures.Any(cw => cw.Overlap(di)));
            _pickableItems.RemoveAll(pi => pi.Disapear);
        }

        /// <summary>
        /// Gets a list of <see cref="GateTrigger"/> associated to the specified <see cref="Gate"/>.
        /// </summary>
        /// <param name="gate"><see cref="Gate"/></param>
        /// <returns>List of <see cref="GateTrigger"/>.</returns>
        public IReadOnlyCollection<GateTrigger> GetTriggersForSpecifiedGate(Gate gate)
        {
            return _gateTriggers.Where(gt => gt.GateIndex == _gates.IndexOf(gate) && gt.IsActivated).ToList();
        }

        /// <summary>
        /// Checks if a bomb (or several) is currently exploding near to <paramref name="sprite"/>.
        /// </summary>
        /// <typeparam name="T">Sprite type; must inherit from <see cref="IExplodable"/>.</typeparam>
        /// <param name="sprite"><see cref="Sprite"/></param>
        /// <returns>Life points lost.</returns>
        public double OverlapAnExplodingBomb<T>(T sprite) where T : Sprite, IExplodable
        {
            return _actionnedItems.Where(di => di is ActionnedBomb).Sum(b => (b as ActionnedBomb).GetLifePointsCost(sprite));
        }

        /// <summary>
        /// Checks if an arrow (or several) has reached the specified <paramref name="sprite"/>.
        /// </summary>
        /// <param name="sprite"><see cref="Sprite"/></param>
        /// <returns>Life points lost.</returns>
        public double OverlapAnArrow(LifeSprite sprite)
        {
            return _actionnedItems.Where(di => di is ActionnedArrow).Sum(b => (b as ActionnedArrow).GetLifePointsCost(sprite));
        }

        /// <summary>
        /// Freeze movements for every <see cref="Enemies"/>.
        /// </summary>
        public void FreezeEnemies()
        {
            _enemies.ForEach(e => e.Freeze());
        }

        /// <summary>
        /// Adds an <see cref="ActionnedItem"/>
        /// </summary>
        /// <param name="itemDropped"><see cref="ActionnedItem"/> to add.</param>
        public void AddDroppedItem(ActionnedItem itemDropped)
        {
            _actionnedItems.Add(itemDropped);
        }

        /// <summary>
        /// Gets the next <see cref="Screen"/> from a <see cref="Direction"/>.
        /// </summary>
        /// <param name="direction"><see cref="Direction"/></param>
        /// <returns><see cref="Screen"/></returns>
        public Screen GetNextScreenFromDirection(Direction direction)
        {
            // Ensures a non-corner direction.
            if (direction == Direction.BottomLeft)
            {
                direction = Direction.Left;
            }
            else if (direction == Direction.BottomRight)
            {
                direction = Direction.Bottom;
            }
            else if (direction == Direction.TopLeft)
            {
                direction = Direction.Top;
            }
            else if (direction == Direction.TopRight)
            {
                direction = Direction.Right;
            }

            return GetScreen(_neighboringScreens[direction]);
        }
    }
}
