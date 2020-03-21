using UnityEngine;

namespace DataFlows
{
    /// <summary>
    /// Manage a Server entity.
    /// </summary>
    public class Switch : Device
    {

        private int counter = 0;

        new void Start()
        {
            base.Start();
            this.deviceName = "Switch" + counter++;
            instantiatedTooltip.transform.localScale *= 0.5f;
            instantiatedTooltip.transform.localPosition -= new Vector3(0.0f, 0.5f, 0.0f);
            instantiatedTooltip.SetText(this.deviceName);
        }

        public override void OnUserSelect()
        {
            MainARController.Log("Picked a Switch!");
            this.infoCanvas.SetActive(true);
            this.infoCanvas.GetComponent<UI.DeviceInfoPanel>().PanelDevice = this;
        }
    }
}
