using UnityEngine;
using DigitalRubyShared;
using DataFlows.Commons;
using System.Collections;

namespace DataFlows
{
    public class ExplorationGraph : MonoBehaviour
    {
        /// <summary>
        /// The URL of the middleware to connect to.
        /// </summary>
        public Camera FirstPersonCamera;

        /// <summary>
        /// The child flow graph script.
        /// </summary>
        private FlowGraph flowGraph;

        /// <summary>
        /// The result from Api.GetScene. It is here to launch 
        /// </summary>
        private string deserialized;

        /// <summary>
        /// The id of the scene to load.
        /// </summary>
        [HideInInspector]
        public int sceneId
        {
            set
            {
                StartCoroutine(Api.GetScene(value, LoadSceneCallback));
            }
        }

        private TapGestureRecognizer tapGesture;

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
        /// Callback for when a tap is performed
        /// </summary>
        /// <param name="gesture">The most recent gesture</param>
        private void TapGestureCallback(GestureRecognizer gesture)
        {
            // Create a new global anchor
            if (!flowGraph.globalAnchor)
            {
                var hit = Raycastings.RaycastOnPlane(gesture.FocusX, gesture.FocusY, FirstPersonCamera);
                if (hit.HasValue)
                {
                    flowGraph.globalAnchor = hit.Value.Trackable.CreateAnchor(hit.Value.Pose);
                    SerializableFlowGraph.Deserialize(deserialized, flowGraph);
                }
            }
        }

        private void LoadSceneCallback(string deserialized)
        {
            this.deserialized = deserialized;
        }

        void Start()
        {
            CreateTapGesture();
            MainARController.Log("Started Edit mode!");

            flowGraph = GetComponentInChildren<FlowGraph>();
            if (flowGraph == null)
            {
                Debug.Log("ManipulationSystem needs a flow graph as a child");
                Destroy(gameObject);
            }
        }
    }

}