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
using MMS.UI.Views.AppUserControls;

namespace MMS.UI.Views.AppPages
{
    /// <summary>
    /// Interaction logic for CommandLog.xaml
    /// </summary>
    public partial class CommandLog : Page
    {
        private List<Predicate<object>> CommandLogFilters { get; }
        public CommandLog()
        {
            CommandLogFilters = new List<Predicate<object>>(3);
            CommandLogFilters.Add(p => true);
            CommandLogFilters.Add(p => true);
            CommandLogFilters.Add(p => true);
            InitializeComponent();

            FloorOptions.ItemsSource = DataHub.Floors.Select(a => a.Name).ToList().Append("All");
            CommandOptions.ItemsSource = DataHub.Commands.Select(a => a.Command).ToList().Append("All");

            CommandLogListView.Items.IsLiveFiltering = true;
            CommandLogListView.Items.LiveFilteringProperties.Add(nameof(FloorModel.Name));
            CommandLogListView.Items.LiveFilteringProperties.Add(nameof(CommandModel.Command));
        }

        private void CommandLogListViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var listview = (ListView)sender;
            var gridview = (GridView)listview.View;
            UIHelper.UniformGridViewColumnSize(gridview, e);
            Console.WriteLine(e.NewSize);
        }

        private void FloorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CommandLogListView == null) return;
            var cmbBox = (ComboBox)sender;
            if (cmbBox.SelectedIndex == -1) CommandLogFilters[0] = p => true;
            else if (cmbBox.SelectedItem.ToString() == "All") CommandLogFilters[0] = p => true;
            else CommandLogFilters[0] = p => ((CommandLogModel)p).Node?.Floor?.Name == cmbBox.SelectedItem.ToString();
            CommandLogListView.Items.Filter = p => CombinedFilterPredicate((CommandLogModel)p);
        }

        private void CommandSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CommandLogListView == null) return;
            var cmbBox = (ComboBox)sender;
            if (cmbBox.SelectedIndex == -1) CommandLogFilters[1] = p => true;
            else if (cmbBox.SelectedItem.ToString() == "All") CommandLogFilters[1] = p => true;
            else CommandLogFilters[1] = p => ((CommandLogModel)p).Command?.Command == cmbBox.SelectedItem.ToString();
            CommandLogListView.Items.Filter = p => CombinedFilterPredicate((CommandLogModel)p);
        }

        private void DateRangeChanged(object sender, DateRangeChangedEventArgs e)
        {
            if (CommandLogListView == null) return;
            bool frombool, tobool;
            if (e.IsEnabled)
            {
                CommandLogFilters[2] = p =>
                {
                    if (!e.FromDate.HasValue) frombool = true;
                    else frombool = ((CommandLogModel)p).UpdatedAt >= e.FromDate.Value;

                    if (!e.ToDate.HasValue) tobool = true;
                    else tobool = ((CommandLogModel)p).UpdatedAt <= e.ToDate.Value;

                    return frombool && tobool;
                };
            }
            else
            {
                CommandLogFilters[2] = p => true;
            }
        }

        private bool CombinedFilterPredicate(CommandLogModel p)
        {
            foreach (var predicate in CommandLogFilters)
            {
                if (!predicate.Invoke(p)) return false;
            }
            return true;
        }
    }
}
