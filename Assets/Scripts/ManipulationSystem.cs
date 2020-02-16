using DigitalRubyShared;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;

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
        /// Cord prefab that connects the vertices
        /// </summary>
        public GameObject Cord;
        /// <summary>
        /// Material for a selected pawn
        /// </summary>
        public Material SelectedMaterial;

        /// <summary>
        /// Flip add/delete button
        /// </summary>
        public Button AddDeleteButton;

        /// <summary>
        /// Control the dropdown to select a device to add (when in Add mode)
        /// </summary>
        public Dropdown DeviceDropdown;


        private TapGestureRecognizer tapGesture;
        private PanGestureRecognizer panGesture;


        private SelectionState selected;


        private Material oldUnselectedMaterial = null;

        private bool isPaused = false;

        private EditMode mode;

        private DataFlows.DeviceType deviceType = DeviceType.PC;

        /// <summary>
        /// Access the FlowGraph prefab
        /// </summary>
        private FlowGraph flowGraph;


        /// <summary>
        /// Remove visual hint under a pawn, making it unselected.
        /// As a side effect, selected is set to null and its second material is set to null.
        /// </summary>
        private void DeselectPawn()
        {
            if (selected.focusedObject == null)
                return;

            Renderer[] renderer = selected.focusedObject.GetComponentsInChildren<Renderer>();

            renderer[0].material = oldUnselectedMaterial;
            selected.Unfocus();
        }

        /// <summary>
        /// Select the object and add a material to make the selection evident
        /// </summary>
        /// <param name="toSelect">The object to select</param>
        private void SelectPawn(GameObject toSelect)
        {
            selected.Select(toSelect);
            Renderer[] renderer = selected.focusedObject.GetComponentsInChildren<Renderer>();
            oldUnselectedMaterial = renderer[0].material;
            renderer[0].material = SelectedMaterial;
        }

        private int max_id = 0;

        private GameObject RaycastOnDevice(float touchX, float touchY)
        {
            // Perform a raycast to see if there is a device that already exists
            RaycastHit hit;
            Ray ray = FirstPersonCamera.ScreenPointToRay(new Vector2(touchX, touchY));

            /// Check if succeded and if the parent object (that is, the object that contains the mesh) has the asked tag
            if (Physics.Raycast(ray, out hit) && hit.transform.parent.gameObject.tag == "Selectable")
            {
                return hit.transform.parent.gameObject;
            }
            return null;
        }

        /// <summary>
        /// Perform a ARCore raycast to find planes.
        /// <param name="touchX">The x-coordinate of the touch event based on the canvas reference frame.
        /// <param name="touchY">The y-coordinate of the touch event, based on the canvas reference frame.
        /// </summary>
        private TrackableHit? RaycastOnPlane(float touchX, float touchY)
        {
            // Raycast against the location the player tapped to search for planes
            TrackableHit hit;
            TrackableHitFlags raycastFlags = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;

            if (deviceType != DeviceType.LINK && Frame.Raycast(touchX, touchY, raycastFlags, out hit))
            {
                if (hit.Trackable is DetectedPlane && Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                                hit.Pose.rotation * Vector3.up) >= 0)
                {

                    DetectedPlane plane = hit.Trackable as DetectedPlane;
                    if (plane.PlaneType == DetectedPlaneType.HorizontalUpwardFacing)
                    {
                        return hit;
                    }
                }
            }
            return null;
        }

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


                GameObject selectable = RaycastOnDevice(gesture.FocusX, gesture.FocusY);
                if (selectable)
                {
                    Debug.Log("There is already a device at this location, selecting it!");
                    SelectPawn(selectable);
                    return;
                }

                /// Re-raycast to find a plane this time
                var hit = RaycastOnPlane(gesture.FocusX, gesture.FocusY);
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
                        SelectPawn(pawn);
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
                DeselectPawn();
                var selectable = RaycastOnDevice(gesture.FocusX, gesture.FocusY);

                if (selectable)
                {
                    MainARController.Log("Reselected object position" + selectable.transform.position);
                    SelectPawn(selectable);

                    if (deviceType == DeviceType.LINK && selected.previousFocusedObject)
                    {
                        int? id1 = FlowGraph.GetDeviceId(selected.focusedObject);
                        int? id2 = FlowGraph.GetDeviceId(selected.previousFocusedObject);

                        if (id1.HasValue && id2.HasValue)
                        {
                            var link = flowGraph.AddLink(id1.Value, id2.Value);
                            DeselectPawn();
                            SelectPawn(link);
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
            GameObject pawn = RaycastOnDevice(gesture.FocusX, gesture.FocusY);
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
        /// Create a tap gesture
        /// </summary>
        private void CreateTapGesture()
        {
            tapGesture = new TapGestureRecognizer();
            tapGesture.StateUpdated += TapGestureCallback;

            FingersScript.Instance.AddGesture(tapGesture);
        }


        private void SetAddDeleteButtonLabel(string label)
        {
            Text textLabel = AddDeleteButton.GetComponentInChildren<Text>();
            textLabel.text = label;
        }
        void Start()
        {
            CreateTapGesture();
            SetAddDeleteButtonLabel("Add devices");
            MainARController.Log("Started Edit mode!!");

            flowGraph = GetComponentInChildren<FlowGraph>();
            if (flowGraph == null)
            {
                Debug.Log("ManipulationSystem needs a flow graph as a child");
                Destroy(gameObject);
            }

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