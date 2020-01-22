using GoogleARCore;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class MainARController : MonoBehaviour
{
    private bool m_IsQuitting;

    int frameCounter;

    void Start()
    {
        m_IsQuitting = false;
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
    }

    void Update()
    {
        _UpdateApplicationLifecycle();
        
        // TODO: add more logic
    }

    /// <summary>
    /// Check and update the application lifecycle.
    /// </summary>
    private void _UpdateApplicationLifecycle()
    {
        // Exit the app when the 'back' button is pressed.
        if (Input.GetKey(KeyCode.Escape))
        {
            if (this.gameObject.scene.name == "Menu")
            {
                Application.Quit();
            }
            else
            {
                SceneManager.LoadScene("Menu", LoadSceneMode.Single);
            }
            
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
            ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            ShowAndroidToastMessage(
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
    [ObsoleteAttribute("Using Android toasts breaks the UI and is not portable. A Unity UI-based approach to be done soon")]
    public static void ShowAndroidToastMessage(string message)
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

    /// <summary>project~
    /// Log (temporary method) that is going to be removed soom
    /// </summary>
    /// <param name="message">The message to print</param>
    public static void Log(string message)
    {
        ShowAndroidToastMessage(message);   
    }

}
