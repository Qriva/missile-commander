using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public class GameplayController : MonoBehaviour
    {
        [Header("Game scene settings")]
        [Tooltip("Game view bounds")]
        [SerializeField] private Rect viewport;
        [SerializeField] private float cameraZDistance = -10f;
        public Camera mainCamera;
        [Header("Other")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private EnemyController enemyController;
        [SerializeField] private PlayerInitialSetup initialSetup;

        private void Awake()
        {
            // First adjust game camera
            AdjustCameraSize();

            // Initialize player controller
            playerController.SetGamePlane(new Plane(this.transform.forward, viewport.center));

            // Set viewport in enemy controller
            enemyController.SetViewport(this.viewport);
        }

#if UNITY_EDITOR
        private void Update()
        {
            // Adjust camera size in editor
            AdjustCameraSize();
        }
#endif

        private void AdjustCameraSize()
        {
            // Center camera
            mainCamera.transform.position = new Vector3(viewport.center.x, viewport.center.y, cameraZDistance);
            // Adjust size
            float screenRatio = (float)Screen.width / (float)Screen.height;
            float targetRatio = viewport.width / viewport.height;
            if(screenRatio >= targetRatio)
            {
                Camera.main.orthographicSize = viewport.height / 2f;
            }
            else
            {
                float difference = targetRatio / screenRatio;
                Camera.main.orthographicSize = viewport.height / 2f * difference;
            }
            // Vector3 min = mainCamera.WorldToViewportPoint(viewport.min);
            // Vector3 max = mainCamera.WorldToViewportPoint(viewport.max);
            // mainCamera.rect = new Rect(min.x, min.y, max.x - min.y, max.y - min.y);
        }

        public void BeginNewGame()
        {
            // Reset money to 0
            playerController.ResetFunds();
            // Reset waves and buildings
            enemyController.ResetWaves();
            // playerController.Reset
            enemyController.ClearEnemies();
            // Setup default buildings
            initialSetup.InitializeBuildings();
            enemyController.StartWave();
        }

        public void BeginNextWave()
        {
            enemyController.StartNextWave();
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(viewport.center, new Vector3(viewport.width, viewport.height, 0.01f));
        }
    }
}
