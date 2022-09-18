using System;
using System.ComponentModel;
using System.Linq;
using MMS.Backend;

namespace MMS.DataModels
{
    public class CommandLogModel : INotifyPropertyChanged
    {
        #region Private Variables
        private long _id = -1;
        private long _commandid = -1;
        private long _nodeid = -1;
        private string _status = "";
        private string _message = "";
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
        public long CommandID
        {
            get
            {
                return _commandid;
            }
            set
            {
                _commandid = value;
                OnPropertyChanged(nameof(CommandID));
            }
        }
        public CommandModel Command
        {
            get
            {
                return DataHub.Commands.FirstOrDefault(a => a.ID == CommandID);
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
        public NodeModel Node
        {
            get
            {
                return DataHub.Nodes.FirstOrDefault(a => a.ID == NodeID);
            }
        }
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
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
