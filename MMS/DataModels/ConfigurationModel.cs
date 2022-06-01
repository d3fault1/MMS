using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataModels
{
    class ConfigurationModel
    {
        public int HTTPServerPort { get; set; }
        public string DBInstance { get; set; }
        public string DBName { get; set; }
    }
}
