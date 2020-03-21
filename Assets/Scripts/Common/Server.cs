using UnityEngine;

namespace DataFlows
{
    /// <summary>
    /// Manage a Server entity.
    /// </summary>
    public class Server : Device
    {

        private int counter = 0;

        new void Start()
        {
            base.Start();
            this.deviceName = "Server" + counter++;
            instantiatedTooltip.transform.localPosition += new Vector3(0.0f, 1.5f, 0.0f);
            base.instantiatedTooltip.SetText(deviceName);
        }

        public override void OnUserSelect()
        {
            MainARController.Log("Picked a Server!");
            this.infoCanvas.SetActive(true);
            this.infoCanvas.GetComponent<UI.DeviceInfoPanel>().PanelDevice = this;
        }
    }
}
