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
        LINK
    }

    /// <summary>
    /// The base class for many device types.
    ///
    /// Each device gameobject must have:
    /// <list type="bullet">
    /// <item>
    /// A script component that inherits from <c>Device</c>
    /// </item>
    /// <item>
    /// The very first child that is either has a MeshRenderer component or
    /// is parent of many meshes. The latter is common for multi-mesh objects where having
    /// more transforms is desirable.
    /// </item>
    /// </list>
    /// </summary>
    public abstract class Device : MonoBehaviour
    {
        public OSIStack osiType;
        [HideInInspector]
        public string deviceName;
        [HideInInspector]
        public int deviceId;
        public DeviceType deviceType;

        /// <summary>
        /// A tooltip to display small contextual information with, for example
        /// the device name.
        /// Derived classes <b>must</b> set the parent transform.
        /// </summary>
        public GameObject tooltipBillboard;

        protected UI.Billboard instantiatedTooltip;

        /// <summary>
        /// A canvas to display on screen as users tap on the objects.
        /// Children have free control over this game object
        /// </summary>
        public GameObject infoCanvas;


        /// <summary>
        /// Initialize the tooltip. Note that we need to set up the tooltip
        /// to origin from the newly created device. However, this cannot
        /// be moved to <c>Awake()</c> as the transform
        /// <c>instantiatedTooltip</c> refers to is the newly spawned mesh,
        /// while in <c>Awake()</c> it is just the origin.
        /// </summary>
        protected void Start()
        {
            Debug.Log("Calling Start from Device.cs");

            var tooltipObject = Instantiate(tooltipBillboard, transform.GetChild(0));
            var tooltipTransform = tooltipObject.transform;
            instantiatedTooltip = tooltipObject.GetComponent<UI.Billboard>();
            tooltipTransform.localScale *= 3;
            tooltipTransform.localPosition += new Vector3(0.0f, 0.8f, 0.0f);
        }

        /// <summary>
        /// Action to perform when the object is selected
        /// </summary>
        public abstract void OnUserSelect();

        
    }
}
