using Mach3_netframework.MACH3;
using System;
using System.Security;
using System.Threading.Tasks;
using System.Windows;

namespace ProfileCutter.Model.Programms
{
    public class CutProgramm : ModelObject
    {
        public bool IsStopping 
        {
            get => _isstopping;
            private set
            {
                _isstopping = value;
                OnPropertyChanged(nameof(IsStopping));
            }
        }
        private bool _isstopping = true;

        public Guid Id { get; set; } = Guid.NewGuid();

        public string Display
        {
            get => $"{this.Name} - {this.Length} - {this.Interval} - {this.Height}";
            set => this.Name = value;
        }
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Display));
            }
        }
        private string _name = "Empty";

        public double Length
        {
            get => _length;
            set
            {
                _length = value;
                OnPropertyChanged(nameof(Length));
                OnPropertyChanged(nameof(Display));
            }
        }
        private double _length = 0;

        public double Width
        {
            get => _width;
            set
            {
                _width = value;
                OnPropertyChanged(nameof(Width));
                OnPropertyChanged(nameof(Display));
            }
        }
        private double _width = 0;
        public double Interval
        {
            get => _interval;
            set
            {
                _interval = value;
                OnPropertyChanged(nameof(Interval));
                OnPropertyChanged(nameof(Display));
            }
        }
        private double _interval = 0;

        public double Height
        {
            get => _height;
            set
            {
                _height = value;
                OnPropertyChanged(nameof(Height));
                OnPropertyChanged(nameof(Display));
            }
        }
        private double _height = 1;

        public int StepActual { get; set; } = 0;

        public int StepCount => (int)Math.Round(this.Length / Interval) - 1;

        private void SetProfile(Profile profile)
        {
            if (MessageBox.Show("Пересчитать под профиль?", "Настройки", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this.Length = profile.Length;
            }
        }

        public async Task<bool> Run(CutterModel mach3)
        {
            IsStopping = false;
            return false;
        }
    }
}
