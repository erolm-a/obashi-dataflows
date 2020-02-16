using System;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;


namespace DataFlows
{
    public class FlowGraph : MonoBehaviour
    {

        /// <summary>
        /// The anchor that roots the whole graph on
        /// </summary>
        [HideInInspector]
        public Anchor globalAnchor;

        public GameObject PCPawn;

        public GameObject RouterPawn;
        public GameObject SwitchPawn;
        public GameObject ServerPawn;

        public GameObject Link;

        /// <summary>
        /// Graph data represent vertices as integers for a more compact representation.
        /// </summary>
        public Dictionary<int, GameObject> id2GameObject { get; private set; }

        /// <summary>
        /// Reverse of `id2GameObject`
        /// </summary>
        public Dictionary<GameObject, int> GameObject2Id { get; private set; }
        /// <summary>
        /// A normal adjacency list.
        /// </summary>
        private Dictionary<int, List<int>> adjacencyList;

        /// <summary>
        /// Maps pairs of device IDs to the cord that joins them.
        /// </summary>
        private Dictionary<(int, int), GameObject> edgeList;

        /// <summary>
        /// Add a device to the graph
        /// </summary>
        /// <param name="type">The type of the device to instantiate</param>
        /// <returns>The id of the device</returns>
        public GameObject AddDevice(DeviceType type, int id, Vector3 position)
        {
            if (globalAnchor == null)
            {
                Debug.Log("Trying to create a graph on a null anchor point");
                return null;
            }
            Debug.Log($"Trying to create device {type}");

            GameObject newObject;

            switch (type)
            {
                case DeviceType.PC:
                    newObject = Instantiate(PCPawn, position, Quaternion.identity);
                    break;
                case DeviceType.ROUTER:
                    newObject = Instantiate(RouterPawn, position, Quaternion.identity);
                    break;
                case DeviceType.SERVER:
                    newObject = Instantiate(ServerPawn, position, Quaternion.identity);
                    break;
                default:
                    newObject = Instantiate(PCPawn, position, Quaternion.identity);
                    break;
            }

            Debug.Log($"newObject is {newObject}");
            adjacencyList[id] = new List<int>();

            id2GameObject[id] = newObject;
            GameObject2Id[newObject] = id;

            newObject.transform.parent = globalAnchor.transform.parent;

            return newObject;
        }

        /// <summary>
        /// Create a link between the two devices
        /// </summary>
        /// <param name="id1">The id of the first device</param>
        /// <param name="id2">The id of the second device</param>
        /// <returns>The newly instantiated link, or the existing one if the link already existed.</returns>
        public GameObject AddLink(int id1, int id2)
        {

            int min_id = Math.Min(id1, id2);
            int max_id = Math.Max(id1, id2);

            // If the
            GameObject newLink;
            if (!edgeList.TryGetValue((min_id, max_id), out newLink))
            {
                GameObject first = id2GameObject[min_id], second = id2GameObject[max_id];

                newLink = Link.GetComponent<CordFlare>().create(first, second);
                newLink.transform.parent = globalAnchor.transform;

                adjacencyList[min_id].Add(max_id);
                adjacencyList[max_id].Add(min_id);
            }
            return newLink;
        }

        void Start()
        {
            edgeList = new Dictionary<(int, int), GameObject>();
            adjacencyList = new Dictionary<int, List<int>>();
            id2GameObject = new Dictionary<int, GameObject>();
            GameObject2Id = new Dictionary<GameObject, int>();
        }
    }
}
