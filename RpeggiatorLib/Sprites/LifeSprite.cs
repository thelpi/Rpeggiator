using System.Linq;
using RpeggiatorLib.Enums;
using RpeggiatorLib.Renders;

namespace RpeggiatorLib.Sprites
{
    /// <summary>
    /// Represents a living <see cref="Sprite"/> (player, enemies, pngs...).
    /// </summary>
    /// <seealso cref="DamageableSprite"/>
    public abstract class LifeSprite : DamageableSprite
    {
        // Original speed.
        private double _originalSpeed;
        // Recovery time manager.
        private Elapser _recoveryManager;
        // Recovery time, in milliseconds.
        private double _recoveryTime;
        // Render while recovering.
        private Render _renderRecovery;

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
        /// <summary>
        /// Indicates the sprite direction.
        /// </summary>
        public Direction Direction { get; protected set; }
        /// <inheritdoc />
        public override Render Render { get { return RecoveryRenderSwitch(); } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"><see cref="Sprite.Id"/></param>
        /// <param name="x"><see cref="Sprite.X"/></param>
        /// <param name="y"><see cref="Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprite.Height"/></param>
        /// <param name="maximalLifePoints"><see cref="MaximalLifePoints"/></param>
        /// <param name="hitLifePointCost"><see cref="HitLifePointCost"/></param>
        /// <param name="speed"><see cref="_originalSpeed"/></param>
        /// <param name="recoveryTime"><see cref="_recoveryTime"/></param>
        /// <param name="renderFilename">File name for <see cref="Sprite._render"/>.</param>
        /// <param name="renderRecoveryFilename">File name for <see cref="_renderRecovery"/>.</param>
        /// <param name="defaultDirection">Default <see cref="Direction"/>.</param>
        protected LifeSprite(int id, double x, double y, double width, double height, double maximalLifePoints, double hitLifePointCost,
            double speed, double recoveryTime, string renderFilename, string renderRecoveryFilename, Direction defaultDirection)
            : base(id, x, y, width, height, maximalLifePoints, nameof(ImageDirectionRender), new[] { renderFilename, nameof(Direction) })
        {
            Direction = defaultDirection;
            MaximalLifePoints = maximalLifePoints;
            HitLifePointCost = hitLifePointCost;
            _originalSpeed = speed;
            _recoveryManager = null;
            _recoveryTime = recoveryTime;
            ExplosionLifePointCost = Constants.Bomb.EXPLOSION_LIFE_POINT_COST;
            ArrowLifePointCost = Constants.Arrow.LIFE_POINT_COST;
            _renderRecovery = new ImageDirectionRender(renderRecoveryFilename, this, nameof(Direction));
        }

        /// <summary>
        /// Checks if the instance is in-game dead.
        /// </summary>
        /// <param name="screen"><see cref="Screen"/></param>
        /// <returns><c>True</c> if death; <c>False</c> otherwise.</returns>
        internal bool CheckDeath(Screen screen)
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
        internal virtual bool CheckIfHasBeenHit()
        {
            // currently recovering ?
            if (_recoveryManager?.Elapsed == true)
            {
                _recoveryManager = null;
            }

            if (_recoveryManager == null)
            {
                double cumuledLifePoints = Engine.Default.CurrentScreen.HitByAnActionnedItem(this);

                cumuledLifePoints += ComputeLifePointCostFromEnemies();

                if (cumuledLifePoints.Greater(0))
                {
                    CurrentLifePoints -= cumuledLifePoints;
                    _recoveryManager = _recoveryTime > 0 ? new Elapser(_recoveryTime) : null;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Overridden; Computes the cost, in life points, inflicted by current instance antagonists.
        /// </summary>
        /// <returns>Life points cost.</returns>
        protected abstract double ComputeLifePointCostFromEnemies();

        /// <summary>
        /// Creates an altenation between <see cref="Sprite._render"/> and <see cref="_renderRecovery"/> (or any render alternative).
        /// </summary>
        /// <param name="alternativeRender">Optionnal; alternative <see cref="Renders.Render"/> for <see cref="Sprite._render"/>.</param>
        /// <param name="alternativeRecoveryRender">Optionnal; alternative <see cref="Renders.Render"/> for <see cref="_renderRecovery"/>.</param>
        /// <returns>Current <see cref="Renders.Render"/>.</returns>
        protected Render RecoveryRenderSwitch(Render alternativeRender = null, Render alternativeRecoveryRender = null)
        {
            alternativeRender = alternativeRender ?? _render;
            alternativeRecoveryRender = alternativeRecoveryRender ?? _renderRecovery;
            return IsRecovering ?
                ((_recoveryManager.ElapsedMilliseconds / 100) % 2 == 0 ?
                    alternativeRender : alternativeRecoveryRender)
                : alternativeRender;
        }
    }
}
