using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.BaseBusiness;

namespace Business.CommonModel.DMNguoiDung
{
    public class DM_NGUOIDUNG_SEARCHBO : SearchBaseBO
    {
        public string sea_HoTen { get; set; }
        public string sea_TenDangNhap { get; set; }
        public string sea_Email { get; set; }
        public string sea_DienThoai { get; set; }
        public int? deptId { get; set; }
        public int? sea_PHONGBAN_ID { get; set; }
        public int? sea_CHUCVU_ID { get; set; }
    }
}
