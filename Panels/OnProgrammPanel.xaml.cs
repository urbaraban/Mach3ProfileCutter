using ProfileCutter.Model;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для OnProgrammPanel.xaml
    /// </summary>
    public partial class OnProgrammPanel : UserControl
    {
        public OnProgrammPanel()
        {
            InitializeComponent();
        }

        private void ProgressBar_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (this.DataContext is CutterModel cutterModel)
            {
                cutterModel.CutConfigs.SelectConf.StepActual += (e.Delta / Math.Abs(e.Delta));
            }
        }
    }
}
