using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace ProfileCutter.Model.Programms
{
    public class ProgrammManager : ModelObject
    {
        public ObservableCollection<CutProgramm> Programms { get; } = new ObservableCollection<CutProgramm>();
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
                if (_profile != null) SelectProgramm.Length = this.SelectProfile.Length;
                OnPropertyChanged(nameof(SelectProfile));
            }
        }
        private Profile _profile;

        public CutProgramm SelectProgramm
        {
            get
            {
                if (_selectprogramm == null)
                {
                    _selectprogramm = new CutProgramm();
                }
                return _selectprogramm;
            }
            set
            {
                _selectprogramm = value;
                if (_selectprogramm == null)
                {
                    _selectprogramm = new CutProgramm();
                }
                OnPropertyChanged(nameof(SelectProgramm));
            }
        }
        private CutProgramm _selectprogramm;

        public ICommand AddProgrammCommand => new ActionCommand(() => 
        {
            SelectProgramm = new CutProgramm();
            Programms.Add(SelectProgramm);
        });

        public ICommand RemoveProgrammCommand => new ActionCommand(() =>
        {
            for (int i = 0; i < Programms.Count; i += 1)
            {
                if (SelectProgramm.Id == Programms[i].Id)
                    Programms.RemoveAt(i);
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
            this.SelectProgramm.Length = SelectProfile.Length;
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
