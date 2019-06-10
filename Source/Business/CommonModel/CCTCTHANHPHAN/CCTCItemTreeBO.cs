using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonBusiness
{
    public class CCTCItemTreeBO
    {
        public int ID { get; set; }
        public bool? IS_DELETE { get; set; }
        public int? ITEM_LEVEL { get; set; }
        public string NAME { get; set; }
        public DateTime? NGAYSUA { get; set; }
        public DateTime? NGAYTAO { get; set; }
        public long? NGUOISUA { get; set; }
        public long? NGUOITAO { get; set; }
        public int? PARENT_ID { get; set; }
        public int TYPE { get; set; }
        public int? THUTU { get; set; }
        public string CODE { get; set; }
        public int TYPE_NAME { get; set; }
        public bool IS_ALLOW_SELECT { set; get; }
        public List<CCTCItemTreeBO> Child { get; set; }
    }
}
