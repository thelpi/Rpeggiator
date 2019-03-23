using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RPG4.Abstractions
{
    /// <summary>
    /// Game engine.
    /// </summary>
    public class AbstractEngine
    {
        private Dictionary<Directions, int> _adjacentScreens;
        private List<Wall> _walls;
        private List<Enemy> _enemies;
        private List<WallTrigger> _wallTriggers;
        private List<FloorItem> _items;
        private List<Bomb> _bombs;

        /// <summary>
        /// Current screen width.
        /// </summary>
        public double AreaWidth { get; private set; }
        /// <summary>
        /// Current screen height.
        /// </summary>
        public double AreaHeight { get; private set; }
        /// <summary>
        /// <see cref="Player"/>
        /// </summary>
        public Player Player { get; private set; }
        /// <summary>
        /// List of <see cref="Wall"/> (concrete or not).
        /// </summary>
        public ReadOnlyCollection<Wall> Walls { get { return _walls.AsReadOnly(); } }
        /// <summary>
        /// Inferred; list of concrete <see cref="Wall"/>.
        /// </summary>
        public ReadOnlyCollection<Wall> ConcreteWalls { get { return _walls.Where(w => w.Concrete).ToList().AsReadOnly(); } }
        /// <summary>
        /// List of <see cref="Enemy"/>.
        /// </summary>
        public ReadOnlyCollection<Enemy> Enemies { get { return _enemies.AsReadOnly(); } }
        /// <summary>
        /// List of <see cref="WallTrigger"/>.
        /// </summary>
        public ReadOnlyCollection<WallTrigger> WallTriggers { get { return _wallTriggers.AsReadOnly(); } }
        /// <summary>
        /// List of <see cref="FloorItem"/>.
        /// </summary>
        public ReadOnlyCollection<FloorItem> Items { get { return _items.AsReadOnly(); } }
        /// <summary>
        /// List of <see cref="Bomb"/>.
        /// </summary>
        public ReadOnlyCollection<Bomb> Bombs { get { return _bombs.AsReadOnly(); } }
        /// <summary>
        /// Inferred; indicates it the player overlaps an enemy.
        /// </summary>
        public bool PlayerOverlapEnemy { get { return Enemies.Any(p => p.Overlap(Player)); } }
        /// <summary>
        /// Inferred; indicates it the player overlaps a wall.
        /// </summary>
        public bool PlayerOverlapWall { get { return ConcreteWalls.Any(w => w.Overlap(Player)); } }

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
        private void SetEnginePropertiesFromScreenDatas(int screenIndex)
        {
            dynamic screenJsonDatas = Tools.GetScreenDatasFromIndex(screenIndex);

            _walls = new List<Wall>();
            _enemies = new List<Enemy>();
            _wallTriggers = new List<WallTrigger>();
            _items = new List<FloorItem>();
            _bombs = new List<Bomb>();
            AreaWidth = screenJsonDatas.AreaWidth;
            AreaHeight = screenJsonDatas.AreaHeight;

            foreach (dynamic wallJson in screenJsonDatas.Walls)
            {
                _walls.Add(new Wall(wallJson));
            }
            foreach (dynamic enemyJson in screenJsonDatas.Enemies)
            {
                _enemies.Add(new Enemy(enemyJson));
            }
            foreach (dynamic walltriggerJson in screenJsonDatas.WallTriggers)
            {
                _wallTriggers.Add(new WallTrigger(walltriggerJson));
            }
            foreach (dynamic itemJson in screenJsonDatas.Items)
            {
                _items.Add(new FloorItem(itemJson));
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
        /// Refresh the status of every components at tick.
        /// </summary>
        /// <param name="keys"><see cref="KeyPress"/></param>
        public void CheckEngineAtTick(KeyPress keys)
        {
            Player.ComputeBehaviorAtTick(this, keys);

            // checks for items on the new position
            var items = _items.Where(it => it.Overlap(Player)).ToList();
            foreach (var item in items)
            {
                if (Player.Inventory.TryAdd(item.ItemId, item.Quantity))
                {
                    _items.Remove(item);
                }
            }

            // checks for bombs dropped on the new position
            if (keys.InventorySlotId.HasValue)
            {
                int indexId = Player.Inventory.GetSlotByItemId(Item.BOMB_ID);
                if (keys.InventorySlotId.Value == indexId)
                {
                    _bombs.Add(new Bomb(Player.X, Player.Y, Constants.BOMB_WIDTH, Constants.BOMB_HEIGHT));
                    Player.Inventory.UseItem(Item.BOMB_ID);
                }
            }

            foreach (var enemy in _enemies)
            {
                enemy.ComputeBehaviorAtTick(this, null);
            }

            _enemies.RemoveAll(p => ConcreteWalls.Any(w => w.Overlap(p)));
            
            if (!PlayerOverlapEnemy && !PlayerOverlapWall)
            {
                _enemies.RemoveAll(p => p.CheckHitAndHealthStatus(this));
            }

            foreach (var wt in _wallTriggers)
            {
                wt.ComputeBehaviorAtTick(this, keys);
            }
            foreach (var w in _walls)
            {
                w.ComputeBehaviorAtTick(this, keys);
            }

            _bombs.RemoveAll(b => (!b.IsPending && !b.DisplayHalo) || ConcreteWalls.Any(cw => cw.Overlap(b)));
            foreach (var b in _bombs)
            {
                b.ComputeBehaviorAtTick(this);
            }

            if (!PlayerOverlapEnemy && !PlayerOverlapWall && Player.NewScreenEntrance.HasValue)
            {
                SetEnginePropertiesFromScreenDatas(_adjacentScreens[Player.NewScreenEntrance.Value]);
            }
        }
    }
}
