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
        bool WriteNodeLogData(List<NodeLogModel> data);
        bool WriteNodeStatusData(List<NodeModel> data);
        bool WriteNodeData(List<NodeModel> data);
        bool WriteFloorData(List<FloorModel> data);
        bool WriteZoneData(List<ZoneModel> data);
        bool WriteExhibitData(List<ExhibitModel> data);
    }

    enum DatabaseCheckResult
    {
        OK,
        NotFound,
        TableCorrupt,
        Exception
    }
}
