using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Upgrades;
using System;

namespace Game
{
    // Update early to finish reloading before usage
    [DefaultExecutionOrder(-100)]
    public class MissileLauncher : Building
    {
        #region Inspector
        public ProjectileSpawner spawner;
        [Header("Launcher Settings")]
        [Range(0.02f, 3f)]
        public float reloadTime = 1f;
        [Range(1, 50)]
        public int ammoCapacity = 10;
        [ReadOnly]
        [SerializeField] private float currentReload = 0f;
        [ReadOnly]
        [SerializeField] private int currentAmmo = 10;
        #endregion

        // IMPROVEMENT: Create custom inspector with list of upgrades for debugging
        private List<MissileLauncherUpgrade> upgrades = new List<MissileLauncherUpgrade>(8);
        // Lanucher is ready when there is any ammo and it's reloaded
        public bool IsReadyToFire => currentAmmo > 0 && currentReload >= reloadTime;
        public int MissingAmmo => ammoCapacity - currentAmmo;
        
        /// <summary>
        /// World space position of projectile spawn point
        /// </summary>
        public Vector3 SpawnPosition => spawner.SpawnPosition;

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                currentAmmo = ammoCapacity;
            }
        }

        void Update()
        {
            if (currentReload < reloadTime)
            {
                currentReload += Time.deltaTime;
            }
        }

        /// <summary>
        /// Tries to fire missile towards target position
        /// </summary>
        /// <returns>False when ammo is equal to 0 or launcher is not reloaded, otherwise true</returns>
        public bool Fire(Vector3 target)
        {
            if (this.IsReadyToFire)
            {
                // Spawn projectile
                spawner.Fire(target);
                // Reset reload and reduce ammo
                currentReload = 0f;
                currentAmmo -= 1;
                return true;
            }
            return false;
        }

        public void AddUpgrade(MissileLauncherUpgrade upgrade)
        {
            this.upgrades.Add(upgrade);
            upgrade.OnAttach(this);
        }

        public void RemoveUpgrade(MissileLauncherUpgrade upgrade)
        {
            upgrade.OnDettach(this);
            this.upgrades.Remove(upgrade);
        }

        /// <summary>
        /// Refills ammunition
        /// </summary>
        /// <param name="refillLimit">Maximum number of refilled ammo</param>
        /// <returns>final amount of refilled ammo</returns>
        internal int RefillAmmo(int refillLimit)
        {
            int amount = Mathf.Min(refillLimit, MissingAmmo);
            currentAmmo += amount;
            return amount;
        }
    }
}
