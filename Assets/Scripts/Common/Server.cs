using UnityEngine;

namespace DataFlows
{
    /// <summary>
    /// Manage a Server entity.
    /// </summary>
    public class Server : Device
    {
        public override void OnUserSelect()
        {
            MainARController.Log("Picked a Server!");
        }
    }
}