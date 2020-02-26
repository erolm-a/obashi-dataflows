using DigitalRubyShared;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using DataFlows.Commons;

namespace DataFlows
{
    enum EditMode
    {
        ADD,
        DELETE
    }

    public class ManipulationSystem : MonoBehaviour
    {
        /// <summary>
        /// Google ARCore Camera for raycasting
        /// </summary>
        public Camera FirstPersonCamera;

        /// <summary>
        /// Flip add/delete button
        /// </summary>
        public Button AddDeleteButton;

        /// <summary>
        /// Control the dropdown to select a device to add (when in Add mode)
        /// </summary>
        public Dropdown DeviceDropdown;


        /// <summary>
        /// The URL of the middleware to connect to.
        /// </summary>
        public string ServerURL = "https://glasgow-cs25-middleware.herokuapp.com";

        private TapGestureRecognizer tapGesture;
        private PanGestureRecognizer panGesture;

        private SelectionState selected;

        private bool isPaused = false;

        private EditMode mode;

        private DataFlows.DeviceType deviceType = DeviceType.PC;

        /// <summary>
        /// Access the FlowGraph prefab
        /// </summary>
        private FlowGraph flowGraph;

        private int max_id = 0;

        /// <summary>
        /// Perform an action in ADD mode.
        /// <param name="gesture">The gesture argument from FingersLite tap callback</param>
        /// </summary>
        private void AddDevices(GestureRecognizer gesture)
        {
            // Ignore gestures until they are done.
            if (gesture.State != GestureRecognizerState.Ended)
            {
                return;
            }

            if (selected.focusedObject == null)
            {
                MainARController.Log($"Focus position: ${gesture.FocusX} {gesture.FocusY}");
                Touch touch = Input.GetTouch(0);


                GameObject selectable = Raycastings.RaycastOnDevice(gesture.FocusX, gesture.FocusY, FirstPersonCamera);
                if (selectable)
                {
                    Debug.Log("There is already a device at this location, selecting it!");
                    selected.Select(selectable);
                    return;
                }

                /// Re-raycast to find a plane this time
                var hit = Raycastings.RaycastOnPlane(gesture.FocusX, gesture.FocusY, FirstPersonCamera);
                if (hit.HasValue)
                {
                    Debug.Log("Hit a plane, trying to create a vertex");

                    if (!flowGraph.globalAnchor)
                    {
                        flowGraph.globalAnchor = hit.Value.Trackable.CreateAnchor(hit.Value.Pose);
                    }

                    GameObject pawn = null;
                    pawn = flowGraph.AddDevice(deviceType, max_id++, hit.Value.Pose.position);
                    if (pawn)
                    {
                        selected.Select(pawn);
                    }
                }
                else
                {
                    MainARController.Log("No plane found! Tap on a mesh plane to create a device!");
                }
            }
            else
            {
                Debug.Log("Deselecting current gameobject");
                selected.Unfocus();
                var selectable = Raycastings.RaycastOnDevice(gesture.FocusX, gesture.FocusY, FirstPersonCamera);

                if (selectable)
                {
                    MainARController.Log("Reselected object position" + selectable.transform.position);
                    selected.Select(selectable);

                    if (deviceType == DeviceType.LINK && selected.previousFocusedObject)
                    {
                        int? id1 = FlowGraph.GetDeviceId(selected.focusedObject);
                        int? id2 = FlowGraph.GetDeviceId(selected.previousFocusedObject);

                        if (id1.HasValue && id2.HasValue)
                        {
                            var link = flowGraph.AddLink(id1.Value, id2.Value);
                            selected.UnfocusAndSelect(link);
                        }
                    }
                }
                else
                {
                    MainARController.Log("Nothing selected");
                }
            }
        }

        /// <summary>
        /// Delete a device.
        /// <param name="gesture">The gesture</param>
        /// </summary>
        private void DeleteDevices(GestureRecognizer gesture)
        {
            GameObject pawn = Raycastings.RaycastOnDevice(gesture.FocusX, gesture.FocusY, FirstPersonCamera);
            if (pawn)
            {
                flowGraph.DeleteObject(pawn);
            }
        }

