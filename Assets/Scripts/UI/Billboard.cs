using UnityEngine;

namespace DataFlows.UI
{
    /// <summary>
    /// Make a billboard
    /// </summary>
    public class Billboard : MonoBehaviour
    {
        private TextMesh textMesh;

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

        void LateUpdate()
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}
