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
using System.Windows.Shapes;

namespace MMS.UI.Views.AppWindows
{
    /// <summary>
    /// Interaction logic for AddDevice.xaml
    /// </summary>
    public partial class AddDevice : Window
    {
        public AddDevice()
        {
            InitializeComponent();
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void AddDeviceClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
