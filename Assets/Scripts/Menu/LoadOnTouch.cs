using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DataFlows
{
    public class LoadOnTouch : MonoBehaviour
    {
        /// <summary>
        /// Scene to go to when touched
        /// </summary>
        public int sceneBuildIndex;

        [HideInInspector]
        public SerializableFlowGraph sceneInfo;

        private IEnumerator LoadSceneAsync()
        {
            Debug.Log("Debugging scene: " + sceneBuildIndex);
            MainARController.instance.currentScene = sceneInfo;

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Single);

            while (!asyncLoad.isDone)
                yield return null;

            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneBuildIndex));
        }

        void Start()
        {
            var text = GetComponentInChildren<Text>();
            text.text = sceneInfo.name;
        }

        public void OnClick()
        {
            StartCoroutine(LoadSceneAsync());
        }
    }

}