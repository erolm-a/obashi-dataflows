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
        public Dictionary<(int, int), GameObject> edgeList;

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

        /// <summary>
        /// Make a FlowGraph instance from a JSON payload.
        /// Please call this method only when an anchor point has been chosen.
        /// </summary>
        /// <param name="jsonPayload">The JSON string</param>
        public void Deserialize(string jsonPayload)
        {
            SerializableFlowGraph deserialized = JsonUtility.FromJson<SerializableFlowGraph>(jsonPayload);

            foreach (var device in deserialized.devices)
            {
                // AddDevice(Enum.Parse(typeof(DeviceType), device.type), device.scene_id);
            }

        }


        void Start()
        {
            edgeList = new Dictionary<(int, int), GameObject>();
            adjacencyList = new Dictionary<int, List<int>>();
            id2GameObject = new Dictionary<int, GameObject>();
        }
    }

    [Serializable]
    class SerializableFlowGraph
    {
        public string name;
        public List<SerializableDevice> devices;
        public List<SerializableCord> cords;

        private SerializableFlowGraph(FlowGraph flowGraph)
        {
            this.name = flowGraph.name;
            this.devices = new List<SerializableDevice>();
            this.cords = new List<SerializableCord>();

            foreach (int sceneID in flowGraph.id2GameObject.Keys)
            {
                GameObject device = flowGraph.id2GameObject[sceneID];
                Device deviceInfo = device.GetComponent<Device>();

                devices.Add(new SerializableDevice(sceneID, deviceInfo.deviceName, deviceInfo.deviceType, device.transform));
            }

            foreach ((int, int) ids in flowGraph.edgeList.Keys)
            {
                if (ids.Item1 <= ids.Item2)
                {
                    cords.Add(new SerializableCord(ids.Item1, ids.Item2));
                }
            }
        }

        /// <summary>
        /// Serialize a FlowGraph.
        /// </summary>
        /// <param name="flowGraph">The flow graph to serialize</param>
        /// <returns>A JSON payload of the serialization</returns>
        public static string Serialize(FlowGraph flowGraph)
        {
            return JsonUtility.ToJson(new SerializableFlowGraph(flowGraph));
        }

        /// <summary>
        /// Deserialize into a FlowGraph
        /// </summary>
        /// <param name="payload">The JSON payload fetched from the server</param>
        /// <param name="flowgraph">The flowgraph to create on. If its anchor
        ///     point has not been set, the function will do nothing
        /// </param>
        public static void Deserialize(String payload, FlowGraph flowGraph)
        {
            if (!flowGraph.globalAnchor)
            {
                return;
            }

            SerializableFlowGraph deserialized = JsonUtility.FromJson<SerializableFlowGraph>(payload);

            foreach (SerializableDevice device in deserialized.devices)
            {
                flowGraph.AddDevice((DeviceType)Enum.Parse(typeof(DeviceType), device.type), device.scene_id,
                                    new Vector3(device.x, device.y, device.z));
            }

            foreach (SerializableCord cord in deserialized.cords)
            {
                flowGraph.AddLink(cord.id1, cord.id2);
            }
        }
    }

    [Serializable]
    class SerializableDevice
    {
        public int scene_id;
        public string name;
        public string type;

        public float x;
        public float y;
        public float z;

        /// <summary>
        /// Create a serializable representation of a device. This is meant to be used by SerializableFlowGraph when serializing.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="position"></param>
        public SerializableDevice(int id, string name, DeviceType type, Transform position)
        {
            this.scene_id = id;
            this.name = name;
            this.type = type.ToString();

            x = position.position.x;
            y = position.position.y;
            z = position.position.z;
        }
    }

    [Serializable]
    class SerializableCord
    {
        public int id1;
        public int id2;

        public SerializableCord(int id1, int id2)
        {
            this.id1 = id1;
            this.id2 = id2;
        }
    }
}
