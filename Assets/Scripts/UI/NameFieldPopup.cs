using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace DataFlows
{
    public class NameFieldPopup : MonoBehaviour
    {
        public Button OKButton;
        public Button CancelButton;

        public Text PopupTitle;
        public Text PopupContent;

        public InputField InputField;

        public GameObject panel;

        public static NameFieldPopup Instance {get; private set;}

        void Awake()
        {
            if(Instance != null)
            {
                Debug.LogError("Multiple NameFieldPopup definitions here!");
                Destroy(this.gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        /// <summary>
        /// Setup the popup.
        /// </summary>
        /// <param name="title">The title of the popup</param>
        /// <param name="content">The content of the popup</param>
        /// <param name="confirmedAction">The action to perform when the OK button is pressed</param>
        /// <param name="cancelledAction">The action to perform when the Cancel button is pressed</param>
        public void Setup(string title, string content, UnityAction<string> confirmedAction, UnityAction cancelledAction)
        {
            PopupTitle.text = title;
            PopupContent.text = content;

            Debug.Log(panel);

            panel.SetActive(true);

            OKButton.onClick.RemoveAllListeners();
            OKButton.onClick.AddListener(() => {confirmedAction(InputField.text); panel.SetActive(false); } );

            CancelButton.onClick.RemoveAllListeners();
            CancelButton.onClick.AddListener(() => {cancelledAction(); panel.SetActive(false); });
        }
    }
}
