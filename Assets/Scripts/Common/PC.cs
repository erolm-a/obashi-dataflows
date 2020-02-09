using UnityEngine;

namespace DataFlows
{
    /// <summary>
    /// Manage a PC entity.
    /// A PC
    /// </summary>
    public class PC : Device
    {
        void Start()
        {
            Populate(OSIStack.APPLICATION, "PC1");
        }

        public override void OnUserSelect()
        {
            MainARController.Log("Picked a PC!");
        }
    }
}
