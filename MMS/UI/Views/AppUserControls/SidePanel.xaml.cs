using System;
using System.Windows;
using System.Windows.Controls;

namespace MMS.UI.Views.AppUserControls
{
    /// <summary>
    /// Interaction logic for Sidepanel.xaml
    /// </summary>
    public partial class SidePanel : UserControl
    {
        public delegate void NavigationRequestedEventHandler(Uri TargetPage);
        public event NavigationRequestedEventHandler NavigationRequested;

        public SidePanel()
        {
            InitializeComponent();
        }

        private void DeviceDashboardClick(object sender, RoutedEventArgs e)
        {
            OnNavigationRequested("/UI/Views/AppPages/Dashboard.xaml");
        }

        private void TotalDevicesClick(object sender, RoutedEventArgs e)
        {
            OnNavigationRequested("/UI/Views/AppPages/DeviceList.xaml");
        }

        private void ContentUploadClick(object sender, RoutedEventArgs e)
        {

        }

        private void SoftwareUpdateClick(object sender, RoutedEventArgs e)
        {

        }

        private void CommandLogsClick(object sender, RoutedEventArgs e)
        {

        }

        private void ControlPanelClick(object sender, RoutedEventArgs e)
        {

        }

        private void PowerOnOffClick(object sender, RoutedEventArgs e)
        {

        }

        private void ShowDevicesClick(object sender, RoutedEventArgs e)
        {

        }

        private void SettingsClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnNavigationRequested(string RelativeTargetUri)
        {
            NavigationRequested?.Invoke(new Uri(RelativeTargetUri, UriKind.Relative));
        }
    }
}
