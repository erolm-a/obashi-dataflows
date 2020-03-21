using UnityEngine;

namespace DataFlows
{
    /// <summary>
    /// Manage a PC entity.
    /// A PC
    /// </summary>
    public class PC : Device
    {
        private int counter = 0;

        new void Start()
        {
            base.Start();
            this.deviceName = "PC" + counter++;
            instantiatedTooltip.transform.localPosition += new Vector3(0.0f, 0.5f, 0.0f);
            instantiatedTooltip.SetText(deviceName);
        }

        public override void OnUserSelect()
        {
            MainARController.Log($"Picked the PC {this.deviceName} (id ${this.deviceId} )!");
            this.infoCanvas.SetActive(true);
            this.infoCanvas.GetComponent<UI.DeviceInfoPanel>().PanelDevice = this;
        }
    }
}
