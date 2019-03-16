using System.Collections.Generic;
using System.Linq;

namespace RPG4.Abstractions
{
    public class AbstractEngine
    {
        private const int KICK_TICK_MAX_COUNT = 2;

        private int _kickTickCount;

        public Player Player { get; private set; }
        public double AreaHeight { get; private set; }
        public double AreaWidth { get; private set; }
        public List<Wall> Walls { get; private set; }
        public List<Wall> ConcreteWalls { get { return Walls.Where(w => w.Concrete).ToList(); } }
        public List<Enemy> Enemies { get; private set; }
        public List<WallTrigger> WallTriggers { get; private set; }
        public bool MeCollideToPng { get; set; }
        public bool MeCollideToWall { get; set; }
        public bool IsKicking { get { return _kickTickCount >= 0; } }

        public AbstractEngine(Player player, dynamic screenJsonDatas)
        {
            Player = player;
            AreaHeight = screenJsonDatas.AreaHeight;
            AreaWidth = screenJsonDatas.AreaWidth;
            Walls = new List<Wall>();
            Enemies = new List<Enemy>();
            WallTriggers = new List<WallTrigger>();
            _kickTickCount = -1;

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
        }

        public void CheckEngineAtTick(KeyPress keys)
        {
            // gère le temps d'effet du kick
            if (_kickTickCount >= KICK_TICK_MAX_COUNT)
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
        }
    }
}
