using Mach3_netframework.MACH3;
using Microsoft.Xaml.Behaviors.Core;
using ProfileCutter.Model.Interfaces;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using static ProfileCutter.Model.CutterModel;

namespace ProfileCutter.Model.MACH3
{
    public class AxisModel : ModelObject
    {
        public delegate void PreMoveAction();
        public PreMoveAction PreMove { get; set; }

        public TurnAxisDelegate TurnAxis { get; set; }

        public long Steps => this.AxisMotor.Position;

        public bool InversePosition { get; set; } = false;
        public double Position
        {
            get
            {
                double result = (this.AxisMotor.Position - this.StartPosition) / this.StPerMillimetre + this.Offset / 2;
                if (InversePosition == true)
                {
                    result = this.MaxPosition - result;
                }
                return result;
            }
            set
            {
                GoToPosition(value);
                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(Steps));
            }
        }

        public string Name { get; }

        public long StartPosition
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
        private long startposition = 0;

        public double Offset
        {
            get => _offset;
            set
            {
                _offset = value;
                OnPropertyChanged(nameof(Offset));
            }
        }
        private double _offset = 0;

        public double MaxPosition
        {
            get => (this.AxisMotor.Maximum / this.StPerMillimetre + this.Offset / 2);
            set
            {
                this.AxisMotor.Maximum = (long)((value - this.Offset / 2) * this.StPerMillimetre);
                OnPropertyChanged(nameof(MaxPosition));
            }
        }

        public double StPerMillimetre 
        {
            get => stpermillimetre;
            set
            {
                stpermillimetre = value;
                OnPropertyChanged(nameof(StPerMillimetre));
                OnPropertyChanged(nameof(Position));
            }
        }
        public double stpermillimetre = 1;

        public double Speed
        {
            get => speed;
            set
            {
                speed = value;
                OnPropertyChanged(nameof(Speed));
            }
        }
        private double speed = 1;

        private readonly SensorModel _sensor;

        private long Delay => (long)(1000000 / (Speed * StPerMillimetre));

        private Mach3AxisMotor AxisMotor { get; }
        public AxisModel(string name, Mach3AxisMotor mach3Axis, SensorModel sensor, bool inverse, PreMoveAction moveAction, double offset = 0)
        {
            this.Name = name;
            this.AxisMotor = mach3Axis;
            this.Offset = offset;
            this.InversePosition = inverse;
            this._sensor = sensor;
            this.PreMove = moveAction;
            sensor.StatusChanged += Sensor_StatusChanged; ;
        }

        public ICommand SetZeroCommand => new ActionCommand(() => this.StartPosition = this.Steps);

        private void Sensor_StatusChanged(object sender, bool e)
        {
            if (e == true)
            {
                this.AxisMotor.SetZero();
                this.StartPosition = 0;
            }
        }

        private bool AxisStop(MoveVector vector)
        {
            return TurnAxis?.Invoke() == false &&
                ((vector == MoveVector.UP && this.Steps < this.AxisMotor.Maximum) ||
                (vector == MoveVector.DOWN && this._sensor.Detect == false));
        }

        public async Task GoToPosition(double value)
        {
            PreMove?.Invoke();
            long finish = GetPositionInStep(value);
            MoveVector vector =
                (value < this.Position && this.InversePosition == false) ? MoveVector.DOWN : MoveVector.UP;
            if (Math.Abs(finish - this.Steps) > 0)
            {
                while (AxisStop(vector) == false && Math.Abs(finish - this.Steps) > 0)
                {
                    this.Tic(vector);
                }
            };
        }

        public void GoHome()
        {
            while(this.TurnAxis?.Invoke() == true && this._sensor.Detect == false)
            {
                this.Tic(MoveVector.DOWN);
            }
        }

        private void Tic(MoveVector vector)
        {
            this.AxisMotor.Tic(vector, this.Delay);
            OnPropertyChanged(nameof(Position));
            OnPropertyChanged(nameof(Steps));
        }

        private long GetPositionInStep(double value)
        {
            long result = (long)(value * this.stpermillimetre) + this.startposition;
            return result;
        }

        public XElement GetAxisXElement()
        {
            return new XElement(this.Name,
                new XAttribute("SPM", this.StPerMillimetre.ToString(CultureInfo.InvariantCulture)),
                new XAttribute("Speed", this.Speed.ToString(CultureInfo.InvariantCulture)),
                new XAttribute("Offset", this.Offset.ToString(CultureInfo.InvariantCulture)),
                new XAttribute("Maximum", this.MaxPosition.ToString(CultureInfo.InvariantCulture)));
        }

        public void LoadXElement(XElement element)
        {
            if (element != null)
            {
                this.StPerMillimetre = double.Parse(element.Attribute("SPM").Value.Replace(',', '.'), CultureInfo.InvariantCulture);
                this.Speed = double.Parse(element.Attribute("Speed").Value.Replace(',', '.'), CultureInfo.InvariantCulture);
                this.Offset = double.Parse(element.Attribute("Offset").Value.Replace(',', '.'), CultureInfo.InvariantCulture);
                this.MaxPosition = double.Parse(element.Attribute("Maximum").Value.Replace(',', '.'), CultureInfo.InvariantCulture);
            }
        }
    }
}
