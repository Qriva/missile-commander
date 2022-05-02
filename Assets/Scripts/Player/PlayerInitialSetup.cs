using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PlayerInitialSetup : MonoBehaviour
    {
        public Setup[] setups;

        public void InitializeBuildings()
        {
            for (int i = 0; i < setups.Length; i++)
            {
                Setup setup = setups[i];
                if (setup.building == null || setup.slot == null)
                {
                    Debug.LogErrorFormat(this, "Building setup with index '{0}' is not valid,", i);
                }
                // Create building inside slot
                Building building = Instantiate(setup.building, setup.slot.transform);
                // Register building
                setup.slot.playerController.AddBuilding(building);
            }
        }

        [System.Serializable]
        public class Setup
        {
            public Building building;
            public BuildingSlot slot;
        }
    }
}
