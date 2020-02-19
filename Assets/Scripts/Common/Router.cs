using UnityEngine;

namespace DataFlows
{
    /// <summary>
    /// Manage a PC entity.
    /// A PC
    /// </summary>
    public class Router : Device
    {
        public override void OnUserSelect()
        {
            MainARController.Log("Picked a Server!");
        }
    }
}