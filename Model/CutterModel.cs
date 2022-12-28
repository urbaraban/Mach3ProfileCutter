using Mach3_netframework.MACH3;
using Microsoft.Xaml.Behaviors.Core;
using ProfileCutter.Configuration;
using ProfileCutter.Model.Interfaces;
using ProfileCutter.Model.MACH3;
using ProfileCutter.Model.Programms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;


namespace ProfileCutter.Model
{
    public class CutterModel : ModelObject, ITurnObject
    {
        public delegate void StopAllDelegate();
        private StopAllDelegate AllStop { get; set; }

        public delegate bool TurnAxisDelegate();
        public bool IsTurn { get; private set; }

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

        public Mach3 Mach3 { get; }
        public List<SensorModel> Sensors { get; } = new List<SensorModel>();

        public CutConfigurationManager CutConfigs {get;} = new CutConfigurationManager();

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

            this.Saw = new ToggleModel(Mach3.Spindle, null, false, 12000000);
            this.Saw.Stop();
            this.Press = new ToggleModel(Mach3.Clamp, new SensorModel[] { Sensors[0], Sensors[1] }, true, 1000000);
            if (this.Press.Status == ToggleStatus.RUNING) 
                this.Press.Stop();

            this.X = new AxisModel("X", Mach3.X, Sensors[2], false, Press.Stop);
            this.Y = new AxisModel("Y", Mach3.Y, Sensors[3], false, Press.Run);
            this.Z = new AxisModel("Z", Mach3.Z, Sensors[4], true, null, 300);

            AllStop += Saw.Stop;
            AllStop += this.Stop;

            ReadWrite.Load(this);
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

        public ICommand StopCommand => new ActionCommand(() => AllStop?.Invoke());

        public ICommand UpCommand => new ActionCommand(async () => await TryMoveAxis(Y, 0));
        public ICommand DownCommand => new ActionCommand(async () => await TryMoveAxis(Y, this.Y.MaxPosition));
        public ICommand LeftCommand => new ActionCommand(async () => await TryMoveAxis(X, this.X.MaxPosition));
        public ICommand RightCommand => new ActionCommand(async () => await TryMoveAxis(X, 0));

        public ICommand SawUp => new ActionCommand(async () => await TryMoveAxis(Z, 0));
        public ICommand SawDown => new ActionCommand(async () => await TryMoveAxis(Z, CutConfigs.SelectConf.Height));

        public async Task TryMoveAxis(AxisModel axis, double newposition)
        {
            this.IsTurn = true;
            await Task.Delay(100);
            await Task.Run(async () => { 
                await axis.GoToPosition(newposition);
            });
        }

        public ICommand SaveCommand => new ActionCommand(() =>
        {
            ReadWrite.Save(this);
            foreach(CutConfiguration programm in this.CutConfigs.Configs)
            {
                programm.UpdateDisplay();
            }
            foreach (Profile profile in this.CutConfigs.Profiles)
            {
                profile.UpdateDisplay();
            }
        });

        public ICommand HomeCommand => new ActionCommand(async () =>
        {
            await Task.Run(() =>
            {
                StopCommand.Execute(null);
                this.IsTurn = true;
                Z.GoHome();
                Y.GoHome();
                X.GoHome();
            });
        });

        public ICommand RunProgrammCommand => new ActionCommand(async () => await StartProgramm(this.CutConfigs.SelectConf));

        public async Task<bool> StartProgramm(CutConfiguration configuration)
        {
            try
            {
                HomeCommand.Execute(null);
                configuration.IsRunning = true;
                configuration.StepActual = configuration.StepActual == 0 ? 1 : configuration.StepActual;
                await X.GoToPosition(configuration.StepActual * configuration.Interval);
                await Z.GoToPosition(configuration.Height);
                Saw.Run();
                while (this.IsTurn == true && configuration.StepActual <= configuration.StepCount)
                {
                    configuration.StepActual += 1;
                    await X.GoToPosition(configuration.StepActual * configuration.Interval);
                    await Y.GoToPosition(Math.Abs(Y.Position - configuration.Width));
                    
                }
                Saw.Stop();
                return true;
            }
            catch
            {
                StopCommand.Execute(null);
                return false;
            }

        }

        public void Stop() => this.IsTurn = false;

    }
}
