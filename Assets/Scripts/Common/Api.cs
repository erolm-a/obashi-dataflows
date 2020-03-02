using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DataFlows.Commons
{
    [Serializable]
    public class SceneInfo
    {
        public int id;
        public string name;
    }

    /// <summary>
    /// HACK: wrap a list of scenes in a single scene.
    /// This is required as Unity's JsonUtility does not let us have root-level arrays.
    /// </summary>
    [Serializable]
    class SceneRootWrapper
    {
        public SceneInfo[] scenes;
    }

    /// <summary>
    /// A container for all the API calls to the middleware
    /// </summary>
    public static class Api
    {
        public static readonly string ServerURL = "https://glasgow-cs25-middleware.herokuapp.com";

        /// <summary>
        /// Factory method to create an HTTP request for JSON.
        /// </summary>
        /// <param name="content">The payload (as a string) to send</param>
        /// <param name="method">A HTTP verb, such as GET, POST, PUT etc.</param>
        /// <returns>A UnityWebRequest</returns>
        public static UnityWebRequest MakeRequest(string content, string method)
        {
            UnityWebRequest request = new UnityWebRequest(ServerURL + "/scenes/", method);
            byte[] payload = System.Text.Encoding.UTF8.GetBytes(content);

            request.SetRequestHeader("cache-control", "no-cache");
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(payload);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            return request;
        }

        /// <summary>
        /// Get a scene from the middleware
        /// TODO: error handling
        /// </summary>
        /// <param name="sceneId">The id of the scene</param>
        /// <param name="callback">A callback function to call once a the payload has been fetched.</param>
        /// <returns>A coroutine</returns>
        public static IEnumerator GetScene(int sceneId, System.Action<string> callback)
        {
            UnityWebRequest request = UnityWebRequest.Get($"{ServerURL}/scenes/{sceneId}");

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                callback(request.downloadHandler.text);
                yield return null; // FIXME: is this actually needed?
            }
        }

        public static IEnumerator GetScenes(System.Action<SceneInfo[]> scenes)
        {
            UnityWebRequest request = UnityWebRequest.Get($"{ServerURL}/scenes/");

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                // TODO: Error handling
                scenes(JsonUtility.FromJson<SceneRootWrapper>("{\"scenes\":" + request.downloadHandler.text + "}").scenes);
            }
            yield return null;
        }
    }
}