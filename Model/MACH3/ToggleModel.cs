using Mach3_netframework.MACH3;
using Microsoft.Xaml.Behaviors.Core;
using System.Windows.Input;

namespace ProfileCutter.Model.MACH3
{
    public class ToggleModel : ModelObject
    {
        public bool IsOn => Sensor != null && Sensor.Detect == true;
        private SensorModel Sensor { get; set; }

        public int Delay { get; set; } = 0;

        private Mach3Toggle Mach3Toggle { get; }

        public ToggleModel(Mach3Toggle mach3Toggle, SensorModel sensor)
        {
            this.Sensor = sensor;
            this.Mach3Toggle = mach3Toggle;
        }

        public ICommand RunCommand => new ActionCommand(() => this.Run());

        public ICommand StopCommand => new ActionCommand(() => this.Stop());

        public async void Run()
        {
            await this.Mach3Toggle.On(Delay);
        }
        public void Stop() => this.Mach3Toggle.Off();
    }
}
