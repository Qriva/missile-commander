using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UserInterface
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class BuildingSlotUI : MonoBehaviour
    {
        /// <summary>
        /// Target slot, filled Show method
        /// </summary>
        /// <value></value>
        public BuildingSlot slot { get; set; }
        public MoneyStructure moneyStructurePrefab;
        public Button moneyStructureButton;
        public MissileLauncher missileLauncherPrefab;
        public Button missileLauncherButton;
        public System.Action onBuild;

        public void BuildMoneyStructure()
        {
            // Create building inside slot
            Building building = Instantiate(moneyStructurePrefab, slot.transform);
            // Register building and reduce money
            slot.playerController.AddBuilding(building);
            slot.playerController.ReduceFunds(moneyStructurePrefab.cost);
            // Trigger event
            onBuild?.Invoke();
        }

        public void BuildMissileLauncher()
        {
            // Create building inside slot
            Building building = Instantiate(missileLauncherPrefab, slot.transform);
            // Register building and reduce money
            slot.playerController.AddBuilding(building);
            slot.playerController.ReduceFunds(missileLauncherPrefab.cost);
            // Trigger event
            onBuild?.Invoke();
        }

        public void Show(Vector3 position, BuildingSlot slot)
        {
            this.slot = slot;
            this.gameObject.SetActive(true);
            // Transform WS position to canvas space
            Vector2 viewportPos = Camera.main.WorldToViewportPoint(position);
            RectTransform rectTransform = this.transform as RectTransform;
            rectTransform.anchorMin = viewportPos;
            rectTransform.anchorMax = viewportPos;
            // Disable or enable button
            int funds = slot.playerController.Funds;
            moneyStructureButton.interactable = moneyStructurePrefab.cost <= funds;
            missileLauncherButton.interactable = missileLauncherPrefab.cost <= funds;
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}
