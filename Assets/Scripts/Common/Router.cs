using UnityEngine;

namespace DataFlows
{
    /// <summary>
    /// Manage a Router
    /// </summary>
    public class Router : Device
    {
        private int counter = 0;

        new void Start()
        {
            Debug.Log("Reached start here!");
            base.Start();
            this.deviceName = "Router" + counter++;
            instantiatedTooltip.SetText(this.deviceName);
        }

        public override void OnUserSelect()
        {
            MainARController.Log("Picked a Router!");
        }
    }
}
