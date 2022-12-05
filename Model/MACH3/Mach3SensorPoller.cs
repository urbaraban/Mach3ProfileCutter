using Mach3_netframework.MACH3;
using Mach3_netframework.MACH3.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static Mach3_netframework.MACH3.InpOut32x64.InpOut;

namespace ProfileCutter.Model.MACH3
{
    public class Mach3SensorPoller : List<Mach3Sensor>, IInpOutElement
    {
        public OutDelegate Out { get; set; }
        public InpDelegate Inp { get; set; }

        private Timer timer { get; }

        public Mach3SensorPoller()
        {
            TimerCallback timerCallback = new TimerCallback(PollingSensors);
            this.timer = new Timer(timerCallback, 0, 0, 1);
        }

        private void PollingSensors(object obj)
        {
            if (Inp != null)
            {
                int request = Inp(889);
                for (int i = 8; i > 0; i -= 1) 
                {
                    GetByNumber(15 * i)?.SetStat(request % 2);
                    request /= 2;
                }
            }
        }

        public Mach3Sensor GetByNumber(int value)
        {
            return this.FirstOrDefault(e => e.IsThis(value));
        }

        public void Stop()
        {
            this.timer.Dispose();
        }

    }
}
