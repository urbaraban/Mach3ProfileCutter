using Mach3_netframework.MACH3;
using Microsoft.Xaml.Behaviors.Core;
using System.Windows.Input;

namespace ProfileCutter.Model.MACH3
{
    internal class AxisModel : ModelObject
    {
        public double Position { get; }
        public string Name { get; }
        private Mach3AxisMotor axisMotor { get; }
        public AxisModel(string name, Mach3AxisMotor mach3Axis)
        {
            this.Name = name;
            this.axisMotor = mach3Axis;
        }

        public ICommand SetZeroCommand => new ActionCommand(() =>
        {

        });
    }
}
