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
            instantiatedTooltip.SetText(this.deviceName);
        }

        public override void OnUserSelect()
        {
            MainARController.Log("Picked a Switch!");
        }
    }
}
