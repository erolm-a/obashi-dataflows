using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainARController : MonoBehaviour
{
    private bool m_IsQuitting;
    private Vector3 m_PrevPosition;

    //public Camera firstPersonCamera;
    public Camera mappingCamera;
    public GameObject player;

    int frameCounter;

    // Start is called before the first frame update
    void Start()
    {
        m_IsQuitting = false;
        m_PrevPosition = Vector3.zero;
        frameCounter = 0;
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
    }

    // Update is called once per frame
    void Update()
    {
        _UpdateApplicationLifecycle();

        Vector3 curPosition = Frame.Pose.position;
        if (Session.Status != SessionStatus.Tracking)
        {
            m_PrevPosition = curPosition;
        }
        else
        {
            var deltaPosition = curPosition - m_PrevPosition;
            player.transform.Translate(deltaPosition.x, 0, deltaPosition.z);
            
            m_PrevPosition = curPosition;
            frameCounter++;

            if (frameCounter % 10 == 0)
            {
                Debug.Log($"{curPosition.x} {curPosition.y} {curPosition.z}");
                Debug.Log($"Player position: {player.transform.position}");
            }
        }
        
    }

    /// <summary>
    /// Check and update the application lifecycle.
    /// </summary>
    private void _UpdateApplicationLifecycle()
    {
        // Exit the app when the 'back' button is pressed.
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        // Only allow the screen to sleep when not tracking.
        if (Session.Status != SessionStatus.Tracking)
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        if (m_IsQuitting)
        {
            return;
        }

        // Quit if ARCore was unable to connect and give Unity some time for the toast to
        // appear.
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            _ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            _ShowAndroidToastMessage(
                "ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
    }

    /// <summary>
    /// Actually quit the application.
    /// </summary>
    private void _DoQuit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject =
                    toastClass.CallStatic<AndroidJavaObject>(
                        "makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }

}
