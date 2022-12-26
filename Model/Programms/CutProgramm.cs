using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

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
                SetMaxStepCommand.Execute(null);
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
                SetMaxStepCommand.Execute(null);
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

        public async Task<bool> Run(CutterModel modelM3)
        {
            try
            {
                modelM3.HomeCommand.Execute(null);
                modelM3.Mach3.IsTurn = true;
                IsStopping = false;
                StepActual = StepActual == 0 ? 1 : StepActual;
                await modelM3.TryMoveAxis(modelM3.X, this.StepActual);
                await modelM3.TryMoveAxis(modelM3.Z, this.Height);
                await modelM3.Saw.Run();
                while (modelM3.Mach3.IsTurn == true && StepActual <= this.StepCount)
                {
                    await modelM3.TryMoveAxis(modelM3.X, this.StepActual * this.Interval);
                    await modelM3.TryMoveAxis(modelM3.Y, Math.Abs(modelM3.Y.Position - this.Width));
                    StepActual += 1;
                }
                return true;
            }
            catch
            {
                modelM3.StopCommand.Execute(null);
                return false;
            }

        }

        public void UpdateDisplay()
        {
            OnPropertyChanged(nameof(Display));
        }

        public ICommand SetMaxStepCommand => new ActionCommand(() =>
        {
            StepCount = (int)Math.Round(this.Length / Interval) - 1;
        });
        public ICommand SetZeroStepCommand => new ActionCommand(() =>
        {
            StepActual = 0;
        });
    }
}
