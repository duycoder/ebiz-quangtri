using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Entities;
using Business.CommonModel.DMCHUCNANG;

namespace Web.Areas.DMNguoiDungArea.Models
{
    public class PhanQuyenVM
    {
        public DM_NGUOIDUNG NguoiDung { get; set; }
        public List<DM_NGUOIDUNG_THAOTAC> NguoiDungThaoTac { get; set; }
        public List<DM_CHUCNANG_BO> ListAllChucNang { get; set; }
        public List<DM_CHUCNANG_BO> ListChucNangVaiTro { get; set; }

    }
}