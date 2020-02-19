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
        PRINTER,
        LINK
    }

    public abstract class Device : MonoBehaviour
    {
        public OSIStack osiType;
        [HideInInspector]
        public string deviceName;
        [HideInInspector]
        public int deviceId;

        /// <summary>
        /// Action to perform when the object is selected
        /// </summary>
        public abstract void OnUserSelect();
    }
}