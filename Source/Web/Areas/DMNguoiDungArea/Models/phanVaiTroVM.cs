using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Entities;
using System.Web.Mvc;

namespace Web.Areas.DMNguoiDungArea.Models
{
    public class phanVaiTroVM
    {
        public DM_NGUOIDUNG NguoiDung { get; set; }
        public List<SelectListItem> DsVaiTro { get; set; }
        public DM_DANHMUC_DATA Data { get; set; }
    }
}