using System.Collections.Generic;
using UnityEngine;

public class MakeGraph : MonoBehaviour
{
    
    /// <summary>
    /// The "cord flare" prefab to instantiate
    /// </summary>
    public GameObject cordFlare;
    /// <summary>
    /// The "vertex" to instantiate
    /// </summary>
    public GameObject topographicalVertex;

    private GameObject m_instantiated;

    private bool m_downloaded_adj_list;
    private Dictionary<int, List<int>> m_adjancency_list;

    // Path of vertices to follow
    private List<int> m_path;

    private void m_populate() {

        // Basic experiment for pulling all the objects
        // TODO: clean this up after demo!
        var v0 = Instantiate(topographicalVertex, new Vector3(0, -1.25f, 1), new Quaternion(0,0,0,0));
        var v1 = Instantiate(topographicalVertex, new Vector3(0, -1.25f, 2), new Quaternion(0,0,0,0));
        var v2 = Instantiate(topographicalVertex, new Vector3(1, -1.25f, 2), new Quaternion(0,0,0,0));

        v0.GetComponent<TopographicalVertex>().SetAnchor(TopographicalVertex.VertexType.POI);
        v1.GetComponent<TopographicalVertex>().SetAnchor(TopographicalVertex.VertexType.POI);
        v2.GetComponent<TopographicalVertex>().SetAnchor(TopographicalVertex.VertexType.POI);

        v0.transform.SetParent(transform);
        v1.transform.SetParent(transform);
        v2.transform.SetParent(transform);

        var new_cordflare = cordFlare.GetComponent<CordFlare>().create(v0, v1, CordFlare.OSIProtocolStackType.APPLICATION);
        new_cordflare.transform.SetParent(transform);
        new_cordflare = cordFlare.GetComponent<CordFlare>().create(v1, v2, CordFlare.OSIProtocolStackType.APPLICATION);
        new_cordflare.transform.SetParent(transform);
    }

    void Start()
    {
        m_populate();
    }
}
