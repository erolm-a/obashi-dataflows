using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataFlows.Commons;

namespace DataFlows
{
    public class LoadSceneList : MonoBehaviour
    {
        /// <summary>
        /// A button to create for each scene
        /// </summary>
        public GameObject Button;

        void OnScenesFetched(List<SceneInfo> scenes)
        {
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