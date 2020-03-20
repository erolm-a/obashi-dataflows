using UnityEngine;
using UnityEditor;

namespace DataFlows.Commons
{

/// <summary>
/// Small utility function to support automatic box collider fitting inside Unity Editor
/// Taken from here: http://answers.unity.com/answers/216129/view.html
/// </summary>
public class ColliderToFit : MonoBehaviour {

    [MenuItem("DataFlows Tools/Collider/Fit to Children")]
    static void FitToChildren() {
        foreach (GameObject rootGameObject in Selection.gameObjects) {
            if (!(rootGameObject.GetComponent<Collider>() is BoxCollider))
                continue;

            bool hasBounds = false;
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

            for (int i = 0; i < rootGameObject.transform.childCount; ++i) {
                Renderer childRenderer = rootGameObject.transform.GetChild(i).GetComponent<Renderer>();
                if (childRenderer != null) {
                    if (hasBounds) {
                        bounds.Encapsulate(childRenderer.bounds);
                    }
                    else {
                        bounds = childRenderer.bounds;
                        hasBounds = true;
                    }
                }
            }

            BoxCollider collider = (BoxCollider)rootGameObject.GetComponent<Collider>();
            collider.center = bounds.center - rootGameObject.transform.position;
            collider.size = bounds.size;
        }
    }

}

}
