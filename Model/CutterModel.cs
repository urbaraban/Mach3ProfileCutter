using Mach3_netframework.MACH3;
using Microsoft.Xaml.Behaviors.Core;
using ProfileCutter.Configuration;
using ProfileCutter.Model.MACH3;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;


namespace ProfileCutter.Model
{
    public class CutterModel : ModelObject
    {
        public AxisModel X { get; }
        public AxisModel Y { get; }
        public AxisModel Z { get; }

        public ToggleModel Saw { get; }
        public ToggleModel Press { get; }

        public byte SensorsReqest
        {
            get => _sensorrequest;
            set
            {
                _sensorrequest = value;
                OnPropertyChanged(nameof(SensorsReqest));
            }
        }
        private byte _sensorrequest;

        private Mach3 Mach3;
        public List<SensorModel> Sensors { get; } = new List<SensorModel>();

        public ObservableCollection<string> LogsMessages { get; } = new ObservableCollection<string>();

        public CutterModel() 
        {
            this.Mach3 = new Mach3(LogMessage);
            Thread.Sleep(10);

            Mach3.SensorPoller.UpdateRequest += SensorPoller_UpdateRequest;
            Sensors.Add(new SensorModel("Пр1", Mach3.SensorPoller, 3, 0));
            Sensors.Add(new SensorModel("Пр2", Mach3.SensorPoller, 4, 0));
            Sensors.Add(new SensorModel("X", Mach3.SensorPoller, 5, 0));
            Sensors.Add(new SensorModel("Y", Mach3.SensorPoller, 7, 1));
            Sensors.Add(new SensorModel("Z", Mach3.SensorPoller, 6, 0));

            this.Saw = new ToggleModel(Mach3.Spindle, null, false, 12000);
            this.Saw.Stop();
            this.Press = new ToggleModel(Mach3.Clamp, new SensorModel[] { Sensors[0], Sensors[1] }, true, 2000);
            if (this.Press.IsOn == true) 
                this.Press.Stop();

            this.X = new AxisModel("X", Mach3.X, Sensors[2], false);
            this.Y = new AxisModel("Y", Mach3.Y, Sensors[3], false);
            this.Z = new AxisModel("Z", Mach3.Z, Sensors[4], true, 300);

            Config.Load(this);
        }

        private void SensorPoller_UpdateRequest(object sender, byte e)
        {
            this.SensorsReqest = e;
            Thread.Sleep(10);
        }

        private void LogMessage(string message)
        {
            LogsMessages.Add(message);
            OnPropertyChanged(nameof(LogsMessages));
        }

        public ICommand StopCommand => new ActionCommand(() =>
        {
            this.Saw.Stop();
            this.Mach3.IsTurn = false;
        });

        public ICommand UpCommand => new ActionCommand(async () => await TryMoveAxis(Y, 0));
        public ICommand DownCommand => new ActionCommand(async () => await TryMoveAxis(Y, this.Y.MaxPosition));
        public ICommand LeftCommand => new ActionCommand(async () => await TryMoveAxis(X, this.X.MaxPosition));
        public ICommand RightCommand => new ActionCommand(async () => await TryMoveAxis(X, 0));

        public ICommand SawUp => new ActionCommand(async () => await TryMoveAxis(Z, 0));
        public ICommand SawDown => new ActionCommand(async () => await TryMoveAxis(Z, this.Z.MaxPosition));

        public async Task TryMoveAxis(AxisModel axis, double newposition)
        {
            StopCommand.Execute(null);
            await Task.Delay(100);
            await Task.Run(() => { 
                this.Mach3.IsTurn = true;
                axis.GoToPosition(newposition);
            });
        }

        public ICommand SaveCommand => new ActionCommand(() =>
        {
            Config.Save(this);
        });

        public ICommand HomeCommand => new ActionCommand(async () =>
        {
            await Task.Run(() =>
            {
                StopCommand.Execute(null);
                this.Mach3.IsTurn = true;
                Z.GoHome();
                Y.GoHome();
                X.GoHome();
            });
        });
    }

    internal struct Profile
    {
        public string Name { get; set; }
        public double Length { get; set; }
        public double Height { get; set; }

        public Profile(string name, double length, double height)
        {
            this.Name = name;
            this.Length = length;
            this.Height = height;
        }
    }
}
