using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class Building : MonoBehaviour
    {
        public Health health;
        [Space]
        [Tooltip("Building cost")]
        public int cost = 10;

        protected void Awake()
        {
            if (health == null)
            {
                Debug.LogError("Reference to Health is missing,", this.gameObject);
            }
            else
            {
                // Lambda should be GC along with this component
                health.OnDeath += (DeathEvent e) => {
                    if (this.gameObject != null)
                    {
                        Destroy(this.gameObject);
                    }
                };
            }
        }
    }
}
