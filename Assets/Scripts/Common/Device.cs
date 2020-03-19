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
        public DeviceType deviceType;

        /// <summary>
        /// A tooltip to display small contextual information with, for example the device name.
        /// </summary>
        public GameObject tooltipBillboard;

        /// <summary>
        /// A canvas to display on screen as users tap on the objects.
        /// Children have free control over this game object
        /// </summary>
        public GameObject infoCanvas;

        void Awake()
        {
            var newBillboard = Instantiate(tooltipBillboard, new Vector3(0.0f, 0.2f, 0.0f), Quaternion.identity);
            newBillboard.transform.SetParent(transform);
        }

        /// <summary>
        /// Action to perform when the object is selected
        /// </summary>
        public abstract void OnUserSelect();

        
    }
}
