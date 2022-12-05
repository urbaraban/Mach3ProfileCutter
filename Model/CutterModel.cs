using Mach3_netframework.MACH3;
using ProfileCutter.Model.MACH3;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileCutter.Model
{
    internal class CutterModel : ModelObject
    {
        private Mach3 Mach3 = new Mach3();

        public AxisModel X { get; }
        public AxisModel Y { get; }
        public AxisModel Z { get; }

        public List<SensorModel> Sensors { get; } = new List<SensorModel>();

        public CutterModel() 
        {
            this.X = new AxisModel("X", Mach3.X);
            this.Y = new AxisModel("Y", Mach3.X);
            this.Z = new AxisModel("Z", Mach3.X);
        }
    }
}
