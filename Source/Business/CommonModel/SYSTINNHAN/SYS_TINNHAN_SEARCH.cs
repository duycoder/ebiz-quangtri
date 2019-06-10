using Business.BaseBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.SYSTINNHAN
{
    public class SYS_TINNHAN_SEARCH : SearchBaseBO
    {
        public long USER_ID { get; set; }
        public string TIEUDE { get; set; }
        public DateTime? TUNGAY { get; set; }
        public DateTime? DENNGAY { get; set; }
        public bool? TRANGTHAI { get; set; }
    }
}
