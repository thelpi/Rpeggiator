using RPG4.Models.Enums;
using System.Linq;

namespace RPG4.Models.Sprites
{
    /// <summary>
    /// Represents a living <see cref="Sprite"/> (player, enemies, pngs...).
    /// </summary>
    /// <seealso cref="Sprite"/>
    /// <seealso cref="IExplodable"/>
    public class LifeSprite : Sprite, IExplodable
    {
        private double _originalSpeed;

        /// <summary>
        /// Maximal number of life points.
        /// </summary>
        public double MaximalLifePoints { get; private set; }
        /// <summary>
        /// Current number of life points.
        /// </summary>
        public double CurrentLifePoints { get; private set; }
        /// <summary>
        /// When hitting, indicates the life points cost on the enemy.
        /// </summary>
        public double HitLifePointCost { get; private set; }
        /// <inheritdoc />
        public double ExplosionLifePointCost { get { return Constants.Bomb.EXPLOSION_LIFE_POINT_COST; } }
        /// <summary>
        /// Inferred; current speed (i.e. distance, in pixels, by second)
        /// </summary>
        public double Speed
        {
            get
            {
                return _originalSpeed * CurrentFloor.SpeedRatio;
            }
        }
        /// <summary>
        /// Inferred; current <see cref="Floor"/> the player is standing on.
        /// </summary>
        public Floor CurrentFloor
        {
            get
            {
                return Engine.Default.CurrentScreen.Floors.FirstOrDefault(f =>
                    f.Overlap(this, Constants.FLOOR_CHANGE_OVERLAP_RATIO)
                ) ?? Engine.Default.CurrentScreen;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="graphic"><see cref="Sprite.Graphic"/></param>
        /// <param name="maximalLifePoints"><see cref="MaximalLifePoints"/></param>
        /// <param name="hitLifePointCost"><see cref="HitLifePointCost"/></param>
        /// <param name="speed"><see cref="_originalSpeed"/></param>
        protected LifeSprite(double x, double y, double width, double height, Graphic.ISpriteGraphic graphic,
            double maximalLifePoints, double hitLifePointCost, double speed)
            : base(x, y, width, height, graphic)
        {
            MaximalLifePoints = maximalLifePoints;
            CurrentLifePoints = maximalLifePoints;
            HitLifePointCost = hitLifePointCost;
            _originalSpeed = speed;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="lifeSpriteJson">The json dynamic object.</param>
        protected LifeSprite(dynamic lifeSpriteJson) : base((object)lifeSpriteJson)
        {
            MaximalLifePoints = lifeSpriteJson.MaximalLifePoints;
            CurrentLifePoints = MaximalLifePoints;
            HitLifePointCost = lifeSpriteJson.HitLifePointCost;
            _originalSpeed = lifeSpriteJson.Speed;
        }

        /// <summary>
        /// Recomputes <see cref="CurrentLifePoints"/> after an hit.
        /// </summary>
        /// <param name="lifePoints">Life points lost in the process.</param>
        protected void Hit(double lifePoints)
        {
            CurrentLifePoints -= lifePoints;
        }

        /// <summary>
        /// Checks if the instance is in-game dead.
        /// </summary>
        /// <param name="screen"><see cref="Screen"/></param>
        /// <returns><c>True</c> if death; <c>False</c> otherwise.</returns>
        public bool CheckDeath(Screen screen)
        {
            if (screen.Structures.Any(ss => ss.Overlap(this))
                || screen.Pits.Any(r => r.CanFallIn(this) && (GetType() == typeof(Enemy) || r.Deadly))
                || CurrentFloor.FloorType == FloorType.Lava)
            {
                CurrentLifePoints = 0;
            }

            return CurrentLifePoints.LowerEqual(0);
        }

        /// <summary>
        /// Regenerate instance life points.
        /// </summary>
        /// <param name="lifePoints">Life points gained count.</param>
        protected void RegenerateLifePoints(double lifePoints)
        {
            if (lifePoints.Greater(0))
            {
                CurrentLifePoints += lifePoints;
                CurrentLifePoints = CurrentLifePoints.Greater(MaximalLifePoints) ? MaximalLifePoints : CurrentLifePoints;
            }
        }
    }
}
