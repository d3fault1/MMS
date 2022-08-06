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
        public static ObservableCollection<NodeModel> Nodes { get; } = new ObservableCollection<NodeModel>();
        public static ObservableCollection<ExhibitModel> Exhibits { get; } = new ObservableCollection<ExhibitModel>();
        public static ObservableCollection<ZoneModel> Zones { get; } = new ObservableCollection<ZoneModel>();
        public static ObservableCollection<FloorModel> Floors { get; } = new ObservableCollection<FloorModel>();
        public static ObservableCollection<CommandModel> Commands { get; } = new ObservableCollection<CommandModel>();
        public static ObservableCollection<CommandLogModel> CommandLogs { get; } = new ObservableCollection<CommandLogModel>();
    }
}
