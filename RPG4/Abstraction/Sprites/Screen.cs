using System.Collections.Generic;
using System.Linq;

namespace RPG4.Abstraction.Sprites
{
    /// <summary>
    /// Represents a screen.
    /// </summary>
    /// <seealso cref="Sprite"/>
    public class Screen : Sprite
    {
        // List of each instancied screen.
        private static List<Screen> _screens = new List<Screen>();

        private Dictionary<Directions, int> _neighboringScreens;
        private List<Sprite> _permanentStructures;
        private List<Gate> _gates;
        private List<Rift> _rifts;
        private List<Enemy> _enemies;
        private List<GateTrigger> _gateTriggers;
        private List<PickableItem> _pickableItems;
        private List<ActionnedItem> _actionnedItems;
        private List<Pit> _pits;
        private List<Chest> _chests;

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
        public IReadOnlyCollection<Sprite> PermanentStructures { get { return _permanentStructures; } }
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

            var screen = new Screen(id, screenJsonDatas);

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
            _permanentStructures = new List<Sprite>();
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
                _permanentStructures.Add(new Sprite(structureJson));
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
            _neighboringScreens = new Dictionary<Directions, int>
            {
                { Directions.bottom, (int)neighboringScreens.bottom },
                { Directions.bottom_left, (int)neighboringScreens.bottom_left },
                { Directions.bottom_right, (int)neighboringScreens.bottom_right },
                { Directions.left, (int)neighboringScreens.left },
                { Directions.right, (int)neighboringScreens.right },
                { Directions.top, (int)neighboringScreens.top },
                { Directions.top_left, (int)neighboringScreens.top_left },
                { Directions.top_right, (int)neighboringScreens.top_right },
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
            _rifts.RemoveAll(r => r.LifePoints <= 0);
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
        /// Checks if a bomb (or several) is currently exploding near to a <see cref="Sprite"/>.
        /// </summary>
        /// <typeparam name="T">Type of sprite requirement (must inherit from <see cref="IExplodable"/>).</typeparam>
        /// <param name="sprite"><see cref="Sprite"/></param>
        /// <returns>Life points lost.</returns>
        public double OverlapAnExplodingBomb<T>(T sprite) where T : Sprite, IExplodable
        {
            return _actionnedItems.Where(di => di is ActionnedBomb).Sum(b => (b as ActionnedBomb).GetLifePointCost(sprite));
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
        /// Gets the next <see cref="Screen"/> from a <see cref="Directions"/>.
        /// </summary>
        /// <param name="direction"><see cref="Directions"/></param>
        /// <returns><see cref="Screen"/></returns>
        public Screen GetNextScreenFromDirection(Directions direction)
        {
            return GetScreen(_neighboringScreens[direction]);
        }
    }
}
