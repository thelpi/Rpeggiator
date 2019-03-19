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
            
            foreach (var enemy in Enemies)
            {
                enemy.ComputeBehaviorAtTick(this, null);
            }

            _enemies.RemoveAll(p => ConcreteWalls.Any(w => w.Overlap(p)));
            
            if (!PlayerOverlapEnemy && !PlayerOverlapWall)
            {
                _enemies.RemoveAll(p => p.CheckHitAndHealthStatus(this));
            }

            foreach (var wt in WallTriggers)
            {
                wt.ComputeBehaviorAtTick(this, keys);
            }
            foreach (var w in Walls)
            {
                w.ComputeBehaviorAtTick(this, keys);
            }

            if (!PlayerOverlapEnemy && !PlayerOverlapWall && Player.NewScreenEntrance.HasValue)
            {
                SetEnginePropertiesFromScreenDatas(_adjacentScreens[Player.NewScreenEntrance.Value]);
            }
        }
    }
}
