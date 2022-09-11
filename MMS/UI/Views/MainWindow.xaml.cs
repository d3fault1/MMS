using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Hardcodet.Wpf.TaskbarNotification;
using MMS.Backend;
using MMS.UI.Views.AppPages;

namespace MMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TaskbarIcon taskbarIcon;
        public MainWindow()
        {
            InitializeComponent();
            taskbarIcon = new TaskbarIcon();
            taskbarIcon.Visibility = Visibility.Collapsed;
            Application.Current.Exit += AppExit;
            SidePanel.NavigationRequested += NavigationRequested;
            Globals.PushNotifyRequested += PushNotifyRequested;
            //Globals.Initialize();
        }

        private async void PushNotifyRequested(string title, string message, string type)
        {
            taskbarIcon.Visibility = Visibility.Visible;
            switch (type)
            {
                case "error":
                    taskbarIcon.ShowBalloonTip(title, message, BalloonIcon.Error);
                    break;
                case "warning":
                    taskbarIcon.ShowBalloonTip(title, message, BalloonIcon.Warning);
                    break;
                case "info":
                    taskbarIcon.ShowBalloonTip(title, message, BalloonIcon.Info);
                    break;
                case "none":
                    taskbarIcon.ShowBalloonTip(title, message, BalloonIcon.None);
                    break;
            }   
            await Task.Delay(3000);
            taskbarIcon.HideBalloonTip();
            taskbarIcon.Visibility = Visibility.Collapsed;
        }

        private void AppExit(object sender, ExitEventArgs e)
        {
            //Globals.Destroy();
        }

        private void WindowTopBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MinimizeClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized) WindowState = WindowState.Normal;
            else WindowState = WindowState.Maximized;
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void FrameNavigated(object sender, NavigationEventArgs e)
        {
            switch (e.Uri.ToString())
            {
                case "UI/Views/AppPages/TitlePage.xaml":
                    break;
                case "UI/Views/AppPages/Dashboard.xaml":
                    ((Dashboard)e.Content).NavigationRequested += NavigationRequested;
                    break;
                case "UI/Views/AppPages/DeviceList.xaml":
                    ((DeviceList)e.Content).TransitionRequested += TransitionRequested;
                    var page = (DeviceList)e.Content;
                    switch (e.ExtraData?.ToString())
                    {
                        case "online":
                            page.StatusOptions.SelectedIndex = 0;
                            break;
                        case "offline":
                            page.StatusOptions.SelectedIndex = 1;
                            break;
                        case "new":
                            page.StatusOptions.SelectedIndex = 2;
                            break;
                        case "total":
                            page.StatusOptions.SelectedIndex = 3;
                            break;
                    }
                    break;
            }
        }

        private void TransitionRequested(string page, string data)
        {
            ColorAnimation animationMain = new ColorAnimation();
            ColorAnimation animationAux = new ColorAnimation();
            animationMain.Duration = (Duration)Application.Current.Resources["DurationFadeIn"];
            animationAux.Duration = (Duration)Application.Current.Resources["DurationFadeIn"];
            switch (data)
            {
                case "online":
                    animationMain.To = (Color)Application.Current.Resources["ColorPrimaryMain3"];
                    animationAux.To = (Color)Application.Current.Resources["ColorPrimaryAux3"];
                    break;
                case "offline":
                    animationMain.To = (Color)Application.Current.Resources["ColorPrimaryMain4"];
                    animationAux.To = (Color)Application.Current.Resources["ColorPrimaryAux4"];
                    break;
                case "new":
                    animationMain.To = (Color)Application.Current.Resources["ColorPrimaryMain5"];
                    animationAux.To = (Color)Application.Current.Resources["ColorPrimaryAux5"];
                    break;
                case "total":
                    animationMain.To = (Color)Application.Current.Resources["ColorPrimaryMain2"];
                    animationAux.To = (Color)Application.Current.Resources["ColorPrimaryAux2"];
                    break;
            }
            ((LinearGradientBrush)AppWindow.Background).GradientStops[0].BeginAnimation(GradientStop.ColorProperty, animationMain);
            ((LinearGradientBrush)AppWindow.Background).GradientStops[1].BeginAnimation(GradientStop.ColorProperty, animationAux);
        }

        private void NavigationRequested(Uri TargetPage, string from, string param)
        {
            ColorAnimation animationMain = new ColorAnimation();
            ColorAnimation animationAux = new ColorAnimation();
            animationMain.Duration = (Duration)Application.Current.Resources["DurationFadeIn"];
            animationAux.Duration = (Duration)Application.Current.Resources["DurationFadeIn"];
            switch (TargetPage.ToString())
            {
                case "/UI/Views/AppPages/TitlePage.xaml":
                    animationMain.To = (Color)Application.Current.Resources["ColorPrimaryMain1"];
                    animationAux.To = (Color)Application.Current.Resources["ColorPrimaryAux1"];
                    break;
                case "/UI/Views/AppPages/Dashboard.xaml":
                    animationMain.To = (Color)Application.Current.Resources["ColorPrimaryMain1"];
                    animationAux.To = (Color)Application.Current.Resources["ColorPrimaryAux1"];
                    break;
                case "/UI/Views/AppPages/DeviceList.xaml":
                    switch (param)
                    {
                        case "online":
                            animationMain.To = (Color)Application.Current.Resources["ColorPrimaryMain3"];
                            animationAux.To = (Color)Application.Current.Resources["ColorPrimaryAux3"];
                            break;
                        case "offline":
                            animationMain.To = (Color)Application.Current.Resources["ColorPrimaryMain4"];
                            animationAux.To = (Color)Application.Current.Resources["ColorPrimaryAux4"];
                            break;
                        case "new":
                            animationMain.To = (Color)Application.Current.Resources["ColorPrimaryMain5"];
                            animationAux.To = (Color)Application.Current.Resources["ColorPrimaryAux5"];
                            break;
                        case "total":
                            animationMain.To = (Color)Application.Current.Resources["ColorPrimaryMain2"];
                            animationAux.To = (Color)Application.Current.Resources["ColorPrimaryAux2"];
                            break;
                    }
                    break;
            }
            ((LinearGradientBrush)AppWindow.Background).GradientStops[0].BeginAnimation(GradientStop.ColorProperty, animationMain);
            ((LinearGradientBrush)AppWindow.Background).GradientStops[1].BeginAnimation(GradientStop.ColorProperty, animationAux);
            PagePresenter.Navigate(TargetPage, param);
        }

        private void MainWindowStateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized) AppWindow.CornerRadius = new CornerRadius(0);
            else AppWindow.CornerRadius = (CornerRadius)Application.Current.Resources["CorderRadiusLarge"];
        }
    }
}
