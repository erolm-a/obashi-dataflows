using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var meshes = GetComponentsInChildren<MeshFilter>();

        Debug.Log($"We have {meshes.Length} objects");

        foreach(var mesh in meshes)
        {
            mesh.gameObject.AddComponent<cakeslice.Outline>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
