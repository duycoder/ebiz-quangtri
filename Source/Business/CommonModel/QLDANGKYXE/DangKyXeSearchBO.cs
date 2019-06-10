using Business.BaseBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.QLDANGKYXE
{
    public class DangKyXeSearchBO : SearchBaseBO
    {
        public long? USER_ID { set; get; }
        public int? TRANGTHAI { set; get; }
        public bool HAS_ROLE_CONFIRM { set; get; }
        public int? CCTC_THANHPHAN_ID { set; get; }
        public int? querySoNguoiStart { set; get; }
        public int? querySoNguoiEnd { set; get; }
        public string MUCDICH { set; get; }
        public DateTime? queryThoiGianXuatPhatStart { set; get; }
        public DateTime? queryThoiGianXuatPhatEnd { set; get; }
    }
}
