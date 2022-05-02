using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public class ExplosionSpawner : MonoBehaviour
    {
        #region Inspector
        [Tooltip("Explosion prefab")]
        public Explosion explosion;
        #endregion

        public void SpawnExplosion(float damage)
        {
            Explosion e = PrefabPooler.Instance.Spawn(explosion, transform.position, Quaternion.identity, this.transform.parent);
            e.Init(damage);
        }

        public void SpawnExplosion(Projectile projectile)
        {
            SpawnExplosion(projectile.damage);
        }
    }
}
