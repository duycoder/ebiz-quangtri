using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonBusiness
{
    public class ChatBO
    {
        public long ID { get; set; }
        public int? COSO_ID { get; set; }
        public long? GROUPCHAT_ID { get; set; }
        public long? NGUOIGUI_ID { get; set; }
        public long? NGUOINHAN_ID { get; set; }
        public string FROMFULLNAME { get; set; }
        public string FROMUSER { get; set; }
        public bool? IS_DELETE { get; set; }
        public DateTime? NGAYGUI { get; set; }
        public DateTime? NGAYSUA { get; set; }
        public string NOIDUNG { get; set; }
        public string TOFULLNAME { get; set; }
        public string TOUSER { get; set; }
        public string GROUPNAME { get; set; }
    }
}
