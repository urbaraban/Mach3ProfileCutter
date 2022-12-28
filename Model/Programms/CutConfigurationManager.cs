using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ProfileCutter.Model.Programms
{
    public class CutConfigurationManager : ModelObject
    {
        public ObservableCollection<CutConfiguration> Configs { get; } = new ObservableCollection<CutConfiguration>();
        public ObservableCollection<Profile> Profiles { get; } = new ObservableCollection<Profile>();

        public Profile SelectProfile
        {
            get
            {
                if (_profile == null)
                {
                    SelectProfile = new Profile()
                    {
                        Name = "Empty",
                        Length = 2000,
                        Width = 50
                    };
                }
                return _profile;
            }
            set
            {
                _profile = value;
                if (value != null && GetProfile(value.Id.ToString()) == null) this.Profiles.Add(value);
                if (_profile != null) SelectConf.Length = this.SelectProfile.Length;
                OnPropertyChanged(nameof(SelectProfile));
            }
        }
        private Profile _profile;

        public CutConfiguration SelectConf
        {
            get
            {
                if (_SelectConf == null)
                {
                    _SelectConf = new CutConfiguration();
                }
                return _SelectConf;
            }
            set
            {
                _SelectConf = value;
                if (_SelectConf == null)
                {
                    _SelectConf = new CutConfiguration();
                }
                OnPropertyChanged(nameof(SelectConf));
            }
        }
        private CutConfiguration _SelectConf;

        public ICommand AddProgrammCommand => new ActionCommand(() => 
        {
            SelectConf = new CutConfiguration();
            Configs.Add(SelectConf);
        });

        public ICommand RemoveProgrammCommand => new ActionCommand(() =>
        {
            for (int i = 0; i < Configs.Count; i += 1)
            {
                if (SelectConf.Id == Configs[i].Id)
                    Configs.RemoveAt(i);
            }
        });

        public ICommand AddProfileCommand => new ActionCommand(() =>
        {
            SelectProfile = new Profile();
            Profiles.Add(SelectProfile);
        });

        public ICommand RemoveProfileCommand => new ActionCommand(() =>
        {
            for (int i = 0; i < Profiles.Count; i += 1)
            {
                if (SelectProfile.Id == Profiles[i].Id)
                    Profiles.RemoveAt(i);
            }
        });

        public ICommand UpdateProfileLengthCommand => new ActionCommand(() =>
        {
            SelectConf.Length = SelectProfile.Length;
        });

        public ICommand SetMaxStepCommand => new ActionCommand(() =>
        {
            SelectConf.StepCount = (int)Math.Round(SelectConf.Length / SelectConf.Interval) - 1;
        });
        public ICommand SetZeroStepCommand => new ActionCommand(() =>
        {
            SelectConf.StepActual = 0;
        });

        internal Profile GetProfile(string value)
        {
            Guid id = Guid.Parse(value);
            foreach (Profile profile in this.Profiles)
            {
                if(profile.Id == id) return profile;
            }
            return new Profile();
        }
    }

    public class Profile : ModelObject
    {
        public string Display
        {
            get => $"{this.Name} - {this.Length}";
            set
            {
                this.Name = value;
            }
        }
        public Guid Id { get; set; } = Guid.NewGuid();
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
        public double Length { get; set; }
        public double Width { get; set; }

        internal void UpdateDisplay()
        {
            OnPropertyChanged(nameof(Display));
        }
    }
}
