using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonBusiness
{
    public class DMLoaiDonViBO
    {
        public int ID { get; set; }
        public string LOAI { get; set; }
        public int? PARENT_ID { get; set; }
        public string PARENT_NAME { get; set; }
        public DateTime? NGAYSUA { get; set; }
        public DateTime? NGAYTAO { get; set; }
        public int? NGUOISUA { get; set; }
        public int? NGUOITAO { get; set; }
    }
}
