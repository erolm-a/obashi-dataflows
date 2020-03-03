using UnityEngine;
using DataFlows.Commons;

namespace DataFlows
{
    public class LoadSceneList : MonoBehaviour
    {
        /// <summary>
        /// A button to create for each scene to load
        /// </summary>
        public GameObject Button;

        void OnScenesFetched(SerializableFlowGraph[] scenes)
        {
            Debug.Log(scenes);
            foreach (SerializableFlowGraph scene in scenes)
            {
                Debug.Log($"Found scene name: {scene.name}, id: {scene.id}");
                var newButton = Instantiate(Button, Vector3.zero, Quaternion.identity);
                newButton.transform.SetParent(transform);
                var loadOnTouch = newButton.GetComponent<LoadOnTouch>();
                loadOnTouch.sceneInfo = scene;
                newButton.SetActive(true);
            }
        }

        void Start()
        {
            StartCoroutine(Api.GetScenes(OnScenesFetched));
        }
    }

}