using MMS.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace MMS.UI.Views.AppUserControls
{
    /// <summary>
    /// Interaction logic for MediaPlayer.xaml
    /// </summary>
    public partial class MediaPlayer : UserControl, INotifyPropertyChanged
    {
        private bool isplaying = false;
        private bool ismute = false;
        private NodeModel node;

        public NodeModel Node
        {
            get
            {
                return node;
            }
            set
            {
                node = value;
                OnPropertyChanged(nameof(Node));
            }
        }
        public bool IsPlaying
        {
            get
            {
                return isplaying;
            }
            set
            {
                isplaying = value;
                OnPropertyChanged(nameof(IsPlaying));
            }
        }
        public bool IsMute
        {
            get
            {
                return ismute;
            }
            set
            {
                ismute = value;
                OnPropertyChanged(nameof(IsMute));
            }
        }

        public MediaPlayer()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void volumeButtonMouseEnter(object sender, MouseEventArgs e)
        {
            if (!volumeSliderPopup.IsOpen) volumeSliderPopup.IsOpen = true;
        }

        private void volumeButtonMouseLeave(object sender, MouseEventArgs e)
        {
            if (volumeSliderPopup.IsOpen)
            {
                if (!volumeSliderPopup.IsMouseOver) volumeSliderPopup.IsOpen = false;
            }
        }

        private void volumeSliderPopupMouseLeave(object sender, MouseEventArgs e)
        {
            volumeSliderPopup.IsOpen = false;
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void volumeButtonClick(object sender, RoutedEventArgs e)
        {
            IsMute = !IsMute;
        }

        private void playButtonClick(object sender, RoutedEventArgs e)
        {
            IsPlaying = !IsPlaying;
        }

        private void restartButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void nextButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void prevButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
