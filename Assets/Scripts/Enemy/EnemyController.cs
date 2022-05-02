using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    [DisallowMultipleComponent]
    public class EnemyController : MonoBehaviour
    {
        public ProjectileSpawner projectileSpawner;
        [Space]
        public UnityEvent onWaveEnd;
        private Rect viewport;
        private int waveIndex = 0;
        Coroutine waveCoroutine = null;

        private void Awake()
        {
            // Enemy controller must not have any children
            // Children of this component are used to determine if wave ended
            if (transform.childCount > 0)
            {
                Debug.LogError("EnemyController child count must be 0.", this);
            }
        }

        IEnumerator WaveSequence()
        {
            for (int i = 0; i < 10; i++)
            {
                float randomSpawnX = Random.Range(viewport.xMin, viewport.xMax);
                float randomTargetX = Random.Range(viewport.xMin, viewport.xMax);
                yield return Yields.WaitForHalfSecond;
                projectileSpawner.Fire(
                    new Vector3(randomTargetX, viewport.yMin, 0f), 
                    new Vector3(randomSpawnX, viewport.yMax, 0f),
                    this.transform
                );
            }
            // Spawning finished, now wait for death of all objects
            while (this.transform.childCount > 0)
            {
                yield return Yields.WaitForFixedUpdate;
            }
            // Wait additional second just in case
            yield return Yields.WaitForOneSecond;
            // Clear coroutine
            this.waveCoroutine = null;
            // Trigger event
            onWaveEnd.Invoke();
        }

        public void StartNextWave()
        {
            // Icrement wave index
            waveIndex += 1;
            StartWave();
        }

        public void StopCurrentWave()
        {
            // If there is wave running right now then stop (interrupt) it
            if (waveCoroutine != null)
            {
                StopCoroutine(waveCoroutine);
                waveCoroutine = null;
            }
        }

        public void StartWave()
        {
            waveCoroutine = StartCoroutine(WaveSequence());
        }

        internal void SetViewport(Rect viewport)
        {
            this.viewport = viewport;
        }

        /// <summary>
        /// Removes all children of transform (current enemies)
        /// </summary>
        internal void ClearEnemies()
        {
            foreach (Transform child in this.transform)
            {
                Destroy(child.gameObject);
            }
        }

        internal void ResetWaves()
        {
            waveIndex = 0;
        }
    }
}