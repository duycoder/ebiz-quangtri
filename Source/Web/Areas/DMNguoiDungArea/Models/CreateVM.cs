using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Entities;
using Business.CommonModel.DMNguoiDung;
using System.Web.Mvc;

namespace Web.Areas.DMNGUOIDUNGArea.Models
{
    public class CreateVM
    {
        public DM_NGUOIDUNG objModel { get; set; }
        public List<SelectListItem> DsChucVu { get; set; }
        public List<SelectListItem> LstDonViHienTai { get; set; }
    }
}
