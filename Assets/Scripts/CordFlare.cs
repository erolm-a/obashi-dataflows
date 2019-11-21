using UnityEngine;
using GoogleARCore;

public class CordFlare : MonoBehaviour
{
    
    /// <summary>
    /// Scrolling of the texture
    /// </summary>
    public float ScrollY = -0.8f;

    /// <summary>
    /// </summary>
    public enum OSIProtocolStackType {
        DATA_FRAME,
        NETWORK,
        TRANSPORT,
        APPLICATION
    }

    /// <summary>
    /// Each cord could use a different cord material according to the transport protocol.
    /// For example, we might expect that a cord between switches could be different from a cord between a router and a l
    /// </summary>
    public OSIProtocolStackType protocolType;

    private (Vector2, Vector2) m_endpoints;
    private (bool, bool) m_visitedEndpoints = (false, false);

    /// <summary>
    /// Create a cord between two points and perform proper scaling.
    /// The cord will be oriented to look at the second object.
    /// </summary>
    /// <param name="p1">The first point</param>
    /// <param name="p2">The second point</param>
    /// <returns>The newly instantiated cord</returns>
    public GameObject create(GameObject p1, GameObject p2, OSIProtocolStackType type) {
        GameObject result = Instantiate(this.gameObject, Vector3.Lerp(p1.transform.position, p2.transform.position, 0.5f), new Quaternion(0,0,0,0));

        CordFlare script = result.GetComponent<CordFlare>();
        script.m_endpoints.Item1 = new Vector2(p1.transform.position.x, p1.transform.position.z);
        script.m_endpoints.Item2 = new Vector2(p2.transform.position.x, p2.transform.position.z);
        
            
        result.transform.LookAt(p2.transform);
        result.transform.Rotate(90f, 0, 0);
        float distance = Vector3.Distance(p1.transform.position, p2.transform.position);
        result.transform.localScale = new Vector3(0.05f, distance / 2, 0.05f);


        return result;
    }

    /// <summary>
    /// "Animate" the flare. This basically consists of moving the flare by a variable offset for every frame depending on time
    /// the value of ScrollY dictates the speed of the flare.
    /// </summary>
    private void m_scroll()
    {
        Vector2 scroll = new Vector2( 0, Time.time * ScrollY);
        GetComponent<Renderer>().material.mainTextureOffset = scroll;
    }

    /// <summary>
    /// Check if both the endpoints have been visited. If yes, delete the object.
    /// </summary>
    private void m_updateEndpoints()
    {
        Vector3 cur_position = Frame.Pose.position;
        Vector2 projected_position = new Vector3(cur_position.x, cur_position.z);

        if (Vector2.Distance(projected_position, m_endpoints.Item1) < 0.2f)
            m_visitedEndpoints.Item1 = true;

        if (Vector2.Distance(projected_position, m_endpoints.Item2) < 0.2f)
            m_visitedEndpoints.Item2 = true;
        
        if (m_visitedEndpoints.Item1 && m_visitedEndpoints.Item2)
            Destroy(gameObject);

    }

    void Update()
    {
        m_scroll();
        m_updateEndpoints();
    }
}
