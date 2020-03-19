using UnityEngine;
using DataFlows.UI;

namespace DataFlows
{
    public class CordFlare : MonoBehaviour
    {

        /// <summary>
        /// Scrolling speed
        /// </summary>
        public float ScrollY = -0.8f;

        /// <summary>
        /// Each cord could use a different cord material according to the transport protocol.
        /// For example, we might expect that a cord between switches could be different from a cord between a router and a l
        /// </summary>
        [HideInInspector]
        public (GameObject, GameObject) endpoints;

        // The billboard of the cord
        private Billboard billboard;

        private Material meshMaterial;

        /// <summary>
        /// Create a cord between two points and perform proper scaling.
        /// The cord will be oriented to look at the second object.
        /// </summary>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <returns>The newly instantiated cord</returns>
        public GameObject Create(GameObject p1, GameObject p2)
        {
            GameObject result = Instantiate(this.gameObject, Vector3.Lerp(p1.transform.position, p2.transform.position, 0.5f), Quaternion.identity);
            CordFlare script = result.GetComponent<CordFlare>();
            // apply stretching to the cylinder only, i.e. to the gameobject that has a MeshFilter
            MeshFilter cordMesh = result.GetComponentInChildren<MeshFilter>();
            Billboard billboardScript = script.billboard;

            script.endpoints.Item1 = p1;
            script.endpoints.Item2 = p2;

            // Stretch the cord, not the tooltip
            GameObject toStretch = cordMesh.gameObject;
            float distance = Vector3.Distance(p1.transform.position, p2.transform.position);

            result.transform.LookAt(p2.transform);
            result.transform.Rotate(90f, 0, 0);

            toStretch.transform.localScale = new Vector3(0.05f, distance / 2, 0.05f);

            billboardScript.SetText(distance.ToString("n2"));
            return result;
        }

        void Awake()
        {
            meshMaterial = GetComponentInChildren<Renderer>().material;
            billboard = GetComponentInChildren<Billboard>();
        }

        /// <summary>
        /// </summary>
        void Update()
        {
            /// "Animate" the flare. This basically consists of moving the flare
            /// by a variable offset depending on time.
            Vector2 scroll = new Vector2(0, Time.time * ScrollY);
            meshMaterial.mainTextureOffset = scroll;
        }
    }
}
