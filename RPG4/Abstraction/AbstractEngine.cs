using RPG4.Abstraction.Graphic;
using RPG4.Abstraction.Sprites;
using System.Collections.Generic;
using System.Linq;

namespace RPG4.Abstraction
{
    /// <summary>
    /// Game engine.
    /// </summary>
    public class AbstractEngine
    {
        private Dictionary<Directions, int> _adjacentScreens;
        private List<Sprite> _walls;
        private List<Gate> _gates;
        private List<Rift> _rifts;
        private List<Enemy> _enemies;
        private List<GateTrigger> _gateTriggers;
        private List<PickableItem> _pickableItems;
        private List<ActionnedItem> _actionnedItems;
        private List<Pit> _pits;

        /// <summary>
        /// Current screen width.
        /// </summary>
        public double AreaWidth { get; private set; }
        /// <summary>
        /// Current screen height.
        /// </summary>
        public double AreaHeight { get; private set; }
        /// <summary>
        /// Current screen shadow.
        /// </summary>
        public double AreaShadowOpacity { get; private set; }
        /// <summary>
        /// <see cref="Player"/>
        /// </summary>
        public Player Player { get; private set; }
        /// <summary>
        /// List of walls.
        /// </summary>
        public IReadOnlyCollection<Sprite> Walls { get { return _walls; } }
        /// <summary>
        /// Inferred; list of <see cref="Sprite"/> which can't be crossed.
        /// </summary>
        public IReadOnlyCollection<Sprite> SolidStructures { get { return _walls.Concat(_rifts).Concat(_gates.Where(g => g.Activated)).ToList(); } }
        /// <summary>
        /// List of <see cref="Pit"/>.
        /// </summary>
        public IReadOnlyCollection<Pit> Pits { get { return _pits; } }
        /// <summary>
        /// List of every <see cref="Sprite"/> which requires a display management at each frame.
        /// </summary>
        /// <remarks>Doesn't include <see cref="Walls"/>.</remarks>
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
                return sprites;
            }
        }
        /// <summary>
        /// Current screen identifier..
        /// </summary>
        public int CurrentScreenId { get; private set; }
        /// <summary>
        /// Frames count.
        /// </summary>
        public ulong FramesCount { get; private set; }
        /// <summary>
        /// Screen graphic rendering.
        /// </summary>
        public ISpriteGraphic ScreenGraphic { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="screenIndex">The screen index.</param>
        public AbstractEngine(int screenIndex)
        {
            Player = new Player();

            SetEnginePropertiesFromScreenDatas(screenIndex);
        }

        // proceeds to every changes implied by a new screen
        private void SetEnginePropertiesFromScreenDatas(int screenId)
        {
            CurrentScreenId = screenId;

            dynamic screenJsonDatas = Tools.GetScreenDatasFromIndex(screenId);

            _walls = new List<Sprite>();
            _enemies = new List<Enemy>();
            _gateTriggers = new List<GateTrigger>();
            _gates = new List<Gate>();
            _rifts = new List<Rift>();
            _pits = new List<Pit>();
            _pickableItems = new List<PickableItem>();
            _actionnedItems = new List<ActionnedItem>();
            AreaWidth = screenJsonDatas.AreaWidth;
            AreaHeight = screenJsonDatas.AreaHeight;
            AreaShadowOpacity = screenJsonDatas.AreaShadowOpacity;
            switch ((string)screenJsonDatas.GraphicType)
            {
                case nameof(ImageBrushGraphic):
                    ScreenGraphic = new ImageBrushGraphic((string)screenJsonDatas.ImagePath);
                    break;
                case nameof(PlainBrushGraphic):
                    ScreenGraphic = new PlainBrushGraphic((string)screenJsonDatas.HexColor);
                    break;
                    // TODO : other types of ISpriteGraphic must be implemented here.
            }

            foreach (dynamic wallJson in screenJsonDatas.Walls)
            {
                _walls.Add(new Sprite(wallJson));
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
            dynamic adjacentScreens = screenJsonDatas.AdjacentScreens;
            _adjacentScreens = new Dictionary<Directions, int>
            {
                { Directions.bottom, (int)adjacentScreens.bottom },
                { Directions.bottom_left, (int)adjacentScreens.bottom_left },
                { Directions.bottom_right, (int)adjacentScreens.bottom_right },
                { Directions.left, (int)adjacentScreens.left },
                { Directions.right, (int)adjacentScreens.right },
                { Directions.top, (int)adjacentScreens.top },
                { Directions.top_left, (int)adjacentScreens.top_left },
                { Directions.top_right, (int)adjacentScreens.top_right },
            };
        }

        /// <summary>
        /// Refresh the status of every components at new frame.
        /// </summary>
        /// <param name="keys"><see cref="KeyPress"/></param>
        public void CheckEngineAtNewFrame(KeyPress keys)
        {
            // 0 - Frames count.
            FramesCount++;

            // 1 - Player.
            Player.BehaviorAtNewFrame(this, keys);
            // 2 - Enemies.
            _enemies.ForEach(enemy => enemy.BehaviorAtNewFrame(this));
            // 3 - Items pick.
            CheckItemsToPick();
            // 4 - Items use.
            CheckInventoryUse(keys);
            // 5 - Gate triggers.
            _gateTriggers.ForEach(gt => gt.BehaviorAtNewFrame(this));
            // 6 - Gates.
            _gates.ForEach(g => g.BehaviorAtNewFrame(this));
            // 7 - Rifts.
            _rifts.ForEach(r => r.BehaviorAtNewFrame(this));
            // 8 - Actionned items.
            _actionnedItems.ForEach(di => di.BehaviorAtNewFrame(this));
            // 9 - Check hit and death on each instances where it's applicable.
            Player.CheckIfHasBeenHit(this);
            _enemies.ForEach(e => e.CheckIfHasBeenHit(this));
            _enemies.RemoveAll(e => e.CheckDeath(this));
            _rifts.RemoveAll(r => r.LifePoints <= 0);
            _actionnedItems.RemoveAll(di => di.IsDone || SolidStructures.Any(cw => cw.Overlap(di)));
            // 10 - New screen.
            if (Player.NewScreenEntrance.HasValue)
            {
                SetEnginePropertiesFromScreenDatas(_adjacentScreens[Player.NewScreenEntrance.Value]);
            }
            else
            {
                // 11 - Checks for pits
                Pit pit = _pits.FirstOrDefault(p => p.ScreenIndexEntrance.HasValue && p.CanFallIn(Player));
                if (pit != null)
                {
                    SetEnginePropertiesFromScreenDatas(pit.ScreenIndexEntrance.Value);
                }
            }
        }

        // Checks inventory use.
        private void CheckInventoryUse(KeyPress keys)
        {
            if (keys.InventorySlotId.HasValue)
            {
                var itemDropped = Player.Inventory.UseItem(this, keys.InventorySlotId.Value);
                if (itemDropped != null)
                {
                    _actionnedItems.Add(itemDropped);
                }
            }
        }

        // Checks for items to pick on the current position.
        private void CheckItemsToPick()
        {
            var items = _pickableItems.Where(it => it.Overlap(Player)).ToList();
            foreach (var item in items)
            {
                item.Pick(this);
                if (item.Quantity == 0)
                {
                    _pickableItems.Remove(item);
                }
            }
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
        /// Checks if the specified <see cref="FloorTrigger"/> has been triggered by the environnement.
        /// </summary>
        /// <param name="trigger"><see cref="FloorTrigger"/></param>
        /// <returns><c>True</c> if triggered; <c>False</c> otherwise.</returns>
        public bool IsTriggered(FloorTrigger trigger)
        {
            return trigger.Overlap(Player) || _enemies.Any(e => trigger.Overlap(e));
        }

        /// <summary>
        /// Checks hit(s) made by enemies on player.
        /// </summary>
        /// <returns>Potential cost in life points.</returns>
        public double CheckHitByEnemiesOnPlayer()
        {
            // checks hit (for each enemy, life points lost is cumulable)
            double cumuledLifePoints = 0;
            foreach (var enemy in _enemies)
            {
                if (Player.Overlap(enemy))
                {
                    cumuledLifePoints += enemy.HitLifePointCost;
                }
            }
            return cumuledLifePoints;
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
    }
}
