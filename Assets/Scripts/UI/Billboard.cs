using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataFlows.UI
{
    /// <summary>
    /// Make a billboard
    /// </summary>
    public class Billboard : MonoBehaviour
    {
        public TextMesh textMesh;

        void Awake()
        {
            textMesh = GetComponentInChildren<TextMesh>();
        }

        /// <summary>
        /// Set the billboard text
        /// </summary>
        public void SetText(string text)
        {
            textMesh.text = text;
            // TODO: add scaling
        }

        void Update()
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}
