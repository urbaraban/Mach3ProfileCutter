using System;

namespace ProfileCutter.Model.Programms
{
    public class CutConfiguration : ModelObject
    {
        public bool IsRunning 
        {
            get => _isrunning;
            set
            {
                _isrunning = value;
                OnPropertyChanged(nameof(IsRunning));
            }
        }
        private bool _isrunning = false;

        public Guid Id { get; set; } = Guid.NewGuid();

        public string Display
        {
            get => $"{this.Name} - {this.Length} - {this.Width} - {this.Interval} - {this.Height}";
            set => this.Name = value;
        }
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
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
                OnPropertyChanged(nameof(StepCount));
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
                OnPropertyChanged(nameof(StepCount));
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
            }
        }
        private double _height = 1;

        public int StepActual 
        {
            get => stepactual;
            set
            {
                stepactual = Math.Max(value, 0);
                OnPropertyChanged(nameof(StepActual));
            }
        }
        private int stepactual = 0;

        public int StepCount
        {
            get => stepcount;
            set
            {
                stepcount = value;
                OnPropertyChanged(nameof(StepCount));
            }
        }
        private int stepcount;

        public void UpdateDisplay()
        {
            OnPropertyChanged(nameof(Display));
        }
    }
}
