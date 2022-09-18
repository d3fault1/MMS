using System;
using System.ComponentModel;

namespace MMS.DataModels
{
    public class NodeLogModel : INotifyPropertyChanged
    {
        #region Private Variables
        private long _nodeid = -1;
        private TimeSpan _uptime = TimeSpan.Zero;
        private double _temperature = 0;
        private int _processorusage = 0;
        private int _diskspaceusage = 0;
        private int _ramusage = 0;
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
