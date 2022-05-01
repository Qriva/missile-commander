using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // Update early to finish reloading before usage
    [DefaultExecutionOrder(-100)]
    public class MissileLauncher : MonoBehaviour
    {
        #region Inspector
        public ProjectileSpawner spawner;
        public Health health;
        [Header("Launcher Settings")]
        [Range(0.02f, 3f)]
        [SerializeField] private float reloadTime = 1f;
        [Range(1, 50)]
        [SerializeField] private int ammoCapacity = 10;
        [ReadOnly]
        [SerializeField] private float currentReload = 0f;
        [ReadOnly]
        [SerializeField] private int currentAmmo = 10;
        #endregion

        // Lanucher is ready when there is any ammo and it's reloaded
        public bool IsReadyToFire => currentAmmo > 0 && currentReload >= reloadTime;
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
    }
}
