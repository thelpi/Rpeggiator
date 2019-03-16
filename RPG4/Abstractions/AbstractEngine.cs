using System.Collections.Generic;
using System.Linq;

namespace RPG4.Abstractions
{
    public class AbstractEngine
    {
        private int _kickTickCount;
        private Dictionary<Directions, int> _adjacentScreens;

        public Player Player { get; private set; }
        public List<Wall> Walls { get; private set; }
        public List<Wall> ConcreteWalls { get { return Walls.Where(w => w.Concrete).ToList(); } }
        public List<Enemy> Enemies { get; private set; }
        public List<WallTrigger> WallTriggers { get; private set; }
        public bool MeCollideToPng { get; set; }
        public bool MeCollideToWall { get; set; }
        public bool IsKicking { get { return _kickTickCount >= 0; } }

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
            _kickTickCount = -1;
            MeCollideToWall = false;
            MeCollideToPng = false;

            foreach (dynamic wallJson in screenJsonDatas.Walls)
            {
                Walls.Add(new Wall(wallJson));
            }
            foreach (dynamic pngJson in screenJsonDatas.Pngs)
            {
                Enemies.Add(new Enemy(pngJson));
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
            // gère le temps d'effet du kick
            if (_kickTickCount >= Constants.KICK_TICK_MAX_COUNT)
            {
                _kickTickCount = -1;
            }
            else if (_kickTickCount >= 0)
            {
                _kickTickCount += 1;
            }
            else if (keys.PressKick)
            {
                _kickTickCount = 0;
            }

            Player.ComputeBehaviorAtTick(this, keys);
            
            foreach (var enemy in Enemies)
            {
                enemy.ComputeBehaviorAtTick(this, null);
            }

            MeCollideToPng = Enemies.Any(p => p.Overlap(Player));
            MeCollideToWall = ConcreteWalls.Any(w => w.Overlap(Player));
            Enemies.RemoveAll(p => ConcreteWalls.Any(w => w.Overlap(p)));

            // contrôle le kick
            if (!MeCollideToPng && !MeCollideToWall && IsKicking)
            {
                var pngsKicked = Enemies.Where(enemy => Player.CheckKick(enemy)).ToList();

                pngsKicked.ForEach(p => p.ApplyKick());

                Enemies.RemoveAll(p => p.KickCount >= p.KickTolerance);
            }

            foreach (var wt in WallTriggers)
            {
                wt.ComputeBehaviorAtTick(this, keys);
            }

            if (!MeCollideToPng && !MeCollideToWall && Player.NewScreenEntrance.HasValue)
            {
                SetEnginePropertiesFromScreenDatas(_adjacentScreens[Player.NewScreenEntrance.Value]);
            }
        }
    }
}
