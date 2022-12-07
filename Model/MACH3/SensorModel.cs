using Mach3_netframework.MACH3;

namespace ProfileCutter.Model.MACH3
{
    internal class SensorModel : ModelObject
    {
        public string Name { get; }
        public bool Detect => Poller.pins[Num] == Check;
        private Mach3SensorPoller Poller { get; }
        private int Num { get; }
        private byte Check { get; }

        public SensorModel(string name, Mach3SensorPoller sensor, int num, byte check) 
        {
            this.Name = name;
            this.Poller = sensor;
            this.Num = num;
            this.Check = check;
        }

        private void Mach3Sensor_SensorChanged(object sender, bool e)
        {
            OnPropertyChanged(nameof(Detect));
        }
    }
}
