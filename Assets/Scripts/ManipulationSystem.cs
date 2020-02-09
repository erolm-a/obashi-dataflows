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

        private DataFlows.DeviceType deviceType;

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
            //foreach (Transform child in selected.transform)
            //    Destroy(transform.gameObject);

            Renderer[] renderer = selected.focusedObject.GetComponentsInChildren<Renderer>();

            renderer[0].material = oldUnselectedMaterial;
            selected.focusedObject = null;
            selected.focusedId = -1;
        }

        /// <summary>
        /// Select the object and add a material to make the selection evident
        /// </summary>
        /// <param name="toSelect">The object to select</param>
        private void SelectPawn(GameObject toSelect)
        {
            selected.focusedObject = toSelect;
            selected.focusedId = flowGraph.GameObject2Id[toSelect];
            Renderer[] renderer = selected.focusedObject.GetComponentsInChildren<Renderer>();
            oldUnselectedMaterial = renderer[0].material;
            renderer[0].material = SelectedMaterial;
        }

        private int max_id = 0;

        /// <summary>
        /// Capture a tap gesture
        /// </summary>
        /// <param name="gesture">The gesture</param>
        private void TapGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                if (selected.focusedObject == null)
                {
                    Debug.Log("No selected objects, trying to create a device");
                    MainARController.Log($"Focus position: ${gesture.FocusX} {gesture.FocusY}");
                    Touch touch = Input.GetTouch(0);
                    // raycast against the location the player tapped to search for planes
                    TrackableHit hit;
                    TrackableHitFlags raycastFlags = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;

                    if (Frame.Raycast(gesture.FocusX, gesture.FocusY, raycastFlags, out hit))
                    {
                        Debug.Log("Raycasting found a plane at " + hit.Pose.position);
                        // check if we are hitting a plane (again) and the
                        // plane is not on the roof
                        if (hit.Trackable is DetectedPlane && Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                                        hit.Pose.rotation * Vector3.up) >= 0)
                        {

                            DetectedPlane plane = hit.Trackable as DetectedPlane;
                            if (plane.PlaneType == DetectedPlaneType.HorizontalUpwardFacing)
                            {
                                Debug.Log("Hit a plane, trying to create a vertex");

                                if (!flowGraph.globalAnchor)
                                {
                                    flowGraph.globalAnchor = hit.Trackable.CreateAnchor(hit.Pose);
                                }
                                var pawn = flowGraph.AddDevice(deviceType, max_id++, hit.Pose.position);
                                SelectPawn(pawn);
                            }
                        }
                    }
                    else
                    {
                        MainARController.Log("No plane found! Tap on a mesh plane to create a device!");
                    }
                }
                else
                {
                    Debug.Log("Trying to deselect current gameobject");
                    // check if I can raycast to the object
                    RaycastHit hit;
                    Ray ray = FirstPersonCamera.ScreenPointToRay(new Vector2(gesture.FocusX, gesture.FocusY));

                    DeselectPawn();
                    MainARController.Log(selected.focusedObject == null ? "Deselected!" : "Not yet deselected");
                    if (Physics.Raycast(ray, out hit))
                    {
                        Transform objectHit = hit.transform;

                        // change target
                        if (objectHit.gameObject.tag == "Selectable")
                        {
                            MainARController.Log("Reselected object position: " + objectHit.gameObject.transform.position);
                            SelectPawn(objectHit.gameObject);
                        }
                    }
                }
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
        public void OnDropdownChange()
        {
            var options = DeviceDropdown.options;
            var option = options[DeviceDropdown.value];

            MainARController.Log($"Selected option {option.text}");
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