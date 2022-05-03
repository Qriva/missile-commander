using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class BuildingSlot : MonoBehaviour
    {
        public PlayerController playerController;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, 0.25f);
        }

        // Slot is empty when there are no children
        public bool IsOccupied => transform.childCount > 0;
    }
}
