using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Game.UserInterface
{
    [RequireComponent(typeof(CanvasGroup), typeof(NavigationGuard))]
    public class UIContainer : MonoBehaviour
    {
        [ReadOnly]
        public GameObject lastSelected;
        public GameObject defaultSelected;
        private CanvasGroup group;

        void Awake()
        {
            group = GetComponent<CanvasGroup>();
            lastSelected = defaultSelected;
        }

        public virtual void Open()
        {
            // Open panel
            this.gameObject.SetActive(true);
            // Select remembered control or set to default
            GameObject controlToSelect = (lastSelected == null)? defaultSelected : lastSelected;
            EventSystem.current.SetSelectedGameObject(controlToSelect);
        }

        public virtual void Close()
        {
            // Remember last selected control
            lastSelected = EventSystem.current.currentSelectedGameObject;
            // Close panel
            this.gameObject.SetActive(false);
        }

        public virtual void CloseAndReset()
        {
            // Set last selected control to default
            lastSelected = defaultSelected;
            // Close panel
            this.gameObject.SetActive(false);
        }

        public virtual void EnableInteraction()
        {
            // Check if this window is open
            if (!this.gameObject.activeSelf)
            {
                Debug.LogWarning("UIContainer interaction should not be changed when closed.", this);
            }
            // Select remembered control or set to default
            EventSystem.current.SetSelectedGameObject(lastSelected);
            // Enable interaction of all elements in container
            group.interactable = true;
        }

        public virtual void DisableInteraction()
        {
            // Check if this window is open
            if (!this.gameObject.activeSelf)
            {
                Debug.LogWarning("UIContainer interaction should not be changed when closed.", this);
            }
            // Remember last selected control
            lastSelected = EventSystem.current.currentSelectedGameObject;
            // Disable interaction of all elements in container
            group.interactable = false;

        }

        void OnValidate()
        {
            if (defaultSelected != null)
            {
                Transform parent = defaultSelected.transform.parent;
                while(parent != null)
                {
                    if (parent.gameObject == this.gameObject)
                    {
                        // Default selection is valid
                        return;
                    }
                    parent = parent.parent;
                }
                // This object is not in hierarchy, set to null
                defaultSelected = null;
            }
        }
    }
}
