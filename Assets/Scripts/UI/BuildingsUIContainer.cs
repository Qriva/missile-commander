using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UserInterface
{
    [DisallowMultipleComponent]
    public class BuildingsUIContainer : UIContainer
    {
        #region Inspector
        [SerializeField] private BuildingSlot[] buildingSlots;
        [SerializeField] private BuildingSlotUI slotUITemplate;
        #endregion

        private BuildingSlotUI[] buildingSlotUIs;
        private bool initialized = false;

        public override void Open()
        {
            // Lazy initalize
            if (initialized == false)
            {
                Initialize();
            }

            base.Open();
            ShowHideUIElements();
        }

        private void Initialize()
        {
            // Set flag to initialize once
            initialized = true;
            // Create UI for each slot
            buildingSlotUIs = new BuildingSlotUI[buildingSlots.Length];
            for (int i = 0; i < buildingSlots.Length; i++)
            {
                BuildingSlotUI slotUI = buildingSlotUIs[i] = Instantiate(slotUITemplate, this.transform);
                // Setup slot and event
                slotUI.onBuild += ShowHideUIElements;
                slotUI.slot = buildingSlots[i];
            }
        }

        private void ShowHideUIElements()
        {
            // Find all not occupied slots and show UI to buy building
            for (int i = 0; i < buildingSlots.Length; i++)
            {
                // Show or hide UI for slot
                BuildingSlot slot = buildingSlots[i];
                if (!slot.IsOccupied)
                {
                    buildingSlotUIs[i].Show(slot.transform.position, slot);
                }
                else
                {
                    buildingSlotUIs[i].Hide();
                }
            }
        }

        public override void Close()
        {
            base.Close();
        }

        public override void CloseAndReset()
        {
            base.CloseAndReset();
        }
    }
}
