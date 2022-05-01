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
        private Transform crosshair;
        private Vector2 crosshairScreenPosition;
        private Vector2 crosshairMoveDelta;
        // FIXME: Temporary
        public ProjectileSpawner launcher;

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
            // TODO: cache plane or retrive from other component
            Plane plane = new Plane(Vector3.forward, Vector3.zero);
            // Update crosshair position
            if (plane.Raycast(ray, out float distance))
            {
                // NOTE: Target point must be updated early, so other systems don't lag
                crosshair.transform.position = ray.GetPoint(distance);
            }
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
                launcher.Fire(crosshair.position);
            }
            else if (context.canceled)
            {
                // Debug.Log("Fire canceled");
            }
        }
    }
}
