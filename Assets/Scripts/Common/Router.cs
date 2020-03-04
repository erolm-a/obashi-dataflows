using UnityEngine;

namespace DataFlows
{
    /// <summary>
    /// Manage a PC entity.
    /// A PC
    /// </summary>
    public class Router : Device
    {
        private int counter = 0;
        void Awake()
        {
            this.deviceName = "Router" + counter++;
        }

        public override void OnUserSelect()
        {
            MainARController.Log("Picked a Server!");
        }
    }
}