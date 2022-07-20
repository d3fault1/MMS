using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataModels
{
    class NodeCurrentStatusModel : INotifyPropertyChanged
    {
        #region Private Variables
        private long _nodeid = -1;
        private double _temperature = 0;
        private int _processorusage = 0;
        private int _diskspaceusage = 0;
        private int _ramusage = 0;
        private TimeSpan _timestamp = TimeSpan.Zero;
        private string _videoname = "";
        private int _videonum = 0;
        private string _videostatus = "";
        private int _volume = 0;
        private double _videoduration = 0;
        private int _totalvideos = 0;
        private string[] _videolist = new string[0];
        private TimeSpan _uptime = TimeSpan.Zero;
        private string _version = "";
        private DateTime _createdat = DateTime.MinValue;
        private DateTime _updatedat = DateTime.MinValue;
        #endregion

        public long NodeID
        {
            get
            {
                return _nodeid;
            }
            set
            {
                _nodeid = value;
                OnPropertyChanged(nameof(NodeID));
            }
        }
        public double Temperature
        {
            get
            {
                return _temperature;
            }
            set
            {
                _temperature = value;
                OnPropertyChanged(nameof(Temperature));
            }
        }
        public int ProcessorUsage
        {
            get
            {
                return _processorusage;
            }
            set
            {
                _processorusage = value;
                OnPropertyChanged(nameof(ProcessorUsage));
            }
        }
        public int DiskSpaceUsage
        {
            get
            {
                return _diskspaceusage;
            }
            set
            {
                _diskspaceusage = value;
                OnPropertyChanged(nameof(DiskSpaceUsage));
            }
        }
        public int RamUsage
        {
            get
            {
                return _ramusage;
            }
            set
            {
                _ramusage = value;
                OnPropertyChanged(nameof(RamUsage));
            }
        }
        public TimeSpan TimeStamp
        {
            get
            {
                return _timestamp;
            }
            set
            {
                _timestamp = value;
                OnPropertyChanged(nameof(TimeStamp));
            }
        }
        public string VideoName
        {
            get
            {
                return _videoname;
            }
            set
            {
                _videoname = value;
                OnPropertyChanged(nameof(VideoName));
            }
        }
        public int VideoNumber
        {
            get
            {
                return _videonum;
            }
            set
            {
                _videonum = value;
                OnPropertyChanged(nameof(VideoNumber));
            }
        }
        public string VideoStatus
        {
            get
            {
                return _videostatus;
            }
            set
            {
                _videostatus = value;
                OnPropertyChanged(nameof(VideoStatus));
            }
        }
        public int Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = value;
                OnPropertyChanged(nameof(Volume));
            }
        }
        public double VideoDuration
        {
            get
            {
                return _videoduration;
            }
            set
            {
                _videoduration = value;
                OnPropertyChanged(nameof(VideoDuration));
            }
        }
        public int TotalVideos
        {
            get
            {
                return _totalvideos;
            }
            set
            {
                _totalvideos = value;
                OnPropertyChanged(nameof(TotalVideos));
            }
        }
        public string[] VideoList
        {
            get
            {
                return _videolist;
            }
            set
            {
                _videolist = value;
                OnPropertyChanged(nameof(VideoList));
            }
        }
        public TimeSpan Uptime
        {
            get
            {
                return _uptime;
            }
            set
            {
                _uptime = value;
                OnPropertyChanged(nameof(Uptime));
            }
        }
        public string Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
                OnPropertyChanged(nameof(Version));
            }
        }
        public DateTime CreatedAt
        {
            get
            {
                return _createdat;
            }
            set
            {
                _createdat = value;
                OnPropertyChanged(nameof(CreatedAt));
            }
        }
        public DateTime UpdatedAt
        {
            get
            {
                return _updatedat;
            }
            set
            {
                _updatedat = value;
                OnPropertyChanged(nameof(UpdatedAt));
            }
        }

        #region Notify Event and Functions
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
