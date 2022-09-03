using System;
using System.Windows;
using System.Windows.Controls;

namespace MMS.UI.Views.AppUserControls
{
    /// <summary>
    /// Interaction logic for DateRange.xaml
    /// </summary>
    public partial class DateRange : UserControl
    {
        public delegate void DateRangeChangedEventHandler(object sender, DateRangeChangedEventArgs e);
        public event DateRangeChangedEventHandler DateRangeChanged;

        private bool Applied;
        public DateRange()
        {
            InitializeComponent();
        }

        private void EnabledChecked(object sender, RoutedEventArgs e)
        {
            Applied = true;
            OnDateRangeChanged();
        }

        private void EnabledUnchecked(object sender, RoutedEventArgs e)
        {
            Applied = false;
            OnDateRangeChanged();
        }

        private void FromDateChanged(object sender, SelectionChangedEventArgs e)
        {
            OnDateRangeChanged();
        }

        private void ToDateChanged(object sender, SelectionChangedEventArgs e)
        {
            OnDateRangeChanged();
        }

        private void OnDateRangeChanged()
        {
            DateRangeChangedEventArgs eventArgs = new DateRangeChangedEventArgs(Applied, From.SelectedDate, To.SelectedDate);
            DateRangeChanged?.Invoke(this, eventArgs);
        }
    }

    public struct DateRangeChangedEventArgs
    {
        public bool IsEnabled { get; }
        public DateTime? FromDate { get; }
        public DateTime? ToDate { get; }
        public DateRangeChangedEventArgs(bool enabled, DateTime? from, DateTime? to)
        {
            IsEnabled = enabled;
            FromDate = from;
            ToDate = to;
        }
    }
}
