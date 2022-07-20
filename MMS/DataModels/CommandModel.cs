using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMS.Backend;

namespace MMS.DataModels
{
    class CommandModel : INotifyPropertyChanged
    {
        #region Private Variables
        private long _id = -1;
        private string _command = "";
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

        #region Notify Event and Functions
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
