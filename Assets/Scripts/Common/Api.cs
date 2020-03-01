using UnityEngine;
using UnityEngine.Networking;
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

        public static IEnumerator GetScene(int sceneId, System.Action<string> callback)
        {
            UnityWebRequest request = UnityWebRequest.Get($"{ServerURL}/scenes/{sceneId}");

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                // Show results as text
                yield return null;
                callback(request.downloadHandler.text);
            }

        }

    }
}