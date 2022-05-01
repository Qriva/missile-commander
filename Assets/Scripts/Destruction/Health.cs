using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [DisallowMultipleComponent]
    public class Health : MonoBehaviour
    {
        [ReadOnly(ReadOnlyAttribute.Mode.EditorOnly)]
        [Tooltip("Current health")]
        [SerializeField] private float health = 100f;
        [SerializeField] private float maxHealth = 100f;

        public event System.Action<DamageEvent> OnDamaged;
        public event System.Action<DeathEvent> OnDeath;
        public event System.Action<HealEvent> OnHealed;
        // TODO: Make method to register hitboxes
        [System.NonSerialized]
        public List<Hitbox> hitboxes = new List<Hitbox>();

        public bool IsDead
        {
            get { return health <= 0f; }
        }

        public float CurrentHealth
        {
            get { return health; }
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            if (Application.isPlaying)
            {
                // Do not allow killing from inspector
                health = Mathf.Clamp(health, 1f, maxHealth);
            }
            else
            {
                // Ensure ther is at least 1 HP
                maxHealth = Mathf.Max(1f, maxHealth);
                // In editor HP and armor should be synchronized
                health = maxHealth;
            }
        }
#endif

        public void TakeDamage(DamageEvent damageEvent)
        {
            // Do not do anything when already dead
            if (IsDead)
            {
                return;
            }

            // Trigger global damage event and allow other scripts to modify event
            GameEvents.InvokeOnDamage(damageEvent);

            // Values below 0 and higher than current hp are not allowed
            float dmg = damageEvent.damageDealt = Mathf.Clamp(damageEvent.damage, 0f, health);
            // Reduce HP
            health -= dmg;

            // Local damage event
            OnDamaged?.Invoke(damageEvent);
            // Trigger global damaged event
            GameEvents.InvokeOnDamaged(damageEvent);
            
            if (IsDead)
            {
                // Trigger death with reused event
                DeathEvent deathEvent = DeathEvent.GetInstance(this, damageEvent);
                Kill(deathEvent);
                DeathEvent.ReturnInstance(deathEvent);
            }
        }

        public void Kill(DeathEvent deathEvent)
        {
            // Global death event
            GameEvents.InvokeOnDie(deathEvent);
            // Action should be handled by preventing script and rest must be skipped
            if (deathEvent.prevented)
            {
                return;
            }

            health = 0;
            // Trigger local event
            OnDeath?.Invoke(deathEvent);
            // Trigger global event
            GameEvents.InvokeOnDied(deathEvent);
            this.enabled = false;
        }

        public void RestoreHealth(HealEvent healEvent)
        {
            healEvent.restoredHealth = Mathf.Clamp(healEvent.amount, 0f, maxHealth - health);
            health += healEvent.restoredHealth;
            // Trigger local event
            OnHealed?.Invoke(healEvent);
        }

        public float NormalizedHealth()
        {
            return health / maxHealth;
        }
    }
}
