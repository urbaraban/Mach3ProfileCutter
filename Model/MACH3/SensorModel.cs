using Mach3_netframework.MACH3;
using System;
using System.Threading;

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
        private int Check { get; }

        public SensorModel(string name, Mach3SensorPoller sensor, int num, int check) 
        {
            this.Name = name;
            this.Poller = sensor;
            this.Num = num;
            this.Check = check;

            this.Poller.UpdateSensor += Poller_UpdateSensor;
        }

        private void Poller_UpdateSensor(object sender, int e)
        {
            if (e == this.Num)
            {
                if (this.Detect != lastdetect)
                {
                    OnPropertyChanged(nameof(Detect));
                    lastdetect = this.Detect;
                    StatusChanged?.Invoke(this, this.Detect);
                    Thread.Sleep(1);
                }
            }
        }
    }
}
