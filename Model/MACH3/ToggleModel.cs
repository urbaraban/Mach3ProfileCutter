using Mach3_netframework.MACH3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileCutter.Model.MACH3
{
    internal class ToggleModel : ModelObject
    {
        public bool IsOn => Sensor.Detect != this.InverseSensor;
        private SensorModel Sensor { get; set; }
        private readonly bool InverseSensor = false;

        private Mach3Toggle Mach3Toggle { get; }

        public ToggleModel(Mach3Toggle mach3Toggle, SensorModel sensor)
        {
            this.Sensor = sensor;
            this.Mach3Toggle = mach3Toggle;
        }
    }
}
