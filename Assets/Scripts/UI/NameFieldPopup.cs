using UnityEngine;
using UnityEngine.UI;

namespace DataFlows
{
    public class NameFieldPopup : MonoBehaviour
    {
        public Button OKButton;
        public Button CancelButton;

        public Text PopupTitle;
        public Text PopupContent;

        /// <summary>
        /// Setup the popup.
        /// </summary>
        /// <param name="title">The title of the popup</param>
        /// <param name="content">The content of the popup</param>
        public void Setup(string title, string content)
        {
            PopupTitle.text = title;
            PopupContent.text = content;
        }
    }

}