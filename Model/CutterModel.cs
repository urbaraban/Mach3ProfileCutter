using Mach3_netframework.MACH3;
using Microsoft.Xaml.Behaviors.Core;
using ProfileCutter.Model.MACH3;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProfileCutter.Model
{
    internal class CutterModel : ModelObject
    {
        private Mach3 Mach3 = new Mach3();

        public ToggleModel Saw { get; }
        public ToggleModel Press { get; }

        public AxisModel X { get; }
        public AxisModel Y { get; }
        public AxisModel Z { get; }

        public List<SensorModel> Sensors { get; } = new List<SensorModel>();

        public CutterModel() 
        {
            Sensors.Add(new SensorModel("Пр2", Mach3.SensorPoller, 7, 1));
            Sensors.Add(new SensorModel("Пр1", Mach3.SensorPoller, 4, 0));
            Sensors.Add(new SensorModel("X", Mach3.SensorPoller, 5, 0));
            Sensors.Add(new SensorModel("Y", Mach3.SensorPoller, 3, 0));
            Sensors.Add(new SensorModel("Z", Mach3.SensorPoller, 6, 0));

            this.Saw = new ToggleModel(Mach3.Spindle, null);

            this.X = new AxisModel("X", Mach3.X);
            this.Y = new AxisModel("Y", Mach3.X);
            this.Z = new AxisModel("Z", Mach3.X);
        }

        public ICommand StopCommand => new ActionCommand(() => this.Mach3.IsTurn = false);

        public ICommand UpCommand => new ActionCommand(async () => await TryMoveAxis(Y, 0));
        public ICommand DownCommand => new ActionCommand(async () => await TryMoveAxis(Y, this.Y.MaxPosition));
        public ICommand LeftCommand => new ActionCommand(async () => await TryMoveAxis(X, this.X.MaxPosition));
        public ICommand RightCommand => new ActionCommand(async () => await TryMoveAxis(X, 0));

        public ICommand SawUp => new ActionCommand(async () => await TryMoveAxis(Z, this.Z.MaxPosition));
        public ICommand SawDown => new ActionCommand(async () => await TryMoveAxis(Z, 0));

        public async Task TryMoveAxis(AxisModel axis, double newposition)
        {
            await Task.Run(() => { 
                this.Mach3.IsTurn = true;
                axis.GoToPosition(newposition);
            });
        }
    }
}
