using System.Windows;

namespace MMS.UI.Assists
{
    public static class ListViewAssists
    {
        public static readonly DependencyProperty CanResizeColumnsProperty = DependencyProperty.RegisterAttached("CanResizeColumns", typeof(bool), typeof(ListViewAssists), new PropertyMetadata(false));

        public static bool GetCanResizeColumns(DependencyObject element) => (bool)element.GetValue(CanResizeColumnsProperty);
        public static void SetCanResizeColumns(DependencyObject element, bool value) => element.SetValue(CanResizeColumnsProperty, value);
    }
}
