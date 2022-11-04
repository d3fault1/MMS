using MMS.DataModels;
using System.Collections.Generic;

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
        List<NodeFileModel> ReadNodeFileData();
        List<CommandModel> ReadCommandData();
        List<CommandLogModel> ReadCommandLogData();
        bool WriteNodeLogData(ref List<NodeLogModel> data, bool updateExisting = false);
        bool WriteNodeStatusData(ref List<NodeModel> data, bool updateExisting = false);
        bool WriteNodeFileData(ref List<NodeFileModel> data, bool updateExisting = false);
        bool WriteNodeData(ref List<NodeModel> data, bool updateExisting = false);
        bool WriteFloorData(ref List<FloorModel> data, bool updateExisting = false);
        bool WriteZoneData(ref List<ZoneModel> data, bool updateExisting = false);
        bool WriteExhibitData(ref List<ExhibitModel> data, bool updateExisting = false);
        bool WriteCommandData(ref List<CommandModel> data, bool updateExisting = false);
        bool WriteCommandLogData(ref List<CommandLogModel> data, bool updateExisting = false);
        bool DeleteNodeLogData(ref List<NodeLogModel> data);
        bool DeleteNodeStatusData(ref List<NodeModel> data);
        bool DeleteNodeFileData(ref List<NodeFileModel> data);
        bool DeleteNodeData(ref List<NodeModel> data);
        bool DeleteFloorData(ref List<FloorModel> data);
        bool DeleteZoneData(ref List<ZoneModel> data);
        bool DeleteExhibitData(ref List<ExhibitModel> data);
        bool DeleteCommandData(ref List<CommandModel> data);
        bool DeleteCommandLogData(ref List<CommandLogModel> data);
    }

    enum DatabaseCheckResult
    {
        OK,
        NotFound,
        TableCorrupt,
        Exception
    }
}
