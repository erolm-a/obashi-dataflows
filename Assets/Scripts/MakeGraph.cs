using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using UnityEngine.EventSystems;

enum VertexType {
    /// <summary>
    /// Doors are obstacles that "stop" the path from prosecuting to another vertex
    /// </summary>
    DOOR,
    /// <summary>
    /// Corners make the path "curve" but do not necessarily stop it.
    /// TODO: in many cases, probably manually stopping a path like in a DOOR would be preferred
    /// </summary>
    CORNER,
    // Point of Interest
    POI
}

class Vertex {
    public int id {get;}
    public Pose pose;

    public GameObject pawn {get; set;}

    public VertexType type {get;}
    private static Dictionary<int, Vertex> m_vertices = new Dictionary<int, Vertex>();
    private static int max_id = 0;

    private Vertex(int id, Pose pose, VertexType type) {
        this.id = id;
        this.pose = pose;
        this.type = type;
    }

    public static Vertex getVertex(int id) {
        if(m_vertices.ContainsKey(id))
            return m_vertices[id];
        return null;
    }

    /// <summary>
    /// For testing purposes only.
    /// </summary>
    public static Vertex createVertex(Pose pose, VertexType type) {
        Vertex new_vertex  = new Vertex(max_id, pose, type);
        m_vertices[max_id++] = new_vertex;
        return new_vertex;
    }
}

public class MakeGraph : MonoBehaviour
{
    
    /// <summary>
    /// The Anchor to instantiate for POIs
    /// </summary>
    public GameObject POIAnchor;

    public MainARController controller;

    public GameObject cordFlare;

    private GameObject m_instantiated;

    private bool m_downloaded_adj_list;
    private Dictionary<int, List<int>> m_adjancency_list;

    // Path of vertices to follow
    private List<int> m_path;

    private void m_populate() {

        // Basic experiment for pulling all the objects
        // TODO: clean this up after demo!
        Vertex v0 = Vertex.createVertex(new Pose(new Vector3(0, -1.25f, 1), new Quaternion(0,0,0,0)), VertexType.CORNER);
        Vertex v1 = Vertex.createVertex(new Pose(new Vector3(0, -1.25f, 2), new Quaternion(0,0,0,0)), VertexType.POI);
        Vertex v2 = Vertex.createVertex(new Pose(new Vector3(1, -1.25f, 2), new Quaternion(0,0,0,0)), VertexType.POI);
        m_path = new List<int> {v0.id, v1.id, v2.id};

        foreach(int id in m_path)
        {
            Vertex v = Vertex.getVertex(id);
            v.pawn = Instantiate(POIAnchor, v.pose.position, v.pose.rotation);
            v.pawn.transform.SetParent(transform);
        }

        m_instantiated = Instantiate(cordFlare, Vector3.Lerp(v0.pose.position, v1.pose.position, 0.5f), new Quaternion(0,0,0,0));

        m_instantiated.transform.SetParent(transform);
        m_instantiated.transform.LookAt(v1.pawn.transform);
        m_instantiated.transform.Rotate(90f, 0f, 0f);
        float distance = Vector3.Distance(v0.pawn.transform.position, v1.pawn.transform.position);
        m_instantiated.transform.localScale = new Vector3(0.05f, distance/2, 0.05f);

        
    }


    private void m_updateVertex() {
        Pose cur_pos = Frame.Pose;
        if (m_path.Count == 0)
            return;

        Vertex next_vertex = Vertex.getVertex(m_path[0]);
        
        // _Log("Getting next vertex");
        
        Vector2 next_projected = new Vector2(next_vertex.pose.position.x,
                                             next_vertex.pose.position.z);
        Vector2 cur_projected = new Vector2(Frame.Pose.position.x, Frame.Pose.position.z);

        if (Vector2.Distance(cur_projected, next_projected) < 0.2) {
            _Log("Reached destination " + next_vertex.id);

            Destroy(next_vertex.pawn);
            m_path.RemoveAt(0);
        }
    }

    void Start()
    {
        m_downloaded_adj_list = false;
        m_populate();
        m_downloaded_adj_list = true;
    }

    void Update()
    {
        // Wait for pulling all the vertices
        if (!m_downloaded_adj_list)
            return;

        m_updateVertex();
    }

    private void _Log(string message)
    {
        controller.ShowAndroidToastMessage (message);
    }
}
