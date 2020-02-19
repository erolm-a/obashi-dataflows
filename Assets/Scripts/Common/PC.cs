﻿namespace DataFlows
{
    /// <summary>
    /// Manage a PC entity.
    /// A PC
    /// </summary>
    public class PC : Device
    {

        public override void OnUserSelect()
        {
            MainARController.Log($"Picked the PC {this.deviceName} (id ${this.deviceId} )!");
        }
    }
}
