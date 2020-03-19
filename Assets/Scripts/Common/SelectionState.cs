using System.Collections.Generic;
using UnityEngine;
using cakeslice;

namespace DataFlows
{
    struct SelectionState
    {
        /// <summary>
        /// The object that is currently being selected.
        /// NB: The object should have a script that inherits from `DataFlow.Device`
        /// </summary>
        public GameObject focusedObject;

        /// <summary>
        /// The previous object that was being selected. Useful for links.
        /// NB: The object should have a script that inherits from `DataFlow.Device`
        /// <summary>
        public GameObject previousFocusedObject;

        /// <summary>
        /// Select the current object. After this call focusedObject will point to pawn.
        /// <param name="pawn">The object to select</param>
        /// </summary>
        public void Select(GameObject pawn)
        {
            focusedObject = pawn;

            var list = pawn.GetComponentInChildren<Outline>();
            Debug.Log($"Selecting {pawn}");
            Debug.Log($"We have {list} here");

            foreach (var outline in pawn.GetComponentsInChildren<Outline>())
            {
                outline.enabled = true;
            }

            // When re-selecting the same object as before trigger an action
            if (focusedObject == previousFocusedObject)
            {
                focusedObject.GetComponent<Device>()?.OnUserSelect();
            }

        }

        /// <summary>
        /// Unfocus the previous object and select the given pawn.
        /// This is equivalent to perform `Unfocus(); Select(pawn);` .
        /// <param name="pawn">The object to focus</param>
        /// </summary>
        public void UnfocusAndSelect(GameObject pawn)
        {
            // Deselect all the outlines.
            if (focusedObject)
            {
                foreach (var outline in focusedObject.GetComponentsInChildren<Outline>())
                {
                    outline.enabled = false;
                }
            }

            previousFocusedObject = focusedObject;
            focusedObject = pawn;
        }

        /// <summary>
        /// Unfocus the current element. The previous element becomes the current element.
        /// </summary>
        public void Unfocus()
        {
            if (focusedObject)
            {
                foreach (var outline in focusedObject.GetComponentsInChildren<Outline>())
                {
                    outline.enabled = false;
                }
            }
            previousFocusedObject = focusedObject;
            focusedObject = null;
        }
    }
}
