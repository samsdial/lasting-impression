using System;
using GuardiansOfTheWetland.Gameplay;
using UnityEngine;

namespace GuardiansOfTheWetland.Core
{
    /// <summary>
    /// Keeps the progress of the current game: collected trash and seeds,
    /// and whether the wetland has already been restored.
    /// Other systems (HUD, restoration) listen to its events instead of polling.
    /// </summary>
    public class GameSession : MonoBehaviour
    {
        /// <summary>Single active session, so collectibles and the finish point can reach it.</summary>
        public static GameSession Instance { get; private set; }

        public int TrashCollected { get; private set; }
        public int TrashTotal { get; private set; }
        public int SeedsCollected { get; private set; }
        public int SeedsTotal { get; private set; }
        public bool IsRestored { get; private set; }

        /// <summary>True once every piece of trash in the level has been picked up.</summary>
        public bool AllTrashCollected => TrashCollected >= TrashTotal;

        /// <summary>Raised whenever a counter or total changes.</summary>
        public event Action CountsChanged;

        /// <summary>Raised once, when the player restores the wetland.</summary>
        public event Action Restored;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            // Clear the static reference so a destroyed session is never used.
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// Stores how many trash pieces and seeds the level contains.
        /// </summary>
        public void SetTotals(int trashTotal, int seedsTotal)
        {
            TrashTotal = trashTotal;
            SeedsTotal = seedsTotal;
            CountsChanged?.Invoke();
        }

        /// <summary>
        /// Adds one item of the given type to its counter.
        /// </summary>
        public void RegisterCollected(CollectibleType type)
        {
            // Each collectible type has its own counter.
            if (type == CollectibleType.Trash)
            {
                TrashCollected++;
            }
            else
            {
                SeedsCollected++;
            }

            CountsChanged?.Invoke();
        }

        /// <summary>
        /// Restores the wetland if every piece of trash was collected.
        /// </summary>
        /// <returns>True if the restoration started now.</returns>
        public bool TryRestore()
        {
            // Restoration happens only once and only with a clean wetland.
            if (IsRestored || !AllTrashCollected)
            {
                return false;
            }

            IsRestored = true;
            Restored?.Invoke();
            return true;
        }
    }
}
