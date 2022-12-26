using ProfileCutter.Model.MACH3;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProfileCutter.Panels
{
    /// <summary>
    /// Логика взаимодействия для ManualPanel.xaml
    /// </summary>
    public partial class ManualPanel : UserControl
    {
        public ManualPanel()
        {
            InitializeComponent();
        }
    }

    public class ToggleStatusBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ToggleStatus status)
            {
                return status == ToggleStatus.RUNING;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
