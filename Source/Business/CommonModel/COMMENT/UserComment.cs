using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonBusiness
{
    public class UserComment
    {
        public string UserAvatar { get; set; }
        public string FullName { get; set; }
        public long? UserId { get; set; }
        public long? CONGVIEC_ID { get; set; }
        public bool? HAS_FILE { get; set; }
        public long ID { get; set; }
        public DateTime? NGAYTAO { get; set; }
        public string NOIDUNG { get; set; }
        public long? REPLY_ID { get; set; }
        public string TIEUDE { get; set; }
        public int NUMBER_REPLY { set; get; }
        public TAILIEUDINHKEM ATTACH { set; get; }
    }
}
