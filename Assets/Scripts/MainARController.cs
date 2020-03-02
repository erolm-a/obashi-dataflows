using GoogleARCore;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace DataFlows
{
    public class MainARController : MonoBehaviour
    {
        private bool m_IsQuitting;

        // Just a singleton
        private static MainARController instance;
        private Toast toast;

        void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("Trying to instantiate MainARController twice!");
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                toast = GetComponentInChildren<Toast>();
                Debug.Log(toast);
                DontDestroyOnLoad(gameObject);
            }
        }

        void Start()
        {
            m_IsQuitting = false;
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        }

        void Update()
        {
            _UpdateApplicationLifecycle();
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
                Log("Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                Log("ARCore encountered a problem connecting.  Please start the app again.");
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

        /// <summary>project~
        /// Log (temporary method) that is going to be removed soom
        /// </summary>
        /// <param name="message">The message to print</param>
        public static void Log(string message)
        {
            // ShowAndroidToastMessage(message);
            instance.toast.ShowToast(message, 3.0f);
        }

    }
}