        /// <summary>
        /// Capture a tap gesture
        /// <param name="gesture">The gesture</param>
        /// </summary>
        private void TapGestureCallback(GestureRecognizer gesture)
        {
            if (this.mode == EditMode.ADD)
            {
                AddDevices(gesture);
            }
            else if (this.mode == EditMode.DELETE)
            {
                DeleteDevices(gesture);
            }
        }

        /// <summary>
        /// Flip between add and delete button
        /// </summary>
        public void OnAddDeleteButtonPress()
        {

            this.mode = this.mode == EditMode.ADD ? EditMode.DELETE : EditMode.ADD;

            DeviceDropdown.gameObject.SetActive(this.mode == EditMode.ADD);
            DeviceDropdown.interactable = this.mode == EditMode.ADD;

            if (this.mode == EditMode.ADD)
                SetAddDeleteButtonLabel("Add devices");
            else
                SetAddDeleteButtonLabel("Remove devices");
        }

        /// <summary>
        /// Listen for a dropdown change.
        /// </summary>
        public void OnDropdownChange(int value)
        {
            var options = DeviceDropdown.options;
            var option = options[value];

            Debug.Log($"Selected option: {option.text}");
            if (option.text == "Router")
            {
                deviceType = DeviceType.ROUTER;
            }
            else if (option.text == "Server")
            {
                deviceType = DeviceType.SERVER;
            }
            else if (option.text == "PC")
            {
                deviceType = DeviceType.PC;
            }
            else if (option.text == "Link")
            {
                deviceType = DeviceType.LINK;
            }
        }

        /// <summary>
        /// Factory method to create an HTTP request for JSON.
        /// </summary>
        /// <param name="content">The payload (as a string) to send</param>
        /// <param name="method">A HTTP verb, such as GET, POST, PUT etc.</param>
        /// <returns>A UnityWebRequest</returns>
        private UnityWebRequest MakeRequest(string content, string method)
        {
            UnityWebRequest request = new UnityWebRequest(ServerURL + "/scenes/", method);
            byte[] payload = System.Text.Encoding.UTF8.GetBytes(content);

            request.SetRequestHeader("cache-control", "no-cache");
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(payload);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            return request;
        }

        /// <summary>
        /// Upload the current scene if it does not exist, or replace it if it exists.
        /// </summary>
        /// <returns></returns>
        public IEnumerator UploadScene()
        {
            MainARController.Log("Sending data");
            string serialized = SerializableFlowGraph.Serialize(flowGraph);
            UnityWebRequest request = MakeRequest(serialized, "POST");

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                MainARController.Log("Could not communicate with the server. Please see app logs for details.");
                Debug.LogError(request.error);
            }
            else
            {
                MainARController.Log("Upload completed!");
            }
        }

        /// <summary>
        /// Listen for a press on the Save button
        /// </summary>
        public void OnSaveButtonPress()
        {
            StartCoroutine(UploadScene());
        }

        /// <summary>
        /// Create a tap gesture
        /// </summary>
        private void CreateTapGesture()
        {
            tapGesture = new TapGestureRecognizer();
            tapGesture.StateUpdated += TapGestureCallback;

            FingersScript.Instance.AddGesture(tapGesture);
        }

        /// <summary>
        /// Update the label for AddDeleteButton
        /// </summary>
        /// <param name="label">The new label</param>
        private void SetAddDeleteButtonLabel(string label)
        {
            Text textLabel = AddDeleteButton.GetComponentInChildren<Text>();
            textLabel.text = label;
        }
        void Start()
        {
            CreateTapGesture();
            SetAddDeleteButtonLabel("Add devices");
            MainARController.Log("Started Edit mode!");

            flowGraph = GetComponentInChildren<FlowGraph>();
            if (flowGraph == null)
            {
                Debug.Log("ManipulationSystem needs a flow graph as a child");
                Destroy(gameObject);
            }

            flowGraph.name = "UnityTest";

            DeviceDropdown.onValueChanged.AddListener(OnDropdownChange);
        }

        void OnApplicationFocus(bool hasFocus)
        {
            isPaused = !hasFocus;
        }

        void OnApplicationPause(bool pauseStatus)
        {
            isPaused = pauseStatus;
        }
    }
}
