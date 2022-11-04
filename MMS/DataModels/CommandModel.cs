using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MMS.DataModels
{
    public class CommandModel : INotifyPropertyChanged
    {
        #region Private Variables
        private long _id = -1;
        private string _commandname = "";
        private string _command = "";
        private int _commandnumber = -1;
        private string _sessionid = "";
        private List<string> _parameters = new List<string>();
        private bool _isenabled = false;
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
        public string CommandName
        {
            get
            {
                return _commandname;
            }
            set
            {
                _commandname = value;
                OnPropertyChanged(nameof(CommandName));
            }
        }
        public string Command
        {
            get
            {
                return _command;
            }
            set
            {
                _command = value;
                OnPropertyChanged(nameof(Command));
            }
        }
        public int CommandNumber
        {
            get
            {
                return _commandnumber;
            }
            set
            {
                _commandnumber = value;
                OnPropertyChanged(nameof(CommandNumber));
            }
        }
        public string SessionID
        {
            get
            {
                return _sessionid;
            }
            set
            {
                _sessionid = value;
                OnPropertyChanged(nameof(SessionID));
            }
        }
        public List<string> Parameters
        {
            get
            {
                return _parameters;
            }
        }
        public bool IsEnabled
        {
            get
            {
                return _isenabled;
            }
            set
            {
                _isenabled = value;
                OnPropertyChanged(nameof(IsEnabled));
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

        #region Functions
        public void Prepare()
        {
            Parameters.Clear();
        }
        public string GetFinalizedCommand()
        {
            SessionID = Guid.NewGuid().ToString("N");
            string finalizedCommand = Command + " " + SessionID;
            foreach (var parameter in Parameters) finalizedCommand += " " + parameter;
            return finalizedCommand;
        }
        #endregion

        #region Notify Event and Functions
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
