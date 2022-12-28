using Mach3_netframework.MACH3;
using Microsoft.Xaml.Behaviors.Core;
using ProfileCutter.Model.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProfileCutter.Model.MACH3
{
    public class ToggleModel : ModelObject, ITurnObject
    {
        public bool IsTurn => Status == ToggleStatus.RUNING;
        public ToggleStatus Status
        {
            get
            {
                ToggleStatus result = status;
                if (Sensors != null && Sensors.Any(e => e.Detect == InverseSensor) == true)
                    result = ToggleStatus.RUNING;
                return result;
            }
            set
            {
                status = value;
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(IsTurn));
            }

        }
        private ToggleStatus status;

        private bool InverseSensor { get; }
        private IEnumerable<SensorModel> Sensors { get; set; }

        public int Delay { get; set; } = 0;

        private Mach3Toggle Mach3Toggle { get; }

        public ToggleModel(Mach3Toggle mach3Toggle, IEnumerable<SensorModel> sensors, bool inverseSensor, int delay)
        {
            this.Delay = delay;
            this.Mach3Toggle = mach3Toggle;
            InverseSensor = inverseSensor;
            this.Sensors = sensors;
            if (this.Sensors != null)
            {
                foreach (SensorModel sensor in sensors)
                {
                    sensor.StatusChanged += Sensor_StatusChanged;
                }
            }
        }

        private void Sensor_StatusChanged(object sender, bool e) => OnPropertyChanged(nameof(Status));

        public ICommand ToggleCommand => new ActionCommand(() =>
        {
            if (this.Status == ToggleStatus.STADY)
            {
                this.Run();
            }
            else
            {
                this.Stop();
            }
        });

        public async void Run()
        {
            Status = ToggleStatus.READY;
            await Task.Run(() => {
                this.Mach3Toggle.On(this.Delay);
            });
            Status = ToggleStatus.RUNING;
        }
        public void Stop()
        {
            this.Mach3Toggle.Off();
            Status = ToggleStatus.STADY;
        }
    }

    public enum ToggleStatus
    {
        STADY,
        READY,
        RUNING
    }
}
