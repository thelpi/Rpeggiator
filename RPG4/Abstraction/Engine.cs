using RPG4.Abstraction.Sprites;
using System.Collections.Generic;
using System.Linq;

namespace RPG4.Abstraction
{
    /// <summary>
    /// Game engine.
    /// </summary>
    public class Engine
    {
        /// <summary>
        /// <see cref="Sprites.Player"/>
        /// </summary>
        public Player Player { get; private set; }
        /// <summary>
        /// Inferred; current screen identifier.
        /// </summary>
        public int CurrentScreenId { get { return CurrentScreen.Id; } }
        /// <summary>
        /// Current <see cref="Screen"/> (where's the player).
        /// </summary>
        public Screen CurrentScreen { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="screenId"><see cref="CurrentScreen"/> identifier.</param>
        public Engine(int screenId)
        {
            Player = new Player();
            CurrentScreen = Screen.GetScreen(screenId);
        }

        /// <summary>
        /// Refresh the status of every components at new frame.
        /// </summary>
        /// <param name="keys"><see cref="KeyPress"/></param>
        public void CheckEngineAtNewFrame(KeyPress keys)
        {
            Player.BehaviorAtNewFrame(this, keys);
            Player.CheckIfHasBeenHit(this);
            CollectPickableItems();
            CheckInventoryUse(keys);

            CurrentScreen.BehaviorAtNewFrame(this);

            if (Player.NewScreenEntrance.HasValue)
            {
                CurrentScreen = CurrentScreen.GetNextScreenFromDirection(Player.NewScreenEntrance.Value);
            }
            else
            {
                Pit pit = CurrentScreen.Pits.FirstOrDefault(p => p.ScreenIndexEntrance.HasValue && p.CanFallIn(Player));
                if (pit != null)
                {
                    CurrentScreen = Screen.GetScreen(pit.ScreenIndexEntrance.Value);
                }
            }
        }

        // Checks inventory use.
        private void CheckInventoryUse(KeyPress keys)
        {
            if (keys.InventorySlotId.HasValue)
            {
                var itemDropped = Player.Inventory.UseItem(this, keys.InventorySlotId.Value);
                if (itemDropped != null)
                {
                    CurrentScreen.AddDroppedItem(itemDropped);
                }
            }
        }

        // Checks for items to pick on the current position.
        private void CollectPickableItems()
        {
            var items = CurrentScreen.PickableItems.Where(it => it.Overlap(Player)).ToList();
            foreach (var item in items)
            {
                item.Pick(this);
            }
            CurrentScreen.CheckPickableItemsQuantities();
        }

        /// <summary>
        /// Checks if the specified <see cref="FloorTrigger"/> has been triggered by the environnement.
        /// </summary>
        /// <param name="trigger"><see cref="FloorTrigger"/></param>
        /// <returns><c>True</c> if triggered; <c>False</c> otherwise.</returns>
        public bool IsTriggered(FloorTrigger trigger)
        {
            return trigger.Overlap(Player) || CurrentScreen.Enemies.Any(e => trigger.Overlap(e));
        }

        /// <summary>
        /// Checks hit(s) made by enemies on player.
        /// </summary>
        /// <returns>Potential cost in life points.</returns>
        public double CheckHitByEnemiesOnPlayer()
        {
            // checks hit (for each enemy, life points lost is cumulable)
            double cumuledLifePoints = 0;
            foreach (var enemy in CurrentScreen.Enemies)
            {
                if (Player.Overlap(enemy))
                {
                    cumuledLifePoints += enemy.HitLifePointCost;
                }
            }
            return cumuledLifePoints;
        }
    }
}
