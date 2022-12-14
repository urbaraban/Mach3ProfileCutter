using Mach3_netframework.MACH3;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace ProfileCutter.Model.MACH3
{
    public class AxisModel : ModelObject
    {
        public long Steps => this.AxisMotor.Position;
        public long Delay => (long)(500000 / (Speed * StPerMillimetre));

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

        private SensorModel _sensor;

        private Mach3AxisMotor AxisMotor { get; }
        public AxisModel(string name, Mach3AxisMotor mach3Axis, SensorModel sensor, bool inverse, double offset = 0)
        {
            this.Name = name;
            this.AxisMotor = mach3Axis;
            this.Offset = offset;
            this.InversePosition = inverse;
            this._sensor = sensor;
            sensor.StatusChanged += Sensor_StatusChanged; ;
        }

        private void Sensor_StatusChanged(object sender, bool e)
        {
            if (e == true)
            {
                this.AxisMotor.SetZero();
                this.StartPosition = 0;
            }
        }

        public void GoToPosition(double value)
        {
            long finish = GetPositionInStep(value);
            if (Math.Abs(finish - this.AxisMotor.Position) > 1)
            {
                int vector = value < this.Position ? -1 : 1;
                this.AxisMotor.TryStart = true;
                while (this.AxisMotor.ThisStop == false && (
                    (vector > 0 && this._sensor.Detect == true)
                    || Math.Abs(finish - Steps) > 0))
                {
                    this.AxisMotor.TryStart = true;
                    this.AxisMotor.Tic(vector, this.Delay);
                    OnPropertyChanged(nameof(Position));
                    OnPropertyChanged(nameof(Steps));
                }
            }
        }

        public void GoHome()
        {
            while(this.AxisMotor.ThisStop == false && this._sensor.Detect == false)
            {
                this.AxisMotor.TryStart = true;
                this.AxisMotor.Tic(-1, this.Delay);
                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(Steps));
            }
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

        public ICommand SetZeroCommand => new ActionCommand(() => this.StartPosition = this.Steps);

    }
}
