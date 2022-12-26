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
    /// Логика взаимодействия для Mach3StatusControl.xaml
    /// </summary>
    public partial class Mach3StatusControl : UserControl
    {
        public Mach3StatusControl()
        {
            InitializeComponent();
        }
    }

    public class BoolColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool detect && detect == true)
            {
                return Brushes.IndianRed;
            }
            return Brushes.WhiteSmoke;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class ToggleColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ToggleStatus status)
            {
                if (status == ToggleStatus.RUNING)
                    return Brushes.IndianRed;
                else if (status == ToggleStatus.READY)
                    return Brushes.Yellow;
                else
                    return Brushes.DarkGreen;
            }
            return Brushes.WhiteSmoke;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
