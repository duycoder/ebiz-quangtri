using Business.BaseBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.QLLAIXE
{
    public class LaiXeSearchBO : SearchBaseBO
    {
        public int? CCTC_THANHPHAN_ID { set; get; }
        public string HOTEN { set; get; }
        public bool? GIOITINH { set; get; }
        public string CMND { set; get; }
        public string EMAIL { set; get; }
        public string SODIENTHOAI { set; get; }
    }
}
