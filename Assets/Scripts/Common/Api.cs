using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

namespace DataFlows.Commons
{
    /// <summary>
    /// A container for all the API calls to the middleware
    /// </summary>
    public static class Api
    {
        public static readonly string ServerURL = "https://glasgow-cs25-middleware.herokuapp.com";

        /// <summary>
        /// Factory method to create an HTTP request for JSON.
        /// It is recommended use the overloaded version `MakeRequest(string content)` for GET requests.
        /// </summary>
        /// <param name="content">The payload (as a string) to send</param>
        /// <param name="restFunction">The rest function to call, e.g. "/scenes"</param>
        /// <param name="method">A HTTP verb, such as GET, POST, PUT etc.</param>
        /// <returns>A UnityWebRequest</returns>
        private static UnityWebRequest MakeRequest(string content, string restFunction, string method)
        {
            UnityWebRequest request = new UnityWebRequest(ServerURL + restFunction, method);
            byte[] payload = System.Text.Encoding.UTF8.GetBytes(content);

            request.SetRequestHeader("cache-control", "no-cache");
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(payload);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            return request;
        }

        /// <summary>
        /// Factory method to create an HTTP GET request.
        /// </summary>
        /// <param name="restFunction">The rest function to call, e.g. "/scenes"</param>
        /// <returns>A UnityWebRequest</returns>
        private static UnityWebRequest MakeRequest(string restFunction)
        {
            UnityWebRequest request = UnityWebRequest.Get(ServerURL + restFunction);
            Debug.Log($"Making GET request to {request.url}");

            return request;
        }

        /// <summary>
        /// Save an existing scene or create a new one.
        /// </summary>
        /// <param name="flowGraph">The FlowGraph to serialize</param>
        /// <param name="callback">The action to perform</param>
        /// <param name="newScene">If true, save this as a new scene, otherwise replace an existing scene with the given id</param>
        /// <returns>An IEnumerator to be used for a Unity Coroutine</returns>
        public static IEnumerator SaveScene(FlowGraph flowGraph, System.Action<SerializableFlowGraph> callback, bool newScene = true)
        {
            string serialized = SerializableFlowGraph.Serialize(flowGraph);
            var restCall = "/scenes/";

            if (newScene)
            {
                restCall += $"{flowGraph.id}/";
            }

            var request = MakeRequest(serialized, restCall, "POST");
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                callback(SerializableFlowGraph.Deserialize(request.downloadHandler.text));
            }
        }

        /// <summary>
        /// Get a scene from the middleware
        /// </summary>
        /// <param name="sceneId">The id of the scene</param>
        /// <param name="callback">A callback function to call once a the payload has been fetched.</param>
        /// <returns>A coroutine handler</returns>
        public static IEnumerator GetScene(int sceneId, System.Action<SerializableFlowGraph> callback)
        {
            UnityWebRequest request = MakeRequest($"/scenes/{sceneId}");

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                // TODO: Error handling
                callback(SerializableFlowGraph.Deserialize(request.downloadHandler.text));
            }
        }

        /// <summary>
        /// Get a list of scenes
        /// </summary>
        /// <param name="callback">Function to call when the coroutine has completed</param>
        /// <returns>A couroutine handler</returns>
        public static IEnumerator GetScenes(System.Action<SerializableFlowGraph[]> callback)
        {
            UnityWebRequest request = MakeRequest("/scenes/");

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                // TODO: Error handling
                callback(SerializableFlowGraph.DeserializeFromList(request.downloadHandler.text));
            }
            yield return null;
        }
    }
}