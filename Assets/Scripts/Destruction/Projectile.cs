using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour, IPoolable
    {
        [System.Serializable]
        public class ExpireEvent : UnityEvent<Projectile> {}
        
        #region Inspector
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private Collider2D _collider;
        public float damage = 100f;
        [Tooltip("Lifetime in seconds")]
        [SerializeField] private float lifetime = float.PositiveInfinity;
        [Tooltip("Interaction mask determines what objects can be hit")]
        [SerializeField] private InteractionMask mask;
        [SerializeField] private bool delayDestruction = false;
        [SerializeField] private Yields.Instruction delay = Yields.Instruction.WaitForOneSecond;
        [Space]
        public ExpireEvent onExpire;
        #endregion

        private float age;
        private int poolId;
        private bool hasPooler = false;
        public Vector2 velocity => _rigidbody.velocity;
        public float speed => _rigidbody.velocity.magnitude;

        private void Reset()
        {
            // Default setup
            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.gravityScale = 0f;
            _collider = GetComponent<Collider2D>();
            _collider.isTrigger = true;
        }

        private void OnEnable()
        {
            // _collider.enabled = true;
            _rigidbody.simulated = true;
            // _rigidbody.isKinematic = false;
        }

        private void OnDisable()
        {
            // _collider.enabled = false;
            // Ignore collisions and simulation
            _rigidbody.simulated = false;
            // _rigidbody.isKinematic = true;
        }

        protected void Init(float speed)
        {
            // TODO: Is Interaction mask needed at all?
            Debug.Assert(mask != null, "Interaction mask is null", this.gameObject);
            // Set velocity along direction
            _rigidbody.velocity = transform.rotation * new Vector3(0f, speed, 0f);
        }

        /// <summary>
        /// Init must be called after spawn to correctly finish initialization
        /// </summary>
        public void Init(float speed, float lifetime, float damage)
        {
            Init(speed);
            this.damage = damage;
            this.lifetime = lifetime;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Quit if object is not considered as hit
            if (!mask.Contains(other.gameObject.layer))
            {
                return;
            }

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
            // Something was hit, kill projectile
            Expire();
        }

        /// <summary>
        /// Expires (kills) projectile
        /// </summary>
        public void Expire()
        {
            if (this.enabled)
            {
                // Trigger expire event
                onExpire.Invoke(this);
                // TODO: Play Sound & VFX
                this.enabled = false;
                // Schedule release/destruction
                if (delayDestruction)
                {
                    StartCoroutine(ReleaseProjectileCoroutine(Yields.Get(delay)));
                }
                else
                {
                    // Return projectile to pool or destroy
                    PrefabPooler.Instance.Release(this, true);
                }
            }
        }

        private IEnumerator ReleaseProjectileCoroutine(YieldInstruction d)
        {
            // IMPORTANT: Release after level unload or scene change probably may cause error 
            yield return d;
            // Return projectile to pool or destroy
            PrefabPooler.Instance.Release(this, true);
        }

        private void FixedUpdate()
        {
            // Projectile velocity cannot change on the fly
            // For this reason maximum range is calculated based on lifetime
            // However it could be based on traveled distance:
            // distanceTraveled += _rigidbody.velocity.magnitude * Time.deltaTime;
            if (age >= lifetime)
            {
                // Kill projectile
                Expire();
            }
            age += Time.deltaTime;
        }

        public Component AsComponent()
        {
            return this;
        }

        public void Restore(IPoolable original)
        {
            this.age = 0f;
            this._rigidbody.velocity = Vector2.zero;
            this.enabled = true;
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
