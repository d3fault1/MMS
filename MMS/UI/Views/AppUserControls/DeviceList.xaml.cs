using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Collections.Specialized;
using MMS.Backend;
using MMS.DataModels;

namespace MMS.UI.Views.AppUserControls
{
    /// <summary>
    /// Interaction logic for DeviceList.xaml
    /// </summary>
    public partial class DeviceList : UserControl
    {
        private static ObservableCollection<ViewDevicesModel> ViewList { get; set; }

        public DeviceList()
        {
            InitializeComponent();
            ViewList = new ObservableCollection<ViewDevicesModel>();
            alldevlist.ItemsSource = ViewList;
            Globals.AllDeviceListChanged += Globals_AllDeviceListChanged;
        }

        private void Globals_AllDeviceListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (NodeModel a in e.NewItems)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ViewList.Add(new ViewDevicesModel
                            {
                                Name = a.Name,
                                Mac = a.MacAddress,
                                IP = a.IP,
                                Floor = a.FloorID.ToString(),
                                Zone = a.ZoneID.ToString(),
                                Exhibit = a.ExhibitID.ToString(),
                                Version = a.CurrentStatus.Version,
                                Status = a.IsConfig ? (a.IsOnline ? DeviceStatus.Online : DeviceStatus.Offline) : DeviceStatus.NotConfigured
                            });
                        });
                        
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (NodeModel a in e.OldItems)
                    {
                        var temp = ViewList.FirstOrDefault(v => v.Mac == a.MacAddress);
                        if (temp != null) Dispatcher.Invoke(() => ViewList.Remove(temp));
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (NodeModel a in e.NewItems)
                    {
                        var temp = ViewList.FirstOrDefault(v => v.Mac == a.MacAddress);
                        var index = ViewList.IndexOf(temp);
                        Dispatcher.Invoke(() =>
                        {
                            var vdm = new ViewDevicesModel
                            {
                                Name = a.Name,
                                Mac = a.MacAddress,
                                IP = a.IP,
                                Floor = a.FloorID.ToString(),
                                Zone = a.ZoneID.ToString(),
                                Exhibit = a.ExhibitID.ToString(),
                                Version = a.CurrentStatus.Version,
                                Status = a.IsConfig ? (a.IsOnline ? DeviceStatus.Online : DeviceStatus.Offline) : DeviceStatus.NotConfigured
                            };
                            if (temp == null) index = -1;
                            if (index == -1)
                            {
                                ViewList.Add(vdm);
                            }
                            else
                            {
                                ViewList[index] = vdm;
                            }
                        });
                        
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Dispatcher.Invoke(() => ViewList.Clear());
                    break;
            }
            alldevlist.Items.Refresh();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var item = ((Button)sender).DataContext;
            var index = -1;
            if (item != null)
            {
                index = alldevlist.Items.IndexOf(item);
            }
            if (index == -1) return;
            var dev = Globals.AllDevices.FirstOrDefault(a => a.MacAddress == ViewList[index].Mac);
            if (dev == null) return;
            dev.HeartbeatRate = 10;
            dev.FloorID = 1;
            dev.ZoneID = 1;
            dev.ExhibitID = 1;
            Globals.RegisterDevice(dev);
            ViewList[index].Status = DeviceStatus.AwaitingRegistration;
            alldevlist.Items.Refresh();
        }
    }
}
