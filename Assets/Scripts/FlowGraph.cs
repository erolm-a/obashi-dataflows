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

            Device device = newObject.GetComponent<Device>();
            device.deviceId = id;

            adjacencyList[id] = new List<int>();
            id2GameObject[id] = newObject;

            newObject.transform.SetParent(globalAnchor.transform.parent);

            Debug.Log($"Created device (id: {id}) ");
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

            GameObject newLink;
            if (!edgeList.TryGetValue((min_id, max_id), out newLink))
            {
                GameObject first = id2GameObject[min_id], second = id2GameObject[max_id];

                newLink = Link.GetComponent<CordFlare>().create(first, second);
                newLink.transform.parent = globalAnchor.transform;

                adjacencyList[min_id].Add(max_id);
                adjacencyList[max_id].Add(min_id);
                edgeList.Add((min_id, max_id), newLink);
            }
            return newLink;
        }

        /// <summary>
        /// Disconnect a device from the graph, without actually destroying it.
        /// However, it destroys links (if they exist).
        /// <param name="id">The id of the device within the scene</param>
        /// </summary>
        void DeleteDevice(int id)
        {
            var adjacentVertices = adjacencyList[id];
            foreach (int adjacentId in adjacentVertices)
            {
                adjacencyList[adjacentId].Remove(id);
                var key = (Math.Min(id, adjacentId), Math.Max(id, adjacentId));
                GameObject link = edgeList[key];
                edgeList.Remove(key);
                Destroy(link);
            }

            adjacencyList[id].Clear();
        }

        /// <summary>
        /// If the pawn is a device, call `DeleteDevice()` with its id and `Destroy()` it.
        /// If the pawn is a link, disconnect its endpoints and `Destroy()` the link.
        /// Otherwise, just ignore.
        /// <param name="pawn"></param>
        /// </summary>
        public void DeleteObject(GameObject pawn)
        {
            int? id = GetDeviceId(pawn);

            // It is a device
            if (id.HasValue)
            {
                DeleteDevice(id.Value);
                Destroy(pawn);
            }
            else
            {
                var flare = pawn.GetComponent<CordFlare>();
                if (!flare)
                {
                    return;
                }

                int? id1 = GetDeviceId(flare.endpoints.Item1);
                int? id2 = GetDeviceId(flare.endpoints.Item2);

                if (id1.HasValue && id2.HasValue)
                {
                    var key = (Math.Min(id1.Value, id1.Value), Math.Max(id1.Value, id2.Value));

                    adjacencyList[key.Item1].Remove(key.Item2);
                    adjacencyList[key.Item2].Remove(key.Item1);
                    edgeList.Remove(key);
                    Destroy(pawn);
                }
            }
        }

        /// <summary>
        /// Get the id of a device.
        /// <param name="pawn">The device to extract the device info from</param>
        /// <returns>The id of the device, if the object has a Device script (or one of its derivatives) in it, or a null</returns>
        /// </summary>
        public static int? GetDeviceId(GameObject pawn)
        {
            if (pawn == null)
            {
                return null;
            }
            Device device = pawn.GetComponent<Device>();
            if (device == null)
            {
                return null;
            }
            return device.deviceId;
        }

        void Start()
        {
            edgeList = new Dictionary<(int, int), GameObject>();
            adjacencyList = new Dictionary<int, List<int>>();
            id2GameObject = new Dictionary<int, GameObject>();
        }
    }

    /*
    [Serializable]
    class SerializableFlowGraph
    {
        public int scene;

        SerializableDevice[] devices;
        SerializableCord[] cords;

        private SerializableFlowGraph(int sceneId, FlowGraph flowGraph)
        {
            this.scene = sceneId;
            // flowGraph.id2GameObject
        }
        public static string Serialize(int sceneId, FlowGraph flowGraph)
        {
            return JsonUtility.ToJson(new SerializableFlowGraph(sceneId, flowGraph));
        }
    }

    [Serializable]
    class SerializableDevice
    {
        public int scene_id;
        public string name;
        public string type;

        SerializableDevice(int id, string name, DeviceType type)
        {

        }


    }

    [Serializable]
    class SerializableCord
    {
        public int id1;
        public int id2;
    }

    */
}
