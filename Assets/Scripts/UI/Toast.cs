using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DataFlows
{
    /// <summary>
    /// Implement an OS-agnostic toast message. Abridged from https://stackoverflow.com/a/52592601
    /// </summary>
    public class Toast : MonoBehaviour
    {
        private Text text;
        /// <summary>
        /// Manipulate alpha values
        /// </summary>
        private CanvasGroup canvasGroup;
        /// <summary>
        /// Used to enable/disable the canvas at all
        /// </summary>
        private Canvas canvas;

        private IEnumerator ShowToastCoroutine(string text, float duration)
        {
            Color orginalColor = this.text.color;

            this.text.text = text;
            this.canvas.enabled = true;

            // Fade in
            yield return FadeInAndOut(true, 0.2f);

            // Wait for the duration
            float counter = 0;
            while (counter < duration)
            {
                counter += Time.deltaTime;
                yield return null;
            }

            //Fade out
            yield return FadeInAndOut(false, 0.2f);

            this.canvas.enabled = false;
        }


        private IEnumerator FadeInAndOut(bool fadeIn, float duration)
        {
            float a, b;
            if (fadeIn)
            {
                a = 0.0f;
                b = 1.0f;
            }
            else
            {
                a = 1.0f;
                b = 0.0f;
            }

            canvasGroup.alpha = a;
            float curTime = 0.0f;
            yield return null;

            while (curTime < duration)
            {
                curTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(a, b, curTime / duration);
                yield return null;
            }

            canvasGroup.alpha = b;
        }

        /// <summary>
        /// Display a toast message.
        /// </summary>
        /// <param name="text">The text to show</param>
        /// <param name="duration">The amount of time, in seconds, to display the toast message for</param>
        public void ShowToast(string text, float duration)
        {
            StartCoroutine(ShowToastCoroutine(text, duration));
        }

        void Awake()
        {
            text = GetComponentInChildren<Text>();
            canvasGroup = GetComponent<CanvasGroup>();
            canvas = GetComponent<Canvas>();
        }
    }

}