using UnityEngine;
using GoogleARCore;

public class CordFlare : MonoBehaviour
{

    /// <summary>
    /// Scrolling of the texture
    /// </summary>
    public float ScrollY = -0.8f;

    /// <summary>
    /// Each cord could use a different cord material according to the transport protocol.
    /// For example, we might expect that a cord between switches could be different from a cord between a router and a l
    /// </summary>
    public (GameObject, GameObject) endpoints;

    /// <summary>
    /// Create a cord between two points and perform proper scaling.
    /// The cord will be oriented to look at the second object.
    /// </summary>
    /// <param name="p1">The first point</param>
    /// <param name="p2">The second point</param>
    /// <returns>The newly instantiated cord</returns>
    public GameObject create(GameObject p1, GameObject p2)
    {
        GameObject result = Instantiate(this.gameObject, Vector3.Lerp(p1.transform.position, p2.transform.position, 0.5f), Quaternion.identity);
        CordFlare script = result.GetComponent<CordFlare>();
        script.endpoints.Item1 = p1;
        script.endpoints.Item2 = p2;

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
        Vector2 scroll = new Vector2(0, Time.time * ScrollY);
        GetComponent<Renderer>().material.mainTextureOffset = scroll;
    }

    void Update()
    {
        m_scroll();
    }
}
