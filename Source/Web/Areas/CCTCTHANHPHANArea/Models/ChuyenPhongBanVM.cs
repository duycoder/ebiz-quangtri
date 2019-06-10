using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Entities;
using System.Web.Mvc;


namespace Web.Areas.CCTCTHANHPHANArea.Models
{
    public class ChuyenPhongBanVM
    {
        public List<SelectListItem> listPhongBan { get; set; }
        public DM_NGUOIDUNG nguoiDung { get; set; }
    }
}