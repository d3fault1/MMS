using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMS.DataModels;

namespace MMS.Backend.DatabaseIO
{
    interface IDatabaseIO
    {
        DatabaseCheckResult CheckDatabaseValidity();
        bool CreateDatabase();
        bool CreateTables();
        List<FloorModel> ReadFloorData();
        List<ZoneModel> ReadZoneData();
        List<ExhibitModel> ReadExhibitData();
        List<NodeModel> ReadNodeData();
        List<NodeCurrentStatusModel> ReadNodeStatusData();
        List<CommandModel> ReadCommandData();
        List<CommandLogModel> ReadCommandLogData();
        bool WriteNodeLogData(ref List<NodeLogModel> data, bool updateExisting = false);
        bool WriteNodeStatusData(ref List<NodeModel> data, bool updateExisting = false);
        bool WriteNodeData(ref List<NodeModel> data, bool updateExisting = false);
        bool WriteFloorData(ref List<FloorModel> data, bool updateExisting = false);
        bool WriteZoneData(ref List<ZoneModel> data, bool updateExisting = false);
        bool WriteExhibitData(ref List<ExhibitModel> data, bool updateExisting = false);
        bool WriteCommandData(ref List<CommandModel> data, bool updateExisting = false);
        bool WriteCommandLogData(ref List<CommandLogModel> data, bool updateExisting = false);
    }

    enum DatabaseCheckResult
    {
        OK,
        NotFound,
        TableCorrupt,
        Exception
    }
}
