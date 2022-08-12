using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MMS.Backend;
using MMS.DataModels;
using MMS.UI.Views.AppWindows;

namespace MMS.UI.Views.AppPages
{
    /// <summary>
    /// Interaction logic for DeviceList.xaml
    /// </summary>
    public partial class DeviceList : Page
    {
        private Dictionary<long, CommandModel> NodeCurrentCommand { get; }
        private List<Predicate<object>> DeviceListFilters { get; }

        public DeviceList()
        {
            NodeCurrentCommand = new Dictionary<long, CommandModel>();
            DeviceListFilters = new List<Predicate<object>>(3);
            DeviceListFilters.Add(p => true);
            DeviceListFilters.Add(p => true);
            DeviceListFilters.Add(p => true);

            InitializeComponent();

            DevicesListView.Items.IsLiveFiltering = true;
            DevicesListView.Items.LiveFilteringProperties.Add(nameof(NodeModel.Category));
            DevicesListView.Items.LiveFilteringProperties.Add(nameof(NodeModel.IsConfig));
            DevicesListView.Items.LiveFilteringProperties.Add(nameof(NodeModel.IsOnline));
            DevicesListView.Items.LiveFilteringProperties.Add(nameof(FloorModel.Name));
            DevicesListView.Items.Filter = p => CombinedFilterPredicate((NodeModel)p);
        }

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
            if (DevicesListView == null) return;
            var cmbBox = (ComboBox)sender;
            if (cmbBox.SelectedIndex == -1) DeviceListFilters[0] = p => true;
            else DeviceListFilters[0] = p => ((NodeModel)p).Category == cmbBox.SelectedValue.ToString();
            DevicesListView.Items.Filter = p => CombinedFilterPredicate((NodeModel)p);
        }

        private void FloorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DevicesListView == null) return;
            var cmbBox = (ComboBox)sender;
            if (cmbBox.SelectedIndex == -1) DeviceListFilters[1] = p => true;
            else DeviceListFilters[1] = p => ((NodeModel)p).Floor.Name == cmbBox.SelectedValue.ToString();
            DevicesListView.Items.Filter = p => CombinedFilterPredicate((NodeModel)p);
        }

        private void StatusSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DevicesListView == null) return;
            var cmbBox = (ComboBox)sender;
            if (cmbBox.SelectedIndex == -1) DeviceListFilters[2] = p => true;
            else DeviceListFilters[2] = p =>
            {
                if (cmbBox.SelectedValue.ToString() == "All") return true;
                if (cmbBox.SelectedValue.ToString() == "Online") return ((NodeModel)p).IsConfig && ((NodeModel)p).IsOnline;
                if (cmbBox.SelectedValue.ToString() == "Offline") return ((NodeModel)p).IsConfig && !((NodeModel)p).IsOnline;
                if (cmbBox.SelectedValue.ToString() == "Not Configured") return !((NodeModel)p).IsConfig;
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
