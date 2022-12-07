using Mach3_netframework.MACH3;
using Microsoft.Xaml.Behaviors.Core;
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
                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(Steps));
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
        public double Steps => this.AxisMotor.Position;
        public string Name { get; }
        private Mach3AxisMotor AxisMotor { get; }
        public AxisModel(string name, Mach3AxisMotor mach3Axis)
        {
            this.Name = name;
            this.AxisMotor = mach3Axis;
        }

        public ICommand SetZeroCommand => new ActionCommand(() =>
        {
            this.StartPosition = this.Position;
        });
    }
}
