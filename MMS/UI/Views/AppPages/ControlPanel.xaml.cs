using MMS.UI.Helpers;
using MMS.UI.Views.AppUserControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MMS.DataModels;

namespace MMS.UI.Views.AppPages
{
    /// <summary>
    /// Interaction logic for ControlPanel.xaml
    /// </summary>
    public partial class ControlPanel : Page
    {
        MediaPlayer player;
        UniformGrid grid;
        public ControlPanel()
        {
            InitializeComponent();
            DeviceListView.Items.IsLiveFiltering = true;
            DeviceListView.Items.LiveFilteringProperties.Add(nameof(NodeModel.IsConfig));
            DeviceListView.Items.Filter = p => ((NodeModel)p).IsConfig;
        }

        private void DeviceListViewLoaded(object sender, RoutedEventArgs e)
        {
            player = (MediaPlayer)UIHelper.GetVisualChild(DeviceListView, "mediaPlayer");
            grid = (UniformGrid)UIHelper.GetVisualChild(DeviceListView, "panelGrid");
            ResizeItems();
        }

        private void DeviceListViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeItems();
        }

        private void ResizeItems()
        {
            if (grid != null && player != null)
            {
                var cols = (int)(DeviceListView.ActualWidth / player.MinWidth);
                if (cols < 1) cols = 1;
                if (grid.Columns != cols)
                {
                    if ((double)DeviceListView.Items.Count / grid.Columns > 1) grid.Columns = cols;
                    else if (cols < grid.Columns) grid.Columns = cols;
                }
            }
        }
    }
}
