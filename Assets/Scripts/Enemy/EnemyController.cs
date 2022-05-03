using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace Game
{
    [DisallowMultipleComponent]
    public class EnemyController : MonoBehaviour
    {
        public ProjectileSpawner projectileSpawner;
        [SerializeField] private TextMeshProUGUI waveUI;
        [Header("Spawn setup")]
        [SerializeField] private int projectilesPerWave = 8;
        [SerializeField] private int additionalProjectilesPerWave = 1;
        [SerializeField] private float speedIncreasePerWave = 0.2f;
        [Space]
        [SerializeField] private Enemy[] enemies;
        [Space]
        public UnityEvent onWaveEnd;
        private Rect viewport;
        private int waveIndex = 0;
        private Coroutine waveCoroutine = null;
        private List<Enemy> availableEnemies = new List<Enemy>(4);

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
            // Update list with available projectiles
            UpdateAvailableEnemies();
            // Sanity check
            Debug.Assert(availableEnemies.Count > 0, "There are no available enemies.", this);

            // Number of projectiles spawned this wave
            int projecilesCount = projectilesPerWave + waveIndex * additionalProjectilesPerWave;
            // Spawn projectiles every 500ms
            for (int i = 0; i < projecilesCount; i++)
            {
                yield return Yields.WaitForHalfSecond;
                Enemy enemy = availableEnemies[Random.Range(0, availableEnemies.Count)];

                // Calculate final Speed
                float speed = enemy.speed + waveIndex * speedIncreasePerWave;
                // Find random start and end point
                float randomSpawnX = Random.Range(viewport.xMin, viewport.xMax);
                float randomTargetX = Random.Range(viewport.xMin, viewport.xMax);

                // Set random projectile from list
                projectileSpawner.projectile = enemy.projectile;
                projectileSpawner.speed = speed;
                // Spawn projectile
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

        private void UpdateAvailableEnemies()
        {
            availableEnemies.Clear();
            for (int i = 0; i < enemies.Length; i++)
            {
                // Get enemy if wave is high enough
                Enemy enemy = enemies[i];
                if (enemy.minWave <= waveIndex)
                {
                    availableEnemies.Add(enemy);
                }
            }
        }

        public void StartNextWave()
        {
            // Icrement wave index
            waveIndex += 1;
            waveUI.text = (waveIndex + 1).ToString();
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
            waveUI.text = "1";
        }
    }

    [System.Serializable]
    public class Enemy
    {
        [Tooltip("Minimum wave to spawn this type of projectiles")]
        public int minWave = 0;
        public Projectile projectile;
        public float speed;
    }
}