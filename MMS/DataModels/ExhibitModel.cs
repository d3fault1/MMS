using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataModels
{
    class ExhibitModel
    {
        public long ID { get; set; } = -1;
        public long ZoneID { get; set; } = -1;
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsActive { get; set; } = false;
        public string Image { get; set; } = "";
        public bool IsExhibitShow { get; set; } = false;
    }
}
