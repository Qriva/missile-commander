using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    [SelectionBase]
    [DisallowMultipleComponent]
    // Run this script before other updates, because it updates crosshair
    [DefaultExecutionOrder(-50)]
    public class PlayerController : MonoBehaviour, PlayerControls.IPlayerActions
    {
        [SerializeField] private Transform crosshairPrefab;
        [Tooltip("Game main camera")]
        [SerializeField] private Camera cam;
        [Tooltip("Missile launchers under control of player")]
        [SerializeField] private List<MissileLauncher> missileLaunchers;
        private Transform crosshair;
        private Vector2 crosshairScreenPosition;
        private Vector2 crosshairMoveDelta;
        private Plane gamePlane = new Plane(Vector3.forward, Vector3.zero);

        /// <summary>
        /// True when fire action should be performed
        /// </summary>
        private bool fireEnabled = false;

        private void Awake()
        {
            // Initialize events of initial launchers
            for (int i = 0; i < missileLaunchers.Count; i++)
            {
                missileLaunchers[i].health.OnDeath += HandleMissileLauncherDestruction;
            }
        }

        void OnEnable()
        {
            // Create reticle if not initialized
            if (crosshair == null)
            {
                crosshair = Instantiate(crosshairPrefab);
            }
        }

        void OnDisable()
        {
            if (crosshair != null)
            {
                Destroy(crosshair);
                crosshair = null;
            }
        }

        private void Update()
        {
            // IMPROVEMENT: VirtualMouse can be used instead of crosshairMoveDelta
            // Update crosshair position for both mouse and gamepad
            // Clamp values to keep crosshair inside screen bounds when gamepad is used
            crosshairScreenPosition = new Vector2(
                Mathf.Clamp(crosshairScreenPosition.x + crosshairMoveDelta.x, 0f, Screen.width), 
                Mathf.Clamp(crosshairScreenPosition.y + crosshairMoveDelta.y, 0f, Screen.height)
            );

            // Transform screen position to world position
            // Camera.ScreenToWorldPoint could be used here, but it might work wrong if camera is set Perspective
            // Casting ray on plane should return always correct values even if camera is tilted
            Ray ray = cam.ScreenPointToRay(crosshairScreenPosition);
            // Update crosshair position
            if (gamePlane.Raycast(ray, out float distance))
            {
                // NOTE: Target point must be updated early, so other systems don't lag
                crosshair.transform.position = ray.GetPoint(distance);
            }

            // Shot missile if fire action is enabled and there are any launchers
            if (fireEnabled && missileLaunchers.Count > 0)
            {
                // Find the closest and ready silo to target position
                MissileLauncher launcher = FindLauncher(crosshair.position);
                if (launcher != null)
                {
                    // Set fire action to false to mark it as "used"
                    // Resolving OnFire in update allows player to fire missile when it's ready
                    // This way gives way better feedback
                    fireEnabled = false;
                    launcher.Fire(crosshair.position);
                }
            }
        }

        internal void SetGamePlane(Plane plane)
        {
            gamePlane = plane;
        }

        /// <summary>
        /// Registers MissileLauncher
        /// </summary>
        public void AddMissileLauncher(MissileLauncher launcher)
        {
            missileLaunchers.Add(launcher);
            launcher.health.OnDeath += HandleMissileLauncherDestruction;
        }

        private void HandleMissileLauncherDestruction(DeathEvent e)
        {
            Health target = e.target;
            // Remove launcher by health
            for (int i = 0; i < missileLaunchers.Count; i++)
            {
                MissileLauncher m = missileLaunchers[i];
                if (m.health == target)
                {
                    // Remove launcher and early exit
                    missileLaunchers.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Tries to find closest ready to fire launcher for given position
        /// </summary>
        /// <returns>MissileLauncher ready to fire or null</returns>
        private MissileLauncher FindLauncher(Vector3 position)
        {
            MissileLauncher closest = null;
            float closestSqrDistance = Mathf.Infinity;
            // Find the closest silo to target position
            for (int i = 0; i < missileLaunchers.Count; i++)
            {
                MissileLauncher missileLauncher = missileLaunchers[i];
                // Skip if not ready
                if (!missileLauncher.IsReadyToFire)
                {
                    continue;
                }
                // Squared magnitude to compare distances
                float sqrMagnitude = (position - missileLauncher.SpawnPosition).sqrMagnitude;
                if (sqrMagnitude < closestSqrDistance)
                {
                    closestSqrDistance = sqrMagnitude;
                    closest = missileLauncher;
                }
            }
            return closest;
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            crosshairMoveDelta = context.ReadValue<Vector2>();
        }

        public void OnMouseAim(InputAction.CallbackContext context)
        {
            crosshairScreenPosition = context.ReadValue<Vector2>();
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                fireEnabled = true;
            }
            else if (context.canceled)
            {
                fireEnabled = false;
            }
        }
    }
}
