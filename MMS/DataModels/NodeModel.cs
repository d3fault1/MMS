using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMS.Backend;

namespace MMS.DataModels
{
    class NodeModel : INotifyPropertyChanged
    {
        #region Private Variables
        private long _id = -1;
        private string _name = "";
        private string _nodename = "";
        private string _description = "";
        private string _ip = "";
        private string _macaddress = "";
        private int _port = -1;
        private bool _isactive = false;
        private bool _isconfig = false;
        private string _ostype = "";
        private string _osname = "";
        private string _osarch = "";
        private string _regkey = "";
        private double _totaldiskspace = 0;
        private double _totalcpu = 0;
        private double _totalram = 0;
        private long _exhibitid = -1;
        private long _zoneid = -1;
        private long _floorid = -1;
        private string _contentmetadata = "";
        private string _pemfile = "";
        private int _heartrate = 0;
        private string _image = "";
        private string _category = "";
        private int _seqid = -1;
        private bool _isonline = false;
        private bool _isaudioguide = false;
        private bool _iscontrolpanel = false;
        private DateTime _createdat = DateTime.MinValue;
        private DateTime _updatedat = DateTime.MinValue;
        private NodeCurrentStatusModel _currentstatus = new NodeCurrentStatusModel();
        #endregion

        public long ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(ID));
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string NodeName
        {
            get
            {
                return _nodename;
            }
            set
            {
                _nodename = value;
                OnPropertyChanged(nameof(NodeName));
            }
        }
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        public string IP
        {
            get
            {
                return _ip;
            }
            set
            {
                _ip = value;
                OnPropertyChanged(nameof(IP));
            }
        }
        public string MacAddress
        {
            get
            {
                return _macaddress;
            }
            set
            {
                _macaddress = value;
                OnPropertyChanged(nameof(MacAddress));
            }
        }
        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
                OnPropertyChanged(nameof(Port));
            }
        }
        public bool IsActive
        {
            get
            {
                return _isactive;
            }
            set
            {
                _isactive = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }
        public bool IsConfig
        {
            get
            {
                return _isconfig;
            }
            set
            {
                _isconfig = value;
                OnPropertyChanged(nameof(IsConfig));
            }
        }
        public string OSType
        {
            get
            {
                return _ostype;
            }
            set
            {
                _ostype = value;
                OnPropertyChanged(nameof(OSType));
            }
        }
        public string OSName
        {
            get
            {
                return _osname;
            }
            set
            {
                _osname = value;
                OnPropertyChanged(nameof(OSName));
            }
        }
        public string OSArchitecture
        {
            get
            {
                return _osarch;
            }
            set
            {
                _osarch = value;
                OnPropertyChanged(nameof(OSArchitecture));
            }
        }
        public string RegKey
        {
            get
            {
                return _regkey;
            }
            set
            {
                _regkey = value;
                OnPropertyChanged(nameof(RegKey));
            }
        }
        public double TotalDiskSpace
        {
            get
            {
                return _totaldiskspace;
            }
            set
            {
                _totaldiskspace = value;
                OnPropertyChanged(nameof(TotalDiskSpace));
            }
        }
        public double TotalCPU
        {
            get
            {
                return _totalcpu;
            }
            set
            {
                _totalcpu = value;
                OnPropertyChanged(nameof(TotalCPU));
            }
        }
        public double TotalRam
        {
            get
            {
                return _totalram;
            }
            set
            {
                _totalram = value;
                OnPropertyChanged(nameof(TotalRam));
            }
        }
        public long ExhibitID
        {
            get
            {
                return _exhibitid;
            }
            set
            {
                _exhibitid = value;
                OnPropertyChanged(nameof(ExhibitID));
            }
        }
        public ExhibitModel Exhibit
        {
            get
            {
                return DataHub.Exhibits.FirstOrDefault(a => a.ID == ExhibitID);
            }
        }
        public long ZoneID
        {
            get
            {
                return _zoneid;
            }
            set
            {
                _zoneid = value;
                OnPropertyChanged(nameof(ZoneID));
            }
        }
        public ZoneModel Zone
        {
            get
            {
                return DataHub.Zones.FirstOrDefault(a => a.ID == ZoneID);
            }
        }
        public long FloorID
        {
            get
            {
                return _floorid;
            }
            set
            {
                _floorid = value;
                OnPropertyChanged(nameof(FloorID));
            }
        }
        public FloorModel Floor
        {
            get
            {
                return DataHub.Floors.FirstOrDefault(a => a.ID == FloorID);
            }
        }
        public string ContentMetadata
        {
            get
            {
                return _contentmetadata;
            }
            set
            {
                _contentmetadata = value;
                OnPropertyChanged(nameof(ContentMetadata));
            }
        }
        public string PEMFile
        {
            get
            {
                return _pemfile;
            }
            set
            {
                _pemfile = value;
                OnPropertyChanged(nameof(PEMFile));
            }
        }
        public int HeartbeatRate
        {
            get
            {
                return _heartrate;
            }
            set
            {
                _heartrate = value;
                OnPropertyChanged(nameof(HeartbeatRate));
            }
        }
        public string Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }
        public string Category
        {
            get
            {
                return _category;
            }
            set
            {
                _category = value;
                OnPropertyChanged(nameof(Category));
            }
        }
        public int SequenceID
        {
            get
            {
                return _seqid;
            }
            set
            {
                _seqid = value;
                OnPropertyChanged(nameof(SequenceID));
            }
        }
        public bool IsOnline
        {
            get
            {
                return _isonline;
            }
            set
            {
                _isonline = value;
                OnPropertyChanged(nameof(IsOnline));
            }
        }
        public bool IsAudioGuide
        {
            get
            {
                return _isaudioguide;
            }
            set
            {
                _isaudioguide = value;
                OnPropertyChanged(nameof(IsAudioGuide));
            }
        }
        public bool IsControlPanel
        {
            get
            {
                return _iscontrolpanel;
            }
            set
            {
                _iscontrolpanel = value;
                OnPropertyChanged(nameof(IsControlPanel));
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
        public NodeCurrentStatusModel CurrentStatus
        {
            get
            {
                return _currentstatus;
            }
            set
            {
                _currentstatus = value;
                OnPropertyChanged(nameof(CurrentStatus));
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
