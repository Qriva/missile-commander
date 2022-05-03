using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace Game
{
    [SelectionBase]
    [DisallowMultipleComponent]
    // Run this script before other updates, because it updates crosshair
    [DefaultExecutionOrder(-50)]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour, PlayerControls.IPlayerActions
    {
        [SerializeField] private Transform crosshairPrefab;
        [SerializeField] private TextMeshProUGUI fundsUI;
        [Tooltip("Game main camera")]
        [SerializeField] private Camera cam;
        [Tooltip("Missile launchers under control of player")]
        [SerializeField] private List<MissileLauncher> missileLaunchers;
        [SerializeField] private List<MoneyStructure> moneyStructures;
        [ReadOnly]
        [SerializeField] private int funds = 0;
        [Tooltip("Cost of single missile")]
        [Range(1, 10)]
        [SerializeField] private int ammoCost = 1;
        [Header("Input")]
        [SerializeField] private string playerActionMapName = "Player";
        [SerializeField] private string uiActionMapName = "UI";
        [SerializeField] private float gamepadSpeed = 2f;
        [Space]
        public UnityEvent onGameOver;

        private PlayerInput playerInput;
        private Transform crosshair;
        private Vector2 crosshairScreenPosition;
        private Vector2 crosshairMoveDelta;
        private Plane gamePlane = new Plane(Vector3.forward, Vector3.zero);
        /// <summary>
        /// True when fire action should be performed
        /// </summary>
        private bool fireEnabled = false;
        public int Funds => this.funds;

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            // Initialize events of initial launchers and money structures
            for (int i = 0; i < missileLaunchers.Count; i++)
            {
                missileLaunchers[i].health.OnDeath += HandleMissileLauncherDestruction;
            }
            for (int i = 0; i < moneyStructures.Count; i++)
            {
                moneyStructures[i].health.OnDeath += HandleMoneyStructureDestruction;
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

        internal void ResetFunds()
        {
            funds = 0;
            fundsUI.text = "0";
        }

        public void EnablePlayerActionMap()
        {
            playerInput.SwitchCurrentActionMap(playerActionMapName);
        }

        public void EnableUIActionMap()
        {
            playerInput.SwitchCurrentActionMap(uiActionMapName);
        }

        public void GatherFundsFromStructures()
        {
            // Get money from all buildings
            for (int i = 0; i < moneyStructures.Count; i++)
            {
                funds += moneyStructures[i].moneyGeneration;
            }
            fundsUI.text = funds.ToString();
        }

        public void ReduceFunds(int value)
        {
            funds -= value;
            fundsUI.text = funds.ToString();
        }

        /// <summary>
        /// Refills ammunition in all missile launchers
        /// </summary>
        public void RefillLaunchers()
        {
            // Check if there are any launchers
            if (missileLaunchers.Count == 0)
            {
                return;
            }

            // First get total amount of missing ammo
            int missingAmmo = 0;
            for (int i = 0; i < missileLaunchers.Count; i++)
            {
                missingAmmo += missileLaunchers[i].MissingAmmo;
            }

            int refillLimit = missingAmmo;
            // Check if player can afford ammunition
            if (missingAmmo * ammoCost > funds)
            {
                // There is not enough money, so refill launchers evenly
                refillLimit = (funds / ammoCost) / missileLaunchers.Count;
            }
            // Refill ammo and reduce funds
            for (int i = 0; i < missileLaunchers.Count; i++)
            {
                int refilled = missileLaunchers[i].RefillAmmo(refillLimit);
                funds -= refilled * ammoCost;
            }
            fundsUI.text = funds.ToString();
        }

        private void Update()
        {
            // IMPROVEMENT: VirtualMouse can be used instead of crosshairMoveDelta
            // Update crosshair position for both mouse and gamepad
            // Clamp values to keep crosshair inside screen bounds when gamepad is used
            crosshairScreenPosition = new Vector2(
                Mathf.Clamp(crosshairScreenPosition.x + (crosshairMoveDelta.x * gamepadSpeed), 0f, Screen.width), 
                Mathf.Clamp(crosshairScreenPosition.y + (crosshairMoveDelta.y * gamepadSpeed), 0f, Screen.height)
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

        public void AddBuilding(Building building)
        {
            if (building is MissileLauncher launcher)
            {
                missileLaunchers.Add(launcher);
                building.health.OnDeath += HandleMissileLauncherDestruction;
            }
            else if (building is MoneyStructure structure)
            {
                moneyStructures.Add(structure);
                building.health.OnDeath += HandleMoneyStructureDestruction;
            }
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

        private void HandleMoneyStructureDestruction(DeathEvent e)
        {
            Health target = e.target;
            // Remove launcher by health
            for (int i = 0; i < moneyStructures.Count; i++)
            {
                MoneyStructure m = moneyStructures[i];
                if (m.health == target)
                {
                    // Remove launcher and early exit
                    moneyStructures.RemoveAt(i);
                    break;
                }
            }
            // The objective is to have at least one non-silo building 
            // If there are no buildings it means player lost the game
            // However check if damage event is present to distingush destruction types
            // When damage event is null skip game over check
            if (moneyStructures.Count == 0 && e.damageEvent != null)
            {
                // Destroy all buildings
                DestroyAllBuildings();
                // Trigger event
                onGameOver.Invoke();
            }
        }

        private void DestroyAllBuildings()
        {
            // Remove all buildings
            for (int i = 0; i < moneyStructures.Count; i++)
            {
                Destroy(moneyStructures[i].gameObject);
            }
            moneyStructures.Clear();
            for (int i = 0; i < missileLaunchers.Count; i++)
            {
                Destroy(missileLaunchers[i].gameObject);
            }
            missileLaunchers.Clear();
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
