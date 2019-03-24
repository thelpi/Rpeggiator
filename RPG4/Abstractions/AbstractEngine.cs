﻿using System.Collections.Generic;
using System.Linq;

namespace RPG4.Abstractions
{
    /// <summary>
    /// Game engine.
    /// </summary>
    public class AbstractEngine
    {
        private Dictionary<Directions, int> _adjacentScreens;
        private List<Sprite> _walls;
        private List<Gate> _gates;
        private List<Enemy> _enemies;
        private List<GateTrigger> _gateTriggers;
        private List<FloorItem> _items;
        private List<Sprite> _droppedItems;

        // Shortcuts access to dropped bombs.
        private IEnumerable<Bomb> _bombs { get { return _droppedItems.Where(di => di is Bomb).Cast<Bomb>(); } }

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
        /// List of walls.
        /// </summary>
        public IReadOnlyCollection<Sprite> Walls { get { return _walls; } }
        /// <summary>
        /// Inferred; list of <see cref="Sprite"/> which can't be crossed.
        /// </summary>
        public IReadOnlyCollection<Sprite> SolidStructures { get { return _walls.Concat(_gates.Where(g => g.Activated)).ToList(); } }
        /// <summary>
        /// List of every <see cref="Sprite"/> which requires a display management at each frame.
        /// </summary>
        /// <remarks>Doesn't include <see cref="Walls"/>.</remarks>
        public IReadOnlyCollection<Sprite> Sprites
        {
            get
            {
                return _droppedItems.Concat<Sprite>(_items).Concat(_gateTriggers).Concat(_enemies).Concat(_gates.Where(g => g.Activated)).ToList();
            }
        }

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

            _walls = new List<Sprite>();
            _enemies = new List<Enemy>();
            _gateTriggers = new List<GateTrigger>();
            _gates = new List<Gate>();
            _items = new List<FloorItem>();
            _droppedItems = new List<Sprite>();
            AreaWidth = screenJsonDatas.AreaWidth;
            AreaHeight = screenJsonDatas.AreaHeight;

            foreach (dynamic wallJson in screenJsonDatas.Walls)
            {
                _walls.Add(new Sprite(wallJson));
            }
            foreach (dynamic gateJson in screenJsonDatas.Gates)
            {
                _gates.Add(new Gate(gateJson));
            }
            foreach (dynamic enemyJson in screenJsonDatas.Enemies)
            {
                _enemies.Add(new Enemy(enemyJson));
            }
            foreach (dynamic gatetriggerJson in screenJsonDatas.GateTriggers)
            {
                _gateTriggers.Add(new GateTrigger(gatetriggerJson));
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
        /// Refresh the status of every components at new frame.
        /// </summary>
        /// <param name="keys"><see cref="KeyPress"/></param>
        public void CheckEngineAtNewFrame(KeyPress keys)
        {
            Player.BehaviorAtNewFrame(this, keys);

            // checks for items on the new position
            var items = _items.Where(it => it.Overlap(Player)).ToList();
            foreach (var item in items)
            {
                if (Player.Inventory.TryAdd(item.ItemId, item.Quantity))
                {
                    _items.Remove(item);
                }
            }

            // check inventory use
            if (keys.InventorySlotId.HasValue)
            {
                var itemDropped = Player.Inventory.UseItem(this, keys.InventorySlotId.Value);
                if (itemDropped != null)
                {
                    _droppedItems.Add(itemDropped);
                }
            }
            
            // enemies management must be done after player management
            foreach (var enemy in _enemies)
            {
                enemy.BehaviorAtNewFrame(this, null);
            }
            _enemies.RemoveAll(e => e.CheckDeath(this));
            
            Player.CheckIfHasBeenHit(this);

            foreach (var gt in _gateTriggers)
            {
                gt.BehaviorAtNewFrame(this, keys);
            }
            foreach (var g in _gates)
            {
                g.BehaviorAtNewFrame(this, keys);
            }

            // bombs disappear when they overlap a structure
            //_droppedItems.RemoveAll(b => (b is Bomb) &&  (!(b as Bomb).IsPending && !(b as Bomb).ExplosionHalo.Active) || SolidStructures.Any(cw => cw.Overlap(b)));
            var bombsToRemove = _bombs.Where(b => (!b.IsPending && !b.ExplosionHalo.Active) || SolidStructures.Any(cw => cw.Overlap(b))).Cast<Sprite>().ToList();
            _droppedItems.RemoveAll(di => bombsToRemove.Contains(di));

            foreach (var di in _droppedItems)
            {
                di.BehaviorAtNewFrame(this);
            }

            if (Player.NewScreenEntrance.HasValue)
            {
                SetEnginePropertiesFromScreenDatas(_adjacentScreens[Player.NewScreenEntrance.Value]);
            }
        }

        /// <summary>
        /// Gets a list of <see cref="GateTrigger"/> associated to the specified <see cref="Gate"/>.
        /// </summary>
        /// <param name="gate"><see cref="Gate"/></param>
        /// <returns>List of <see cref="GateTrigger"/>.</returns>
        public IReadOnlyCollection<GateTrigger> GetTriggersForSpecifiedGate(Gate gate)
        {
            return _gateTriggers.Where(gt => gt.GateIndex == _gates.IndexOf(gate) && gt.IsActivated).ToList();
        }

        /// <summary>
        /// Checks if the specified <see cref="FloorTrigger"/> has been triggered by the environnement.
        /// </summary>
        /// <param name="trigger"><see cref="FloorTrigger"/></param>
        /// <returns><c>True</c> if triggered; <c>False</c> otherwise.</returns>
        public bool IsTriggered(FloorTrigger trigger)
        {
            return trigger.Overlap(Player) || _enemies.Any(e => trigger.Overlap(e));
        }

        /// <summary>
        /// Checks hit(s) made by enemies on player.
        /// </summary>
        /// <returns>Potential cost in life points.</returns>
        public int CheckHitByEnemiesOnPlayer()
        {
            // checks hit (for each enemy, life points lost is cumulable)
            int cumuledLifePoints = 0;
            foreach (var enemy in _enemies)
            {
                if (Player.Overlap(enemy))
                {
                    cumuledLifePoints += enemy.HitLifePointCost;
                }
            }
            return cumuledLifePoints;
        }

        /// <summary>
        /// Checks if a bomb (or several) is currently exploding near to a <see cref="LifeSprite"/>.
        /// </summary>
        /// <param name="sprite"><see cref="LifeSprite"/></param>
        /// <returns>Life points lost.</returns>
        public int OverlapAnExplodingBomb(LifeSprite sprite)
        {
            return _bombs.Sum(b => b.ExplosionHalo.Active && b.ExplosionHalo.Overlap(sprite) ? (
                sprite is Player ? Bomb.PLAYER_LIFE_POINT_COST : Bomb.ENEMY_LIFE_POINT_COST
            ) : 0);
        }
    }
}
