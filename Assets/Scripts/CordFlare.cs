using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CordFlare : MonoBehaviour
{
    
    /// <summary>
    /// Scrolling of the texture
    /// </summary>
    public float ScrollY = -0.8f;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // "Animate" the flare. This basically consists of moving the flare by a variable offset for every frame depending on time
        // the value of ScrollY dictates the speed of the flare.
        Vector2 scroll = new Vector2( 0, Time.time * ScrollY);
        GetComponent<Renderer>().material.mainTextureOffset = scroll;
    }
}
