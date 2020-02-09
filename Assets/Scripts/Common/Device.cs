using UnityEngine;

namespace DataFlows
{

    public enum OSIStack
    {
        DATA_LINK,
        IP,
        TRANSPORT,
        APPLICATION
    }

    public enum DeviceType
    {
        PC,
        ROUTER,
        SWITCH,
        SERVER,
        PRINTER
    }

    public abstract class Device : MonoBehaviour
    {
        public OSIStack osiType { get; private set; }
        public string deviceName { get; private set; }

        public void Populate(OSIStack osiType, string deviceName)
        {
            this.osiType = osiType;
            this.deviceName = deviceName;
        }

        /// <summary>
        /// Action to perform when the object is selected
        /// </summary>
        public abstract void OnUserSelect();
    }
}