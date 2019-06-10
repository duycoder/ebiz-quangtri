using Business.BaseBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.QLCHUYEN
{
    public class ChuyenSearchBO : SearchBaseBO
    {
        public int? CCTC_THANHPHAN_ID { set; get; }
        public string TEN_CHUYEN { set; get; }
        public DateTime? queryTimeStart { set; get; }
        public DateTime? queryTimeEnd { set; get; }
        public int? XE_ID { set; get; }
        public int? LAIXE_ID { set; get; }
        public List<long> CANBO_IDs { set; get; }
    }
}
