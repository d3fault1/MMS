using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Specialized;
using MMS.Backend;

namespace MMS.UI.Views.AppPages
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page
    {
        public Dashboard()
        {
            InitializeComponent();
            DataHub.Nodes.CollectionChanged += DeviceListChanged;
        }

        private void DeviceListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            numOnlineDevice.Text = DataHub.Nodes.Where(a => a.IsConfig && a.IsOnline).Count().ToString();
            numOfflineDevice.Text = DataHub.Nodes.Where(a => a.IsConfig && !a.IsOnline).Count().ToString();
            numNewDevice.Text = DataHub.Nodes.Where(a => !a.IsConfig).Count().ToString();
            numTotalDevice.Text = DataHub.Nodes.Count.ToString();
        }

        private void OnlineDevClick(object sender, RoutedEventArgs e)
        {

        }

        private void OfflineDevClick(object sender, RoutedEventArgs e)
        {

        }

        private void NewDevClick(object sender, RoutedEventArgs e)
        {

        }

        private void TotalDevClick(object sender, RoutedEventArgs e)
        {

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
    }
}
