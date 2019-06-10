using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.DMNguoiDung
{
    public class NguoiDungPhongBanBO
    {
        public CCTC_THANHPHAN PhongBan { get; set; }
        public List<DM_NGUOIDUNG_BO> LstNguoiDung { get; set; }
    }
}
