using UnityEngine;
using DigitalRubyShared;
using DataFlows.Commons;
using System.Collections;

namespace DataFlows
{
    public class ExplorationGraph : MonoBehaviour
    {
        /// <summary>
        /// A reference to the first person camera, needed for object selection.
        /// </summary>
        public Camera FirstPersonCamera;

        /// <summary>
        /// The child flow graph script.
        /// </summary>
        private FlowGraph flowGraph;

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
                    MainARController.instance.currentScene.UpdateFlowGraph(flowGraph);
                }
            }
            else
            {
                // TODO: Query objects
            }
        }

        void Start()
        {
            CreateTapGesture();
            MainARController.Log("Started Edit mode!");

            flowGraph = GetComponentInChildren<FlowGraph>();
            MainARController.instance.currentScene.UpdateFlowGraph(flowGraph);
        }
    }

}