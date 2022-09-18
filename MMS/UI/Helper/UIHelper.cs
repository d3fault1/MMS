using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MMS.UI.Assists;

namespace MMS.UI.Helpers
{
    public static class UIHelper
    {
        public static async void UniformGridViewColumnSize(GridView gridview, SizeChangedEventArgs e)
        {
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
                    availableWidth -= gridview.Columns[i].Width;
                    percentTotal -= (double)percentWidth;
                }
            }
        }

        public static FrameworkElement GetVisualChild(FrameworkElement parent, string name)
        {
            var childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = (FrameworkElement)VisualTreeHelper.GetChild(parent, i);
                if (child.Name == name) return child;
                var nextChild = GetVisualChild(child, name);
                if (nextChild != default) return nextChild;
            }
            return default;
        }
    }
}
