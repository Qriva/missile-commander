using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class Hitbox : MonoBehaviour
    {
        private static HashSet<Health> uniqueHealths = new HashSet<Health>(32);

        #region Inspector
        /// <summary>
        /// Connected health
        /// </summary>
        [SerializeField] private Health health;
        #endregion

        public Collider2D hitboxCollider { get; private set; }
        public Health Health => this.health;

        public void Reset()
        {
            // Default setup
            health = GetComponentInParent<Health>();
        }

        private void Awake()
        {
            hitboxCollider = GetComponent<Collider2D>();
            // Validate and register hitbox
            if (health != null)
            {
                health.hitboxes.Add(this);
            }
            else
            {
                Debug.LogError("Hitbox health is null", this.gameObject);
            }
        }

        public bool isHittable
        {
            get { return this.enabled && !health.IsDead; }
        }

        /// <summary>
        /// Helper method to deal damage to hitbox health
        /// </summary>
        /// <param name="e">Damage event object</param>
        public void Damage(DamageEvent e)
        {
            health.TakeDamage(e);
        }

        /// <summary>
        /// Finds hittable hitboxes filtered by unique owner (health) and fills the list
        /// </summary>
        /// <param name="colliders">Array of colliders to search</param>
        /// <param name="collidersCount">Number of elements in array</param>
        /// <param name="hitboxes">List filled with valid hitboxes</param>
        public static void Find(Collider2D[] colliders, int collidersCount, List<Hitbox> hitboxes)
        {
            Debug.Assert(hitboxes != null, "List of hitboxes is not initialized");
            Debug.Assert(collidersCount <= colliders.Length, "Number of colliders is greater than number of elements in array");

            hitboxes.Clear();
            for (int i = 0; i < collidersCount; i++)
            {
                // Find hittable hitboxes and ensure the same health component is not picked more than once
                if (colliders[i].TryGetComponent<Hitbox>(out Hitbox hitbox) && hitbox.isHittable && uniqueHealths.Add(hitbox.health))
                {
                    hitboxes.Add(hitbox);
                }
            }

            // Clear cache
            uniqueHealths.Clear();
        }

        /// <summary>
        /// Finds hittable hitbox
        /// </summary>
        /// <param name="collider">Collider to search</param>
        /// <param name="hitbox">Hitbox or null when not found</param>
        /// <returns>True when valid hitbox has been found</returns>
        public static bool Find(Collider2D collider, out Hitbox hitbox)
        {
            if (collider.TryGetComponent<Hitbox>(out Hitbox h) && h.isHittable)
            {
                hitbox = h;
                return true;
            }
            hitbox = null;
            return false;
        }
    }
}
