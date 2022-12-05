using Mach3_netframework.MACH3;

namespace ProfileCutter.Model.MACH3
{
    internal class SensorModel : ModelObject
    {
        public string Name { get; }
        public bool Detect => this.Mach3Sensor.Detect;
        Mach3Sensor Mach3Sensor { get; }
        public SensorModel(string Name, Mach3Sensor sensor) 
        {
            this.Name = Name;
            this.Mach3Sensor = sensor;
            this.Mach3Sensor.SensorChanged += Mach3Sensor_SensorChanged;
        }

        private void Mach3Sensor_SensorChanged(object sender, bool e)
        {
            OnPropertyChanged(nameof(Detect));
        }
    }
}
