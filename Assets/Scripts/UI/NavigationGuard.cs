using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UserInterface
{
    /// <summary>
    /// Protects user from losing selection
    /// </summary>
    public class NavigationGuard : MonoBehaviour
    {
        [ReadOnly]
        public GameObject lastSelected;

        void LateUpdate()
        {
            // Get current selected
            GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
            // If there is no selected object set the previous one
            if (currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(lastSelected);
                // Return to not override last selected object
                return;
            }
            // Update last selected object
            lastSelected = currentSelectedGameObject;
        }
    }
}
