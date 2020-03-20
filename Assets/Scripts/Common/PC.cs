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
            instantiatedTooltip.SetText(deviceName);
        }

        public override void OnUserSelect()
        {
            MainARController.Log($"Picked the PC {this.deviceName} (id ${this.deviceId} )!");
        }
    }
}
