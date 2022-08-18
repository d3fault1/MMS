using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MMS.Backend;
using MMS.DataModels;
using MMS.UI.Assists;
using MMS.UI.Views.AppWindows;

namespace MMS.UI.Views.AppPages
{
    /// <summary>
    /// Interaction logic for DeviceList.xaml
    /// </summary>
    public partial class DeviceList : Page
    {
        public delegate void TransitionRequestedEventHandler(string page, string data);
        public event TransitionRequestedEventHandler TransitionRequested;

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

            FloorOptions.ItemsSource = DataHub.Floors.Select(a => a.Name).ToList().Append("All");

            DevicesListView.Items.IsLiveFiltering = true;
            DevicesListView.Items.LiveFilteringProperties.Add(nameof(NodeModel.Category));
            DevicesListView.Items.LiveFilteringProperties.Add(nameof(NodeModel.IsConfig));
            DevicesListView.Items.LiveFilteringProperties.Add(nameof(NodeModel.IsOnline));
            DevicesListView.Items.LiveFilteringProperties.Add(nameof(FloorModel.Name));
            DevicesListView.Items.Filter = p => CombinedFilterPredicate((NodeModel)p);
        }

        private void ItemActionButtonClick(object sender, RoutedEventArgs e)
        {
            var itemReference = (NodeModel)((Button)sender).DataContext;
            if (itemReference == null) return;
            if (DataHub.Nodes.IndexOf(itemReference) == -1) return;

            var item = new NodeModel
            {
                ID = itemReference.ID,
                MacAddress = itemReference.MacAddress,
                IP = itemReference.IP,
                Name = itemReference.Name,
            };

            if (((Button)sender).Content.ToString() == "Register")
            {
                AddDevice addDevWindow = new AddDevice();
                addDevWindow.DeviceIP.Text = item.IP;
                addDevWindow.DeviceMac.Text = item.MacAddress;
                addDevWindow.DeviceNameField.Text = item.Name;
                var result = addDevWindow.ShowDialog();
                if (!result.HasValue) return;
                if (!result.Value) return;

                item.HeartbeatRate = 10;
                item.Name = addDevWindow.DeviceNameField.Text;
                if (addDevWindow.DeviceFloorField.SelectedIndex == -1) item.FloorID = -1;
                else
                {
                    var floor = DataHub.Floors.FirstOrDefault(a => a.Name == addDevWindow.DeviceFloorField.SelectedItem.ToString());
                    if (floor == null) item.FloorID = -1;
                    else item.FloorID = floor.ID;
                }
                if (addDevWindow.DeviceCategoryField.SelectedIndex == -1) item.Category = "";
                else item.Category = addDevWindow.DeviceCategoryField.SelectedValue.ToString();

                Globals.RegisterDevice(item);
            }
            if (((Button)sender).Content.ToString() == "Send")
            {
                if (NodeCurrentCommand.ContainsKey(itemReference.ID))
                {
                    Globals.SendCommand(NodeCurrentCommand[itemReference.ID], itemReference, true);
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
            else if (cmbBox.SelectedValue.ToString() == "All") DeviceListFilters[0] = p => true;
            else DeviceListFilters[0] = p => ((NodeModel)p).Category == cmbBox.SelectedValue.ToString();
            DevicesListView.Items.Filter = p => CombinedFilterPredicate((NodeModel)p);
        }

        private void FloorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DevicesListView == null) return;
            var cmbBox = (ComboBox)sender;
            if (cmbBox.SelectedIndex == -1) DeviceListFilters[1] = p => true;
            else if (cmbBox.SelectedItem.ToString() == "All") DeviceListFilters[1] = p => true;
            else DeviceListFilters[1] = p => ((NodeModel)p).Floor?.Name == cmbBox.SelectedItem.ToString();
            DevicesListView.Items.Filter = p => CombinedFilterPredicate((NodeModel)p);
        }

        private void StatusSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DevicesListView == null) return;
            var cmbBox = (ComboBox)sender;
            if (cmbBox.SelectedIndex == -1) OnTransitionRequested("total");
            switch (cmbBox.SelectedValue.ToString())
            {
                case "All":
                    OnTransitionRequested("total");
                    break;
                case "Online":
                    OnTransitionRequested("online");
                    break;
                case "Offline":
                    OnTransitionRequested("offline");
                    break;
                case "Not Configured":
                    OnTransitionRequested("new");
                    break;
            }
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

        private void OnTransitionRequested(string data)
        {
            TransitionRequested?.Invoke("devicelist", data);
        }

        private async void DevicesListViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var listview = (ListView)sender;
            var gridview = (GridView)listview.View;
            double percentTotal = 0;
            double fixedWidth = 0;
            for (int i = 0; i < gridview.Columns.Count; i++)
            {
                var percentWidth = gridview.Columns[i].ReadLocalValue(ListViewAssists.ColumnWidthPercentageProperty);
                if (percentWidth == DependencyProperty.UnsetValue)
                {
                    var minWidth = gridview.Columns[i].ReadLocalValue(ListViewAssists.ColumnMinWidthProperty);
                    if (minWidth == DependencyProperty.UnsetValue) gridview.Columns[i].Width = Double.NaN;
                    else gridview.Columns[i].Width = (double)minWidth;
                }
            }
            await Task.Delay(10);
            for (int i = 0; i < gridview.Columns.Count; i++)
            {
                var percentWidth = gridview.Columns[i].ReadLocalValue(ListViewAssists.ColumnWidthPercentageProperty);
                if (percentWidth == DependencyProperty.UnsetValue)
                {
                    fixedWidth += gridview.Columns[i].ActualWidth;
                }
                else
                {
                    percentTotal += (double)percentWidth;
                }
            }
            var availableWidth = e.NewSize.Width - fixedWidth - 20;
            for (int i = 0; i < gridview.Columns.Count; i++)
            {
                var percentWidth = gridview.Columns[i].ReadLocalValue(ListViewAssists.ColumnWidthPercentageProperty);
                if (percentWidth != DependencyProperty.UnsetValue)
                {
                    var ratioActual = (double)percentWidth / percentTotal;
                    var calculatedWidth = ratioActual * availableWidth;
                    var minWidth = gridview.Columns[i].ReadLocalValue(ListViewAssists.ColumnMinWidthProperty);
                    if (minWidth != DependencyProperty.UnsetValue)
                    {
                        if (calculatedWidth < (double)minWidth) gridview.Columns[i].Width = (double)minWidth;
                        else gridview.Columns[i].Width = calculatedWidth;
                    }
                    else gridview.Columns[i].Width = calculatedWidth;
                }
            }
        }
    }
}
