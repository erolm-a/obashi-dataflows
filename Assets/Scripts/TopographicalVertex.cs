using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

namespace Vertex {

    /// <summary>
    /// The TCP stack represents a "down-to-earth" version of the ISO/OSI stack
    /// actually used in system administration
    /// </summary>
    public enum TCP_STACK {
        PHISICAL,
        MAC,
        IP,
        TRANSPORT,
        SERVICE

    }

    public enum RouterFirmware {
        CISCO_IOS
    }

    public interface IPOIDevice {
        TCP_STACK protocolLevel {get;}
    }

    public class PC: IPOIDevice {
        public TCP_STACK protocolLevel {get { return TCP_STACK.SERVICE; } }

        /// <summary>
        /// Some operating systems
        /// <summary>
        public enum OperatingSystem {
            MS_WINDOWS,
            APPLE_MACOS,
            GNU_LINUX
        }

        public OperatingSystem operatingSystem {get; }
        public List<string> services {get; }

        public PC(OperatingSystem operatingSystem, List<string> services) {
            this.operatingSystem = operatingSystem;
            this.services = services;
        }
    }

    public class Router: IPOIDevice {
        public TCP_STACK protocolLevel {get { return TCP_STACK.TRANSPORT; } }

        /// <summary>
        /// Some operating systems or Firmwares
        /// <summary>
        public enum Firmware {
            CISCO_IOS
        }

        public Firmware firmware {get; }
        public List<string> services {get; }

        public Router(Firmware operatingSystem, List<string> services) {
            this.firmware = operatingSystem;
            this.services = services;
        }
    }

    public class TopographicalVertex : MonoBehaviour
    {

        /// <summary>
        /// vertex id
        /// </summary>
        public int id;

        private static int m_maxid = 0;

        public enum VertexType {
            /// <summary>
            /// Doors are obstacles that "stop" from prosecuting to another vertex
            /// </summary>
            DOOR,
            /// <summary>
            /// Corners make the path "curve" but do not necessarily stop it
            /// </summary>
            CORNER,
            /// Point of Interest
            POI
        }

        /// <summary>
        /// GameObject to instantiate for a Door anchor
        /// </summary>
        public GameObject DoorAnchor;

        /// <summary>
        /// GameObject to instantiate for a Corner anchor
        /// </summary>
        public GameObject CornerAnchor;

        /// <summary>
        /// GameObject to instantiate for the POI anchor
        /// </summary>
        public GameObject POIAnchor;

        /// <summary>
        /// Set an anchor according to the type of the object
        /// </summary>
        /// <param name="type">The type of the Anchor</param>
        public void SetAnchor(VertexType type)
        {
            GameObject new_object;
            switch(type)
            {
                case VertexType.DOOR:
                    new_object = Instantiate(DoorAnchor);
                    break;
                case VertexType.CORNER:
                    new_object = Instantiate(CornerAnchor);
                    break;
                default:
                    new_object = Instantiate(POIAnchor);
                    break;
            }
            new_object.transform.SetParent(transform);
            new_object.transform.position = transform.position;
        }

        void Start()
        {
            id = m_maxid++;
        }

        void Update()
        {
            Vector3 cur_position = Frame.Pose.position;
            Vector2 proj_cur_position = new Vector2(cur_position.x, cur_position.z);

            Vector2 proj_vertex_position = new Vector2(transform.position.x, transform.position.z);

            if(id == 0) {
                Debug.Log("proj_vertex_position: " + proj_vertex_position);
            }

            if (Vector2.Distance(proj_vertex_position, proj_cur_position) < 0.2f)
            {
                MainARController.Log("Reached destination " + id);
                Destroy(gameObject);
            }
        }
    }


}
