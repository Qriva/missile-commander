using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [SelectionBase]
    public class ProjectileSpawner : MonoBehaviour
    {
        #region Inspector
        [Tooltip("Projectile prefab spawned on fire")]
        public Projectile projectile;
        [Tooltip("Missile projectile spawn point in local space")]
        public Vector2 spawnPoint;
        [Header("Projectile Settings")]
        [Range(0.1f, 30f)]
        public float speed = 10f;
        public float damage = 10f;
        #endregion

        /// <summary>
        /// Fire projectile towards target position
        /// </summary>
        public void Fire(Vector3 targetPosition)
        {
            // Calculate position and rotation of projectile
            Vector3 position = transform.TransformPoint(spawnPoint);
            Vector3 direction = targetPosition - position;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, direction);
            // Projectile should expire in target position, so calculate lifetime from speed and distance
            float lifetime = direction.magnitude / speed;

            // IMPROVEMENT: It might be good idea to set current (parent) scene for projectile
            // Spawn and initialize projectile
            Projectile p = PrefabPooler.Instance.Spawn(projectile, position, rotation, null);
            p.Init(speed, lifetime, damage);
        }

        private void OnDrawGizmosSelected()
        {
            // Spawn point gizmo
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.TransformPoint(spawnPoint), 0.1f);
        }
    }
}
