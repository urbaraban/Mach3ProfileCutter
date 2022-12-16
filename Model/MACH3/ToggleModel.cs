using Mach3_netframework.MACH3;
using Microsoft.Xaml.Behaviors.Core;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ProfileCutter.Model.MACH3
{
    public class ToggleModel : ModelObject
    {
        public bool IsOn
        {
            get
            {
                bool result = false;
                if (Sensors != null) result = Sensors.Any(e => e.Detect == InverseSensor);
                else result = _isOn;
                return result;
            }
            set
            {
                _isOn = value;
                OnPropertyChanged(nameof(IsOn));
            }

        }
        private bool _isOn;

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

        private void Sensor_StatusChanged(object sender, bool e) => OnPropertyChanged(nameof(IsOn));

        public ICommand ToggleCommand => new ActionCommand(() =>
        {
            if (this._isOn == true)
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
            await this.Mach3Toggle.On(Delay);
            IsOn = true;
        }
        public void Stop()
        {
            this.Mach3Toggle.Off();
            IsOn = false;
        }
        }
    }
