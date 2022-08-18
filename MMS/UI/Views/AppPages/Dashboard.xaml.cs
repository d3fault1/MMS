using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MMS.Backend;

namespace MMS.UI.Views.AppPages
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page
    {

        public delegate void NavigationRequestedEventHandler(Uri TargetPage, string from, string param);
        public event NavigationRequestedEventHandler NavigationRequested;

        public Dashboard()
        {
            InitializeComponent();
            for (int i = 0; i < DataHub.Nodes.Count; i++) DataHub.Nodes[i].PropertyChanged += DeviceListPropertyChanged;
            UpdateCategoryCount();
        }

        private void DeviceListPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateCategoryCount();
        }

        private void OnlineDevClick(object sender, RoutedEventArgs e)
        {
            OnNavigationRequested("/UI/Views/AppPages/DeviceList.xaml", "online");
        }

        private void OfflineDevClick(object sender, RoutedEventArgs e)
        {
            OnNavigationRequested("/UI/Views/AppPages/DeviceList.xaml", "offline");
        }

        private void NewDevClick(object sender, RoutedEventArgs e)
        {
            OnNavigationRequested("/UI/Views/AppPages/DeviceList.xaml", "new");
        }

        private void TotalDevClick(object sender, RoutedEventArgs e)
        {
            OnNavigationRequested("/UI/Views/AppPages/DeviceList.xaml", "total");
        }

        private void RamUsageClick(object sender, RoutedEventArgs e)
        {

        }

        private void DiskUsageClick(object sender, RoutedEventArgs e)
        {

        }

        private void CpuUsageClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnNavigationRequested(string RelativeTargetUri, string param)
        {
            NavigationRequested?.Invoke(new Uri(RelativeTargetUri, UriKind.Relative), "dashboard", param);
        }

        private void UpdateCategoryCount()
        {
            Dispatcher.Invoke(() =>
            {
                numOnlineDevice.Text = DataHub.Nodes.Where(a => a.IsConfig && a.IsOnline).Count().ToString();
                numOfflineDevice.Text = DataHub.Nodes.Where(a => a.IsConfig && !a.IsOnline).Count().ToString();
                numNewDevice.Text = DataHub.Nodes.Where(a => !a.IsConfig).Count().ToString();
                numTotalDevice.Text = DataHub.Nodes.Count.ToString();
            });
        }
    }
}
