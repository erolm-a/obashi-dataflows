﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LoadOnTouch : MonoBehaviour
{
    /// <summary>
    /// Scene to go to when touched
    /// </summary>
    public void Update() {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                SceneManager.LoadScene(1, LoadSceneMode.Single);
                SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
            }
        }
    }
}