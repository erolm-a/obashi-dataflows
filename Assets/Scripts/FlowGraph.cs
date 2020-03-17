using System;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using System.Linq;

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
        /// The id of the scene.
        /// </summary>
        [HideInInspector]
        public int id;

        /// <summary>
        /// The name of the scene
        /// </summary>
        [HideInInspector]
        public string sceneName;

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
                case DeviceType.SWITCH:
                    newObject = Instantiate(SwitchPawn, position, Quaternion.identity);
                    break;
                default:
                    Debug.Log("Could not understand which type was specified, defaulting to PC");
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
            id2GameObject.Remove(id);
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


    [Serializable]
    public class SerializableFlowGraph
    {
        public string name = "";
        public List<SerializableDevice> devices;
        public List<SerializableCord> cords;

        [NonSerialized]
        public int id;


        /// <summary>
        /// A wrapper for an int.
        /// </summary> 
        [Serializable]
        private class IdWrapper
        {
            public int id = -1;
        }

        /// <summary>
        /// HACK: wrap a list of scenes in a single scene.
        /// This is required as Unity's JsonUtility does not let us have root-level arrays.
        /// </summary>
        [Serializable]
        class ListWrapper<T>
        {
            public T[] content = { };
        }

        private SerializableFlowGraph(FlowGraph flowGraph)
        {
            this.name = flowGraph.sceneName;
            this.devices = new List<SerializableDevice>();
            this.cords = new List<SerializableCord>();

            foreach (int sceneID in flowGraph.id2GameObject.Keys)
            {
                GameObject device = flowGraph.id2GameObject[sceneID];
                Debug.Log($"Here device is {device}");
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
        /// Update the flow gaph from a given deserialized representation
        /// </summary>
        /// <param name="flowGraph">The flow graph to update</param>
        public void UpdateFlowGraph(FlowGraph flowGraph)
        {
            flowGraph.name = name;

            if (!flowGraph.globalAnchor)
            {
                return;
            }

            foreach (SerializableDevice device in devices)
            {
                Debug.Log(device.scene_device_id);
                flowGraph.AddDevice((DeviceType)Enum.Parse(typeof(DeviceType), device.type), device.scene_device_id,
                    new Vector3(device.x, device.y, device.z));
            }

            foreach (SerializableCord cord in cords)
            {
                flowGraph.AddLink(cord.device_1, cord.device_2);
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
        /// Deserialize into a SerializableFlowGraph.
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public static SerializableFlowGraph Deserialize(String payload)
        {
            Debug.Log($"Deserializing {payload}");
            var id = JsonUtility.FromJson<IdWrapper>(payload).id;
            Debug.Log($"Got id {id}");

            var deserialized = JsonUtility.FromJson<SerializableFlowGraph>(payload);
            deserialized.id = id;
            return deserialized;
        }

        public static SerializableFlowGraph[] DeserializeFromList(String payload)
        {
            String wrapper = "{\"content\": " + payload + "}";
            Debug.Log($"Deserializing ${payload}");
            IdWrapper[] ids = JsonUtility.FromJson<ListWrapper<IdWrapper>>(wrapper).content;
            var scenes = JsonUtility.FromJson<ListWrapper<SerializableFlowGraph>>(wrapper).content;
            for (int i = 0; i < ids.Length; i++)
            {
                scenes[i].id = ids[i].id;
            }

            return scenes;
        }
    }

    [Serializable]
    public class SerializableDevice
    {
        public int scene_device_id;
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
            this.scene_device_id = id;
            this.name = name;
            this.type = type.ToString();

            x = position.position.x;
            y = position.position.y;
            z = position.position.z;
        }
    }

    [Serializable]
    public class SerializableCord
    {
        public int device_1;
        public int device_2;

        public SerializableCord(int id1, int id2)
        {
            this.device_1 = id1;
            this.device_2 = id2;
        }
    }
}
