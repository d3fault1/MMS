using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMS.DataModels;

namespace MMS.Backend
{
    static class DataHub
    {
        public static ObservableCollection<NodeModel> Nodes = new ObservableCollection<NodeModel>();
        public static ObservableCollection<ExhibitModel> Exhibits = new ObservableCollection<ExhibitModel>();
        public static ObservableCollection<ZoneModel> Zones = new ObservableCollection<ZoneModel>();
        public static ObservableCollection<FloorModel> Floors = new ObservableCollection<FloorModel>();
        public static ObservableCollection<CommandModel> Commands = new ObservableCollection<CommandModel>();
    }
}
