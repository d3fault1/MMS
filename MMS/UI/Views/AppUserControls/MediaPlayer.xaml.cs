using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using MMS.DataModels;
using MMS.Backend;
using System.Linq;
using System.Windows.Data;
using System.Windows.Navigation;
using MMS.UI.BindingConverters;
using System;

namespace MMS.UI.Views.AppUserControls
{
    /// <summary>
    /// Interaction logic for MediaPlayer.xaml
    /// </summary>
    public partial class MediaPlayer : UserControl
    {
        public static readonly DependencyProperty NodeProperty = DependencyProperty.Register("Node", typeof(NodeModel), typeof(MediaPlayer), new PropertyMetadata(null));
        public static readonly DependencyProperty IsPlayingProperty = DependencyProperty.Register("IsPlaying", typeof(bool), typeof(MediaPlayer), new PropertyMetadata(false));
        public static readonly DependencyProperty IsMuteProperty = DependencyProperty.Register("IsMute", typeof(bool), typeof(MediaPlayer), new PropertyMetadata(false));
        public static readonly DependencyProperty UserVolumeProperty = DependencyProperty.Register("UserVolume", typeof(int), typeof(MediaPlayer), new PropertyMetadata(100));
        public static readonly DependencyProperty UserTimestampProperty = DependencyProperty.Register("UserTimestamp", typeof(int), typeof(MediaPlayer), new PropertyMetadata(0));

        public NodeModel Node
        {
            get
            {
                return (NodeModel)GetValue(NodeProperty);
            }
            set
            {
                SetValue(NodeProperty, value);
            }
        }
        public bool IsPlaying
        {
            get
            {
                return (bool)GetValue(IsPlayingProperty);
            }
            set
            {
                SetValue(IsPlayingProperty, value);
            }
        }
        public bool IsMute
        {
            get
            {
                return (bool)GetValue(IsMuteProperty);
            }
            set
            {
                SetValue(IsMuteProperty, value);
            }
        }
        public int UserVolume
        {
            get
            {
                return (int)GetValue(UserVolumeProperty);
            }
            set
            {
                SetValue(UserVolumeProperty, value);
            }
        }
        public int UserTimestamp
        {
            get
            {
                return (int)GetValue(UserTimestampProperty);
            }
            set
            {
                SetValue(UserTimestampProperty, value);
            }
        }

        private bool timeupdatebyheartbeat = false;
        private bool volupdatebyheartbeat = false;
        private int backupvol = 0;

        public MediaPlayer()
        {
            InitializeComponent();
            HTTPHandler.Instance.HeartbeatReceived += NodeHeartbeatReceived;
        }

        private void MediaPlayerLoaded(object sender, RoutedEventArgs e)
        {
            Binding br = new Binding("IsOnline");
            br.Source = Node;
            mainBorder.SetBinding(Border.IsEnabledProperty, br);

            Binding vn = new Binding("VideoName");
            vn.Source = Node.CurrentStatus;
            vn.Converter = new PlaceholderTextBindingConverter();
            vn.ConverterParameter = "Not Playing";
            VideoName.SetBinding(TextBlock.TextProperty, vn);

            Binding eb = new Binding("VideoName");
            eb.Source = Node.CurrentStatus;
            eb.Converter = new PlaceholderTextBindingConverter();
            eb.ConverterParameter = "N/A";
            Exhibit.SetBinding(TextBlock.TextProperty, eb);

            Binding mx = new Binding("VideoDuration");
            mx.Source = Node.CurrentStatus;
            playerSlider.SetBinding(Slider.MaximumProperty, mx);
        }

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

        private void volumeButtonClick(object sender, RoutedEventArgs e)
        {
            var command = DataHub.Commands.FirstOrDefault(a => a.CommandNumber == (int)CommandNumber.VolumeValue);
            if (IsMute)
            {
                UserVolume = backupvol;
                command?.Prepare();
                command?.Parameters.Add(UserVolume.ToString());
                Globals.SendCommand(command, Node, true);
            }
            else
            {
                backupvol = UserVolume;
                UserVolume = 0;
                command?.Prepare();
                command?.Parameters.Add(UserVolume.ToString());
                Globals.SendCommand(command, Node, true);
            }
            IsMute = !IsMute;
        }

        private void playButtonClick(object sender, RoutedEventArgs e)
        {    
            if (IsPlaying)
            {
                var command = DataHub.Commands.FirstOrDefault(a => a.CommandNumber == (int)CommandNumber.Halt);
                command?.Prepare();
                Globals.SendCommand(command, Node, true);
            }
            else
            {
                var command = DataHub.Commands.FirstOrDefault(a => a.CommandNumber == (int)CommandNumber.Play);
                command?.Prepare();
                Globals.SendCommand(command, Node, true);
            }
            IsPlaying = !IsPlaying;
        }

        private void restartButtonClick(object sender, RoutedEventArgs e)
        {
            var command = DataHub.Commands.FirstOrDefault(a => a.CommandNumber == (int)CommandNumber.Reset);
            command?.Prepare();
            Globals.SendCommand(command, Node, true);
        }

        private void nextButtonClick(object sender, RoutedEventArgs e)
        {
            var command = DataHub.Commands.FirstOrDefault(a => a.CommandNumber == (int)CommandNumber.Next);
            command?.Prepare();
            Globals.SendCommand(command, Node, true);
        }

        private void prevButtonClick(object sender, RoutedEventArgs e)
        {
            var command = DataHub.Commands.FirstOrDefault(a => a.CommandNumber == (int)CommandNumber.Previous);
            command?.Prepare();
            Globals.SendCommand(command, Node, true);
        }

        private void VolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!volupdatebyheartbeat)
            {
                var command = DataHub.Commands.FirstOrDefault(a => a.CommandNumber == (int)CommandNumber.VolumeValue);
                command?.Prepare();
                command?.Parameters.Add(e.NewValue.ToString());
                Globals.SendCommand(command, Node, true);
                IsMute = (int)e.NewValue == 0;
            }
            else volupdatebyheartbeat = false;
        }

        private void PlayerTimeStampChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!timeupdatebyheartbeat)
            {
                var command = DataHub.Commands.FirstOrDefault(a => a.CommandNumber == (int)CommandNumber.GotoTime);
                command?.Prepare();
                command?.Parameters.Add(e.NewValue.ToString());
                Globals.SendCommand(command, Node, true);
            }
            else timeupdatebyheartbeat = false;
        }

        private void NodeHeartbeatReceived(NodeCurrentStatusModel status, string mac)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Node?.MacAddress == mac)
                {
                    IsPlaying = status.VideoStatus == "run";
                    volupdatebyheartbeat = true;
                    UserVolume = status.Volume;
                    IsMute = status.Volume == 0;
                    timeupdatebyheartbeat = true;
                    UserTimestamp = (int)status.TimeStamp.TotalSeconds;
                }
            });        
        }
    }
}
