using Mach3_netframework.MACH3;
using ProfileCutter.Model.MACH3;
using System.Collections.Generic;

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
            Sensors.Add(new SensorModel("Пр2", Mach3.SensorPoller, 7, 0));
            Sensors.Add(new SensorModel("Пр1", Mach3.SensorPoller, 4, 1));
            Sensors.Add(new SensorModel("X", Mach3.SensorPoller, 5, 1));
            Sensors.Add(new SensorModel("Y", Mach3.SensorPoller, 3, 1));
            Sensors.Add(new SensorModel("Z", Mach3.SensorPoller, 6, 1));

            this.Saw = new ToggleModel(Mach3.Spindle, null);

            this.X = new AxisModel("X", Mach3.X);
            this.Y = new AxisModel("Y", Mach3.X);
            this.Z = new AxisModel("Z", Mach3.X);
        }
    }
}
