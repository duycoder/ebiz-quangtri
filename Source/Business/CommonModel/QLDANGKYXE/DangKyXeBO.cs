using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.QLDANGKYXE
{
    public class DangKyXeBO : QL_DANGKY_XE
    {
        public string TEN_CANBO { set; get; }
        public string TEN_LOAIXE { set; get; }
        public string TENLOAI_CHUYEN { set; get; }
        public string THOIGIAN_XUATPHAT { set; get; }
        public string NGUOIDANGKY { set; get; }
        public string PHONGBAN_DANGKY { set; get; }
        public string TEN_TRANGTHAI { set; get; }
        public string MAU_TRANGTHAI { set; get; }
    }
}
