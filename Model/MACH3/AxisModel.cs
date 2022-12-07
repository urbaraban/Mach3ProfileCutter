using Mach3_netframework.MACH3;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProfileCutter.Model.MACH3
{
    internal class AxisModel : ModelObject
    {
        public double StartPosition
        {
            get => startposition;
            set
            {
                startposition= value;
                OnPropertyChanged(nameof(StartPosition));
                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(Steps));
            }
        }
        private double startposition = 0;
        public double Position
        {
            get => Steps / StPerMillimetre - StartPosition;
            set
            {
                GoToPosition(value);
                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(Steps));
            }
        }
        public double MaxPosition
        {
            get => this.AxisMotor.Maximum / this.StPerMillimetre;
            set
            {
                this.AxisMotor.Maximum = (long)(value * this.StPerMillimetre);
                OnPropertyChanged(nameof(MaxPosition));
            }
        }

        public double StPerMillimetre 
        {
            get => stpermillimetre;
            set
            {
                stpermillimetre = value;
                OnPropertyChanged(nameof(StPerMillimetre));
            }
        }
        public double stpermillimetre = 1;

        public double Speed
        {
            get => speed;
            set
            {
                speed = value;
                OnPropertyChanged(nameof(Speed));
            }
        }
        private double speed = 1;

        public long Steps => this.AxisMotor.Position;

        public int Delay => (int)(500000 / (Speed * StPerMillimetre));

        public string Name { get; }
        private Mach3AxisMotor AxisMotor { get; }
        public AxisModel(string name, Mach3AxisMotor mach3Axis)
        {
            this.Name = name;
            this.AxisMotor = mach3Axis;
        }

        public void GoToPosition(double value)
        {
            long finish = (long)(value * this.StPerMillimetre);
            int vector = value < this.Position ? -1 : 1;
            this.AxisMotor.TryStart = true;
            while (this.AxisMotor.ThisStop == false && Math.Abs(finish - Steps) > 0)
            {
                this.AxisMotor.TryStart = true;
                this.AxisMotor.Tic(vector, this.Delay);
                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(Steps));
            }
        }

        public ICommand SetZeroCommand => new ActionCommand(() => this.StartPosition = this.Position);
    }
}
