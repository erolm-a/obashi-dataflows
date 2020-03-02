using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataFlows
{
    /// <summary>
    /// Perform an animation from the previous menu to the next submenu. Handle coming back
    /// </summary>
    public class OpenSelectionMenu : MonoBehaviour
    {
        /// <summary>
        /// Menu to open when the back button is pressed
        /// </summary>
        public GameObject SourceMenu;

        /// <summary>
        /// Menu to open when button is pressed
        /// </summary>
        public GameObject DestinationMenu;

        public void OnTriggered()
        {
            // TODO: Possibly add an animations
            DestinationMenu.SetActive(true);
            SourceMenu.SetActive(false);
        }

        void Update()
        {
            // Jump back to the original menu if pressing the back button (aka ESCAPE here)
            if (Input.GetKey(KeyCode.Escape))
            {
                SourceMenu.SetActive(true);
                DestinationMenu.SetActive(false);
            }
        }
    }

}