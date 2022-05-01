using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class GameEvents
    {
        public static event Action<DamageEvent> onDamage;
        public static event Action<DamageEvent> onDamaged;
        public static event Action<DeathEvent> onDie;
        public static event Action<DeathEvent> onDied;

        internal static void InvokeOnDamage(DamageEvent e)
        {
            onDamage?.Invoke(e);
        }

        internal static void InvokeOnDamaged(DamageEvent e)
        {
            onDamaged?.Invoke(e);
        }

        internal static void InvokeOnDie(DeathEvent e)
        {
            onDie?.Invoke(e);
        }

        internal static void InvokeOnDied(DeathEvent e)
        {
            onDied?.Invoke(e);
        }
    }

    public class DamageEvent
    {
        /// <summary>
        /// Base damage
        /// </summary>
        public float damage { get; private set; }
        // IMPROVEMENT: Implement IDamageSource
        /// <summary>
        /// Entity causing the damage. Can be used to distinguish projectiles.
        /// </summary>
        public GameObject source;
        /// <summary>
        /// Health component of the target taking damage.
        /// </summary>
        public Health target;
        /// <summary>
        /// Final damage inflicted to target. Available after damaging the target.
        /// </summary>
        public float damageDealt = 0;

        // Seal constructor
        private DamageEvent() {}

        private static Queue<DamageEvent> pool = new Queue<DamageEvent>(4);

        /// <summary>
        /// Returns instance of damage event
        /// </summary>
        public static DamageEvent GetInstance(float baseDamage, GameObject source, Health target)
        {
            DamageEvent e;
            if (pool.Count != 0)
            {
                // Get element from pool
                e = pool.Dequeue();
            }
            else
            {
                // Create new instance if queue is empty
                e = new DamageEvent();
            }

            // Initialize parameters
            e.damage = baseDamage;
            e.source = source;
            e.target = target;

            return e;
        }

        public static void ReturnInstance(DamageEvent e)
        {
            // Reset fields and return to queue
            e.damageDealt = 0;
            e.source = null;
            e.target = null;
            pool.Enqueue(e);
        }
    }

    public class DeathEvent
    {
        /// <summary>
        /// Prevents death when set to true. It must be checked by all scripts handling OnDie.
        /// </summary>
        public bool prevented = false;
        /// <summary>
        /// Dying entity.
        /// </summary>
        public Health target;
        /// <summary>
        /// Damage event causing death. Can be null when not killed by damage.
        /// </summary>
        public DamageEvent damageEvent;

        // Seal constructor
        private DeathEvent() {}

        private static Queue<DeathEvent> pool = new Queue<DeathEvent>(4);

        /// <summary>
        /// Returns instance of death event
        /// </summary>
        public static DeathEvent GetInstance(Health target, DamageEvent damageEvent = null)
        {
            DeathEvent e;
            if (pool.Count != 0)
            {
                // Get element from pool
                e = pool.Dequeue();
            }
            else
            {
                // Create new instance if queue is empty
                e = new DeathEvent();
            }

            // Initialize parameters
            e.target = target;
            e.damageEvent = damageEvent;

            return e;
        }

        public static void ReturnInstance(DeathEvent e)
        {
            // Reset fields and return to queue
            e.prevented = false;
            e.target = null;
            e.damageEvent = null;
            pool.Enqueue(e);
        }
    }

    public class HealEvent
    {
        /// <summary>
        /// Target health
        /// </summary>
        public Health target;
        /// <summary>
        /// Amount of hp to be regenerated
        /// </summary>
        public float amount;
        /// <summary>
        /// Final amount of restored HP. Available after healing the target.
        /// </summary>
        public float restoredHealth;

        // Seal constructor
        private HealEvent() {}

        private static Queue<HealEvent> pool = new Queue<HealEvent>(4);

        /// <summary>
        /// Returns instance of heal event
        /// </summary>
        public static HealEvent GetInstance(Health target, float amount)
        {
            HealEvent e;
            if (pool.Count != 0)
            {
                // Get element from pool
                e = pool.Dequeue();
            }
            else
            {
                // Create new instance if queue is empty
                e = new HealEvent();
            }

            // Initialize parameters
            e.target = target;
            e.amount = amount;
            return e;
        }

        public static void ReturnInstance(HealEvent e)
        {
            // Reset fields and return to queue
            e.target = null;
            e.restoredHealth = 0f;
            pool.Enqueue(e);
        }
    }
}
