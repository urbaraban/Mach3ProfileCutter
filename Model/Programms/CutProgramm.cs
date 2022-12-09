using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileCutter.Model.Programms
{
    internal class CutProgramm : ModelObject
    {
        public double Width
        {
            get => _width;
            set
            {
                _width = value;
                OnPropertyChanged(nameof(Width));
            }
        }
        private double _width;
        public double Interval
        {
            get => _interval;
            set
            {
                _interval = value;
                OnPropertyChanged(nameof(Interval));
            }
        }
        private double _interval;

        public int StepCount => (int)Math.Round(SelectProfile.Length / Interval) - 1;

        public Profile SelectProfile 
        {
            get => _profile;
            set
            {
                _profile = value;
                OnPropertyChanged();
            }
        }
        private Profile _profile;


        public async Task<bool> Run()
        {
            return false;
        }
    }
}
