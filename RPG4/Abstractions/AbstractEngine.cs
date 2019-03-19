using System.Collections.Generic;
using System.Linq;

namespace RPG4.Abstractions
{
    public class AbstractEngine
    {
        private Dictionary<Directions, int> _adjacentScreens;

        public Player Player { get; private set; }
        public List<Wall> Walls { get; private set; }
        public List<Wall> ConcreteWalls { get { return Walls.Where(w => w.Concrete).ToList(); } }
        public List<Enemy> Enemies { get; private set; }
        public List<WallTrigger> WallTriggers { get; private set; }
        public bool MeCollideToPng { get; set; }
        public bool MeCollideToWall { get; set; }

        public AbstractEngine(Player player, int screenIndex)
        {
            Player = player;

            SetEnginePropertiesFromScreenDatas(screenIndex);
        }

        private void SetEnginePropertiesFromScreenDatas(int screenIndex)
        {
            dynamic screenJsonDatas = Tools.GetScreenDatasFromIndex(screenIndex);

            Walls = new List<Wall>();
            Enemies = new List<Enemy>();
            WallTriggers = new List<WallTrigger>();
            MeCollideToWall = false;
            MeCollideToPng = false;

            foreach (dynamic wallJson in screenJsonDatas.Walls)
            {
                Walls.Add(new Wall(wallJson));
            }
            foreach (dynamic enemyJson in screenJsonDatas.Enemies)
            {
                Enemies.Add(new Enemy(enemyJson));
            }
            foreach (dynamic walltriggerJson in screenJsonDatas.WallTriggers)
            {
                WallTriggers.Add(new WallTrigger(walltriggerJson));
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

        public void CheckEngineAtTick(KeyPress keys)
        {
            Player.ComputeBehaviorAtTick(this, keys);
            
            foreach (var enemy in Enemies)
            {
                enemy.ComputeBehaviorAtTick(this, null);
            }

            MeCollideToPng = Enemies.Any(p => p.Overlap(Player));
            MeCollideToWall = ConcreteWalls.Any(w => w.Overlap(Player));
            Enemies.RemoveAll(p => ConcreteWalls.Any(w => w.Overlap(p)));

            // contrôle le kick
            if (!MeCollideToPng && !MeCollideToWall)
            {
                Enemies.RemoveAll(p => p.CheckHitAndHealthStatus(this));
            }

            foreach (var wt in WallTriggers)
            {
                wt.ComputeBehaviorAtTick(this, keys);
            }
            foreach (var w in Walls)
            {
                w.ComputeBehaviorAtTick(this, keys);
            }

            if (!MeCollideToPng && !MeCollideToWall && Player.NewScreenEntrance.HasValue)
            {
                SetEnginePropertiesFromScreenDatas(_adjacentScreens[Player.NewScreenEntrance.Value]);
            }
        }
    }
}
