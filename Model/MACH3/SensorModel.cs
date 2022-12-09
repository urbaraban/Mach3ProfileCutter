using Mach3_netframework.MACH3;
using System;

namespace ProfileCutter.Model.MACH3
{
    public class SensorModel : ModelObject
    {
        public event EventHandler<bool> StatusChanged;
        public string Name { get; }
        public bool Detect => Poller.pins[Num] == Check;
        private bool lastdetect = false;
        private Mach3SensorPoller Poller { get; }
        private int Num { get; }
        private byte Check { get; }

        public SensorModel(string name, Mach3SensorPoller sensor, int num, byte check) 
        {
            this.Name = name;
            this.Poller = sensor;
            this.Num = num;
            this.Check = check;

            this.Poller.UpdateSensor += Poller_UpdateSensor;
        }

        private void Poller_UpdateSensor(object sender, System.EventArgs e)
        {
            OnPropertyChanged(nameof(Detect));
            if (this.Detect != lastdetect)
            {
                lastdetect = this.Detect;
                StatusChanged?.Invoke(this, this.Detect);
            }
        }
    }
}
