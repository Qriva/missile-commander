using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Explosion : MonoBehaviour, IPoolable
    {
        [SerializeField] private AnimationCurve scaleOverLife;
        [SerializeField] private float size = 1f;
        [SerializeField] private float damage = 10f;
        [Tooltip("Lifetime in seconds")]
        [Range(0.1f, 5f)]
        [SerializeField] private float duration = 1f;
        [HideInInspector]
        [SerializeField] private float inversedDuration = 1f;

        private bool hasPooler = false;
        private int poolId;
        private float age = 0f;

        private void Reset()
        {
            // Default setup
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.isKinematic = true;
            Collider2D col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        private void OnValidate()
        {
            // Cache inversed duration, to avoid division every frame
            inversedDuration = 1f / duration;
        }

        private void Update()
        {
            // Sample curve to get current size
            float currentScale = scaleOverLife.Evaluate(age * inversedDuration) * size;
            // It is ok to resize some colliders and there should not be too many explosions
            // https://forum.unity.com/threads/dynamically-changin-size-of-colliders.581254/
            transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            // Kill explosion after given time
            if (age >= duration)
            {
                Expire();
            }
            age += Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Try to find hitbox to deal damage
            if (Hitbox.Find(other, out Hitbox hitbox))
            {
                // Reuse damage event
                DamageEvent damageEvent = DamageEvent.GetInstance(damage, this.gameObject, hitbox.Health);
                // Deal damage to hitbox
                hitbox.Damage(damageEvent);
                // Return damage event
                DamageEvent.ReturnInstance(damageEvent);
            }
        }

        private void Expire()
        {
            if (this.enabled)
            {
                this.enabled = false;
                // Return explosion to pool or destroy
                PrefabPooler.Instance.Release(this, true);
            }
        }

        /// <summary>
        /// Init must be called after spawn to correctly finish initialization
        /// </summary>
        public void Init(float damage)
        {
            this.damage = damage;
            // Reenable on start
            this.enabled = true;
        }

        public Component AsComponent()
        {
            return this;
        }

        public void Restore(IPoolable original)
        {
            this.age = 0;
        }

        public void SetPoolerId(int id)
        {
            this.poolId = id;
            hasPooler = true;
        }

        public bool TryGetPoolerId(out int id)
        {
            id = poolId;
            return hasPooler;
        }
    }
}
