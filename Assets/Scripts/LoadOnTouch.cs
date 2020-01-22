using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LoadOnTouch : MonoBehaviour
{
    /// <summary>
    /// Scene to go to when touched
    /// </summary>
    public int sceneBuildIndex;

    private IEnumerator LoadSceneAsync()
    {
        Debug.Log("Debugging scene: " + sceneBuildIndex);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Single);

        while (!asyncLoad.isDone)
            yield return null;
        
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneBuildIndex));
    }

    public void Update() {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && EventSystem.current.currentSelectedGameObject == gameObject)
            {
                StartCoroutine(LoadSceneAsync());
            }
        }
    }
}
