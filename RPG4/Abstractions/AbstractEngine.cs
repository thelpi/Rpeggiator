using System.Collections.Generic;
using System.Linq;

namespace RPG4.Abstractions
{
    public class AbstractEngine
    {
        private const int KICK_TICK_MAX_COUNT = 2;

        private int _kickTickCount;

        public PlayerBehavior Player { get; private set; }
        public double AreaHeight { get; private set; }
        public double AreaWidth { get; private set; }
        public List<SizedPoint> Walls { get; private set; }
        public List<PngBehavior> Pngs { get; private set; }
        public bool MeCollideToPng { get; set; }
        public bool IsKicking { get { return _kickTickCount >= 0; } }

        public AbstractEngine(PlayerBehavior player, dynamic screenJsonDatas)
        {
            Player = player;
            AreaHeight = screenJsonDatas.AreaHeight;
            AreaWidth = screenJsonDatas.AreaWidth;
            Walls = new List<SizedPoint>();
            Pngs = new List<PngBehavior>();
            _kickTickCount = -1;

            foreach (dynamic wallJson in screenJsonDatas.Walls)
            {
                Walls.Add(SizedPoint.FromDynamic(wallJson));
            }
            foreach (dynamic pngJson in screenJsonDatas.Pngs)
            {
                Pngs.Add(PngBehavior.FromDynamic(pngJson));
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

            Player.ComputeNewPositionAtTick(this, keys);
            
            foreach (var png in Pngs)
            {
                png.ComputeNewPositionAtTick(this, null);
            }

            MeCollideToPng = Pngs.Any(p => p.Overlap(Player));

            // contrôle le kick
            if (!MeCollideToPng && IsKicking)
            {
                var pngsKicked = Pngs.Where(png => Player.CheckKick(png)).ToList();

                pngsKicked.ForEach(p => p.ApplyKick());

                Pngs.RemoveAll(p => p.KickCount >= p.KickTolerance);
            }
        }
    }
}
