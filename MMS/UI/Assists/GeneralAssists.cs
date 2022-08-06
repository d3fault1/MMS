using System.Windows;

namespace MMS.UI.Assists
{
    public static class GeneralAssists
    {
        public static readonly DependencyProperty HintTextProperty = DependencyProperty.RegisterAttached("HintText", typeof(string), typeof(GeneralAssists), new PropertyMetadata(""));

        public static string GetHintText(DependencyObject element) => (string)element.GetValue(HintTextProperty);
        public static void SetHintText(DependencyObject element, string value) => element.SetValue(HintTextProperty, value);
    }
}
