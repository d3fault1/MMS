using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using MMS.UI.Views.AppWindows;
using MMS.Backend;
using MMS.DataModels;

namespace MMS.UI.Views.AppPages
{
    /// <summary>
    /// Interaction logic for DeviceList.xaml
    /// </summary>
    public partial class DeviceList : Page
    {
        //private static ObservableCollection<ViewDevicesModel> ViewList { get; set; }
        private Dictionary<long, CommandModel> NodeCurrentCommand { get; }
        private List<Predicate<object>> DeviceListFilters { get; }

        public DeviceList()
        {
            InitializeComponent();
            //ViewList = new ObservableCollection<ViewDevicesModel>();
            NodeCurrentCommand = new Dictionary<long, CommandModel>();
            DeviceListFilters = new List<Predicate<object>>(3);
            DeviceListFilters.Add(p => true);
            DeviceListFilters.Add(p => true);
            DeviceListFilters.Add(p => true);
            DevicesListView.Items.IsLiveFiltering = true;
            DevicesListView.Items.Filter = p => CombinedFilterPredicate((NodeModel)p);

            //Globals.AllDeviceListChanged += Globals_AllDeviceListChanged;
        }

        //private void Globals_AllDeviceListChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    switch (e.Action)
        //    {
        //        case NotifyCollectionChangedAction.Add:
        //            foreach (NodeModel a in e.NewItems)
        //            {
        //                Dispatcher.Invoke(() =>
        //                {
        //                    ViewList.Add(new ViewDevicesModel
        //                    {
        //                        Name = a.Name,
        //                        Mac = a.MacAddress,
        //                        IP = a.IP,
        //                        Floor = a.FloorID.ToString(),
        //                        Zone = a.ZoneID.ToString(),
        //                        Exhibit = a.ExhibitID.ToString(),
        //                        Version = a.CurrentStatus.Version,
        //                        Status = a.IsConfig ? (a.IsOnline ? DeviceStatus.Online : DeviceStatus.Offline) : DeviceStatus.NotConfigured
        //                    });
        //                });

        //            }
        //            break;
        //        case NotifyCollectionChangedAction.Remove:
        //            foreach (NodeModel a in e.OldItems)
        //            {
        //                var temp = ViewList.FirstOrDefault(v => v.Mac == a.MacAddress);
        //                if (temp != null) Dispatcher.Invoke(() => ViewList.Remove(temp));
        //            }
        //            break;
        //        case NotifyCollectionChangedAction.Replace:
        //            foreach (NodeModel a in e.NewItems)
        //            {
        //                var temp = ViewList.FirstOrDefault(v => v.Mac == a.MacAddress);
        //                var index = ViewList.IndexOf(temp);
        //                Dispatcher.Invoke(() =>
        //                {
        //                    var vdm = new ViewDevicesModel
        //                    {
        //                        Name = a.Name,
        //                        Mac = a.MacAddress,
        //                        IP = a.IP,
        //                        Floor = a.FloorID.ToString(),
        //                        Zone = a.ZoneID.ToString(),
        //                        Exhibit = a.ExhibitID.ToString(),
        //                        Version = a.CurrentStatus.Version,
        //                        Status = a.IsConfig ? (a.IsOnline ? DeviceStatus.Online : DeviceStatus.Offline) : DeviceStatus.NotConfigured
        //                    };
        //                    if (temp == null) index = -1;
        //                    if (index == -1)
        //                    {
        //                        ViewList.Add(vdm);
        //                    }
        //                    else
        //                    {
        //                        ViewList[index] = vdm;
        //                    }
        //                });

        //            }
        //            break;
        //        case NotifyCollectionChangedAction.Reset:
        //            Dispatcher.Invoke(() => ViewList.Clear());
        //            break;
        //    }
        //    //alldevlist.Items.Refresh();
        //}

