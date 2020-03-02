using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataFlows.Commons;

namespace DataFlows
{
    public class LoadSceneList : MonoBehaviour
    {
        /// <summary>
        /// A button to create for each scene to load
        /// </summary>
        public GameObject Button;

        void OnScenesFetched(SceneInfo[] scenes)
        {
            Debug.Log(scenes);
            foreach (SceneInfo scene in scenes)
            {
                var newButton = Instantiate(Button, Vector3.zero, Quaternion.identity);
                newButton.transform.SetParent(transform);
                newButton.SetActive(true);
                var textComponent = newButton.GetComponentInChildren<Text>();
                textComponent.text = scene.name;
            }
        }
        void Start()
        {
            StartCoroutine(Api.GetScenes(OnScenesFetched));
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}