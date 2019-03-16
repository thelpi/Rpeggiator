using System.Collections.Generic;
using System.Linq;

namespace RPG4.Abstractions
{
    public class AbstractEngine
    {
        public PlayerBehavior Player { get; private set; }
        public double AreaHeight { get; private set; }
        public double AreaWidth { get; private set; }
        public List<SizedPoint> Walls { get; private set; }
        public List<PngBehavior> Pngs { get; private set; }
        public bool MeCollideToPng { get; set; }

        public AbstractEngine(PlayerBehavior player, double areaWidth, double areaHeight,
            List<SizedPoint> walls, List<PngBehavior> pngs)
        {
            Player = player;
            AreaHeight = areaHeight;
            AreaWidth = areaWidth;
            Walls = walls ?? new List<SizedPoint>();
            Pngs = pngs ?? new List<PngBehavior>();
        }

        public void CheckEngineAtTick(KeyPress keys)
        {
            Player.ComputeNewPositionAtTick(this, keys);
            
            foreach (var png in Pngs)
            {
                png.ComputeNewPositionAtTick(this, null);
            }

            MeCollideToPng = Pngs.Any(p => p.CollideWith(Player));

            // contrôle le kick
            if (!MeCollideToPng && keys.PressKick)
            {
                var pngsKicked = Pngs.Where(png => Player.CheckKick(png)).ToList();

                pngsKicked.ForEach(p => p.ApplyKick());

                Pngs.RemoveAll(p => p.KickCount >= p.KickTolerance);
            }
        }
    }
}