        private void ItemActionButtonClick(object sender, RoutedEventArgs e)
        {
            var item = (NodeModel)((Button)sender).DataContext;
            if (item == null) return;
            if (DataHub.Nodes.IndexOf(item) == -1) return;  

            if (((Button)sender).Content.ToString() == "Register")
            {
                AddDevice addDevWindow = new AddDevice();
                addDevWindow.DeviceIP.Text = item.IP;
                addDevWindow.DeviceMac.Text = item.MacAddress;
                addDevWindow.DeviceNameField.Text = item.Name;
                addDevWindow.DeviceNameField.IsEnabled = false;
                var result = addDevWindow.ShowDialog();
                if (!result.HasValue) return;
                if (!result.Value) return;

                item.HeartbeatRate = 10;
                if (addDevWindow.DeviceFloorField.SelectedIndex == -1) item.FloorID = -1;
                else
                {
                    var floor = DataHub.Floors.FirstOrDefault(a => a.Name == addDevWindow.DeviceFloorField.SelectedItem.ToString());
                    if (floor == null) item.FloorID = -1;
                    else item.FloorID = floor.ID;
                }
                if (addDevWindow.DeviceCategoryField.SelectedIndex == -1) item.Category = "";
                else item.Category = addDevWindow.DeviceCategoryField.SelectedItem.ToString();

                Globals.RegisterDevice(item);
            }
            if (((Button)sender).Content.ToString() == "Send")
            {
                if (NodeCurrentCommand.ContainsKey(item.ID))
                {
                    Globals.SendCommand(NodeCurrentCommand[item.ID], item, true);
                }
            }
            
        }

        private void ConfigureManualClick(object sender, RoutedEventArgs e)
        {
            AddDevice manualDeviceDialog = new AddDevice();
            manualDeviceDialog.DeviceIP.Text = "N/A";
            manualDeviceDialog.DeviceMac.Text = "N/A";
            manualDeviceDialog.ShowDialog();
        }

        private void CommandSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex == -1) return;
            var item = (NodeModel)((ComboBox)sender).DataContext;
            if (NodeCurrentCommand.ContainsKey(item.ID)) NodeCurrentCommand[item.ID] = (CommandModel)((ComboBox)sender).SelectedItem;
            else NodeCurrentCommand.Add(item.ID, (CommandModel)((ComboBox)sender).SelectedItem);
        }

        private void CategorySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmbBox = (ComboBox)sender;
            if (cmbBox.SelectedIndex == -1) DeviceListFilters[0] = p => true;
            else DeviceListFilters[0] = p => ((NodeModel)p).Category == cmbBox.SelectedItem.ToString();
            DevicesListView.Items.Filter = p => CombinedFilterPredicate((NodeModel)p);
        }

        private void FloorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmbBox = (ComboBox)sender;
            if (cmbBox.SelectedIndex == -1) DeviceListFilters[1] = p => true;
            else DeviceListFilters[1] = p => ((NodeModel)p).Floor.Name == cmbBox.SelectedItem.ToString();
            DevicesListView.Items.Filter = p => CombinedFilterPredicate((NodeModel)p);
        }

        private void StatusSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmbBox = (ComboBox)sender;
            if (cmbBox.SelectedIndex == -1) DeviceListFilters[2] = p => true;
            else DeviceListFilters[2] = p =>
            {
                if (cmbBox.SelectedItem.ToString() == "All") return true;
                if (cmbBox.SelectedItem.ToString() == "Online") return ((NodeModel)p).IsConfig && ((NodeModel)p).IsOnline;
                if (cmbBox.SelectedItem.ToString() == "Offline") return ((NodeModel)p).IsConfig && !((NodeModel)p).IsOnline;
                if (cmbBox.SelectedItem.ToString() == "Not Configured") return !((NodeModel)p).IsConfig;
                return true;
            };
            DevicesListView.Items.Filter = p => CombinedFilterPredicate((NodeModel)p);
        }

        private bool CombinedFilterPredicate(NodeModel p)
        {
            foreach (var predicate in DeviceListFilters)
            {
                if (!predicate.Invoke(p)) return false;
            }
            return true;
        }
    }
}
