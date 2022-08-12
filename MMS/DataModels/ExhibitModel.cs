﻿using System;
using System.ComponentModel;
using System.Linq;
using MMS.Backend;

namespace MMS.DataModels
{
    class ExhibitModel : INotifyPropertyChanged
    {
        #region Private Variables
        private long _id = -1;
        private long _zoneid = -1;
        private string _name = "";
        private string _description = "";
        private bool _isactive = false;
        private string _image = "";
        private bool _isexhibitshow = false;
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
        public bool IsExhibitShow
        {
            get
            {
                return _isexhibitshow;
            }
            set
            {
                _isexhibitshow = value;
                OnPropertyChanged(nameof(IsExhibitShow));
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
