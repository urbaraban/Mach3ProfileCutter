using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProfileCutter.Model.Programms
{
    internal class ProgrammManager : ObservableCollection<CutProgramm>, INotifyCollectionChanged
    {
        public CutProgramm SelectProgramm { get; set; }

        public ICommand StartCommand => new ActionCommand(async () => await SelectProgramm.Run());

    }
}
