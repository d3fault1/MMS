using System;
using System.ComponentModel;

namespace MMS.DataModels
{
    public class NodeFileModel : INotifyPropertyChanged
    {
        #region Private Variables
        private long _id = -1;
        private long _nodeid = -1;
        private string _nodefile = "";
        private int _position = -1;
        private DateTime _createdat = DateTime.MinValue;
        private DateTime _updatedat = DateTime.MinValue;
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
        public string NodeFile
        {
            get
            {
                return _nodefile;
            }
            set
            {
                _nodefile = value;
                OnPropertyChanged(nameof(NodeFile));
            }
        }
        public int Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                OnPropertyChanged(nameof(Position));
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
