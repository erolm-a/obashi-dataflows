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
            base.instantiatedTooltip.SetText(deviceName);
        }

        public override void OnUserSelect()
        {
            MainARController.Log("Picked a Server!");
        }
    }
}
