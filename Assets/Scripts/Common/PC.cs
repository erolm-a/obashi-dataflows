namespace DataFlows
{
    /// <summary>
    /// Manage a PC entity.
    /// A PC
    /// </summary>
    public class PC : Device
    {
        private int counter = 0;
        void Awake()
        {
            this.deviceName = "PC" + counter++;
        }

        public override void OnUserSelect()
        {
            MainARController.Log($"Picked the PC {this.deviceName} (id ${this.deviceId} )!");
        }
    }
}
