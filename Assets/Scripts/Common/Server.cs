using UnityEngine;

namespace DataFlows
{
    /// <summary>
    /// Manage a Server entity.
    /// </summary>
    public class Server : Device
    {

        private int counter = 0;
        void Awake()
        {
            this.deviceName = "Server" + counter++;
        }

        public override void OnUserSelect()
        {
            MainARController.Log("Picked a Server!");
        }
    }
}