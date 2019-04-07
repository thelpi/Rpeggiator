using RPG4.Models.Enums;
using RPG4.Models.Graphic;
using System.Linq;

namespace RPG4.Models.Sprites
{
    /// <summary>
    /// Represents a living <see cref="Sprite"/> (player, enemies, pngs...).
    /// </summary>
    /// <seealso cref="DamageableSprite"/>
    public class LifeSprite : DamageableSprite
    {
        // Original speed.
        private double _originalSpeed;
        // Recovery time manager.
        private Elapser _recoveryManager;
        // Recovery time, in milliseconds.
        private double _recoveryTime;
        // Recovery graphic.
        private ISpriteGraphic _recoveryGraphic;

        /// <summary>
        /// Maximal number of life points.
        /// </summary>
        public double MaximalLifePoints { get; private set; }
        /// <summary>
        /// When hitting, indicates the life points cost on the enemy.
        /// </summary>
        public double HitLifePointCost { get; private set; }
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
        /// Indicates the player is currently recovering from an hit.
        /// </summary>
        public bool IsRecovering { get { return _recoveryManager?.Elapsed == false; } }
        /// <inheritdoc />
        public override ISpriteGraphic Graphic { get { return IsRecovering ? Constants.Player.RECOVERY_GRAPHIC : base.Graphic; } }

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
        /// <param name="recoveryTime"><see cref="_recoveryTime"/></param>
        /// <param name="recoveryGraphic"><see cref="_recoveryGraphic"/></param>
        protected LifeSprite(double x, double y, double width, double height, ISpriteGraphic graphic,
            double maximalLifePoints, double hitLifePointCost, double speed, double recoveryTime, ISpriteGraphic recoveryGraphic)
            : base(x, y, width, height, graphic, maximalLifePoints)
        {
            MaximalLifePoints = maximalLifePoints;
            HitLifePointCost = hitLifePointCost;
            _originalSpeed = speed;
            _recoveryManager = null;
            _recoveryTime = recoveryTime;
            _recoveryGraphic = recoveryGraphic;
            ExplosionLifePointCost = Constants.Bomb.EXPLOSION_LIFE_POINT_COST;
            ArrowLifePointCost = Constants.Arrow.LIFE_POINT_COST;
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
            // TODO : set both values in JSON.
            _recoveryTime = 0;
            _recoveryGraphic = null;
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

        /// <summary>
        /// Checks if the instance has been hit.
        /// </summary>
        /// <returns><c>True</c> if has been hit; <c>False</c> otherwise.</returns>
        public virtual bool CheckIfHasBeenHit()
        {
            // currently recovering ?
            if (_recoveryManager?.Elapsed == true)
            {
                _recoveryManager = null;
            }

            if (_recoveryManager == null)
            {
                double cumuledLifePoints = Engine.Default.CurrentScreen.HitByAnActionnedItem(this);

                // TODO : fix this IF
                if (GetType() == typeof(Enemy))
                {
                    if (Engine.Default.Player.IsHitting && Overlap(Engine.Default.Player.HitSprite))
                    {
                        cumuledLifePoints += Engine.Default.Player.HitLifePointCost;
                    }
                }
                else
                {
                    cumuledLifePoints += Engine.Default.CheckHitByEnemiesOnPlayer();
                }

                if (cumuledLifePoints.Greater(0))
                {
                    CurrentLifePoints -= cumuledLifePoints;
                    _recoveryManager = _recoveryTime > 0 ? new Elapser(_recoveryTime) : null;
                    return true;
                }
            }

            return false;
        }
    }
}
