using UnityEngine;
using UnityEngine.UI;

namespace DataFlows.UI
{
public class DeviceInfoPanel : MonoBehaviour
{

    private Device device;
    [HideInInspector]
    public Device PanelDevice
    {
        get
        {
            return device;
        }
        set
        {
            this.device = value;
            this.deviceNameText.text = value.deviceName;
            this.deviceTypeText.text = value.deviceType.ToString();
        }
    }

    public Text deviceNameText;
    public Text deviceTypeText;
    public Button okButton;

    void Start()
    {
        okButton.onClick.AddListener(() => {gameObject.SetActive(false);});
    }
}
}
