using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProfileCutter.Model
{
    public abstract class ModelObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
