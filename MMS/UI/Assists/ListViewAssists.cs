using System.Windows;

namespace MMS.UI.Assists
{
    public static class ListViewAssists
    {
        public static readonly DependencyProperty CanResizeColumnsProperty = DependencyProperty.RegisterAttached("CanResizeColumns", typeof(bool), typeof(ListViewAssists), new PropertyMetadata(false));

        public static bool GetCanResizeColumns(DependencyObject element) => (bool)element.GetValue(CanResizeColumnsProperty);
        public static void SetCanResizeColumns(DependencyObject element, bool value) => element.SetValue(CanResizeColumnsProperty, value);


        public static readonly DependencyProperty ColumnWidthPercentageProperty = DependencyProperty.RegisterAttached("ColumnWidthPercentage", typeof(double), typeof(ListViewAssists), new PropertyMetadata(100.00));

        public static double GetColumnWidthPercentage(DependencyObject element) => (double)element.GetValue(ColumnWidthPercentageProperty);
        public static void SetColumnWidthPercentage(DependencyObject element, double value) => element.SetValue(ColumnWidthPercentageProperty, value);



        public static readonly DependencyProperty ColumnMinWidthProperty = DependencyProperty.RegisterAttached("ColumnMinWidth", typeof(double), typeof(ListViewAssists), new PropertyMetadata());

        public static double GetColumnMinWidth(DependencyObject element) => (double)element.GetValue(ColumnMinWidthProperty);
        public static void SetColumnMinWidth(DependencyObject element, double value) => element.SetValue(ColumnMinWidthProperty, value);


        public static readonly DependencyProperty ColumnMaxWidthProperty = DependencyProperty.RegisterAttached("ColumnMaxWidth", typeof(double), typeof(ListViewAssists), new PropertyMetadata());

        public static double GetColumnMaxWidth(DependencyObject element) => (double)element.GetValue(ColumnMaxWidthProperty);
        public static void SetColumnMaxWidth(DependencyObject element, double value) => element.SetValue(ColumnMaxWidthProperty, value);
    }
}
