using MMS.Backend;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MMS.UI.Views.AppWindows
{
    /// <summary>
    /// Interaction logic for AddDevice.xaml
    /// </summary>
    public partial class AddEditDevice : Window
    {
        public AddEditDevice(bool isedit)
        {
            InitializeComponent();

            if (isedit)
            {
                Title = "EditDevice";
                windowHeader.Text = "Edit Device Details";
            }
            else
            {
                Title = "AddDevice";
                windowHeader.Text = "Add Device Details";
            }
            DeviceFloorField.ItemsSource = DataHub.Floors.Select(a => a.Name);
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
            if (CheckFields())
            {
                DialogResult = true;
                Close();
            }
        }

        private bool CheckFields()
        {
            bool res = true;
            DeviceNameField.BorderBrush = (Brush)Application.Current.Resources["BrushTransparentAux"];
            DeviceNameField.BorderThickness = new Thickness(0);

            if (String.IsNullOrWhiteSpace(DeviceNameField.Text))
            {
                DeviceNameField.BorderBrush = (Brush)Application.Current.Resources["BrushErrorMain"];
                DeviceNameField.BorderThickness = new Thickness(2);
                res = false;
            }
            return res;
        }
    }
}
