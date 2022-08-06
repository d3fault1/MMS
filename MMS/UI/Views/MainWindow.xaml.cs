using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MMS.Backend;
using System.Windows.Media.Animation;

namespace MMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Application.Current.Exit += AppExit;
            SidePanel.NavigationRequested += SidePanelNavigationRequested;
            //Globals.Initialize();
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

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void FrameNavigating(object sender, NavigatingCancelEventArgs e)
        {
            ColorAnimation animationMain = new ColorAnimation();
            ColorAnimation animationAux = new ColorAnimation();
            animationMain.Duration = (Duration)Application.Current.Resources["DurationFadeIn"];
            animationAux.Duration = (Duration)Application.Current.Resources["DurationFadeIn"];
            switch (e.Uri.ToString())
            {                
                case "UI/Views/AppPages/TitlePage.xaml":
                case "UI/Views/AppPages/Dashboard.xaml":
                    animationMain.To = (Color)Application.Current.Resources["ColorPrimaryMain1"];
                    animationAux.To = (Color)Application.Current.Resources["ColorPrimaryAux1"];
                    break;
                case "UI/Views/AppPages/DeviceList.xaml":
                    animationMain.To = (Color)Application.Current.Resources["ColorPrimaryMain2"];
                    animationAux.To = (Color)Application.Current.Resources["ColorPrimaryAux2"];
                    break;
            }
            ((LinearGradientBrush)AppWindow.Background).GradientStops[0].BeginAnimation(GradientStop.ColorProperty, animationMain);
            ((LinearGradientBrush)AppWindow.Background).GradientStops[1].BeginAnimation(GradientStop.ColorProperty, animationAux);
        }

        private void SidePanelNavigationRequested(Uri TargetPage)
        {
            //DoubleAnimation animationFadeOut = new DoubleAnimation(0, (Duration)Application.Current.Resources["DurationFadeOut"]);
            //((Page)PagePresenter.Content)?.BeginAnimation(Page.OpacityProperty, animationFadeOut);
            PagePresenter.Navigate(TargetPage);
        }
    }
}
