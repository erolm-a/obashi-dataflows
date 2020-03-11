using UnityEngine;

namespace DataFlows
{
    /// <summary>
    /// Manage a Server entity.
    /// </summary>
    public class Switch : Device
    {

        private int counter = 0;
        void Awake()
        {
            this.deviceName = "Switch" + counter++;
        }

        public override void OnUserSelect()
        {
            MainARController.Log("Picked a Switch!");
        }
    }
}