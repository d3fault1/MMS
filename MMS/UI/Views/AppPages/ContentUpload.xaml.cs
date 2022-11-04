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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MMS.Backend;
using MMS.DataModels;
using MMS.UI.Helpers;

namespace MMS.UI.Views.AppPages
{
    /// <summary>
    /// Interaction logic for ContentUpload.xaml
    /// </summary>
    public partial class ContentUpload : Page
    {
        private List<Predicate<object>> DeviceFilters { get; }

        public ContentUpload()
        {
            DeviceFilters = new List<Predicate<object>>(1);
            DeviceFilters.Add(p => true);
            InitializeComponent();

            deviceOptions.ItemsSource = DataHub.Nodes;

            deviceOptions.Items.IsLiveFiltering = true;
            deviceOptions.Items.LiveFilteringProperties.Add(nameof(NodeModel.Name));
            deviceOptions.Items.Filter = p => CombinedFilterPredicate((NodeModel)p);
        }

        private void ContentDetailsListViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var listview = (ListView)sender;
            var gridview = (GridView)listview.View;
            UIHelper.UniformGridViewColumnSize(gridview, e);
        }

        private void deviceOptionsTextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(deviceOptions.Text)) DeviceFilters[0] = p => true;
            else DeviceFilters[0] = p => ((NodeModel)p).Name.Contains(deviceOptions.Text);
        }

        private void deviceOptionsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (deviceOptions.SelectedItem != null)
            {
                ContentDetailsView.ItemsSource = ((NodeModel)deviceOptions.SelectedItem).Files;
                Uploader.Node = (NodeModel)deviceOptions.SelectedItem;
            }
        }

        private void UpButtonClick(object sender, RoutedEventArgs e)
        {
            var node = ((NodeModel)deviceOptions.SelectedItem);
            foreach (var item in ContentDetailsView.SelectedItems)
            {
                var index = node.Files.IndexOf((NodeFileModel)item);
                if (index > 0)
                {
                    node.Files.Move(index, index - 1);
                    index = node.Files.IndexOf((NodeFileModel)item);
                    node.Files[index].Position = index;
                }
            }
        }

        private void DownButtonClick(object sender, RoutedEventArgs e)
        {
            var node = ((NodeModel)deviceOptions.SelectedItem);
            foreach (var item in ContentDetailsView.SelectedItems)
            {
                var index = node.Files.IndexOf((NodeFileModel)item);
                if (index < node.Files.Count - 1)
                {
                    node.Files.Move(index, index + 1);
                    index = node.Files.IndexOf((NodeFileModel)item);
                    node.Files[index].Position = index;
                }
            }
        }

        private bool CombinedFilterPredicate(NodeModel p)
        {
            foreach (var predicate in DeviceFilters)
            {
                if (!predicate.Invoke(p)) return false;
            }
            return true;
        }
    }
}
