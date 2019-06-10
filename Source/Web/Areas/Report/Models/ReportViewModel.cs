using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.BaoCaoThongKe.Models
{
    public class ReportViewModel
    {
        public List<SelectListItem> LstCoCauToChuc { get; set; }
        public List<SelectListItem> LstDonVi { get; set; }
        public List<SelectListItem> LstNhanVien { get; set; }
        public List<SelectListItem> listAllDonViBanHanhs { get; set; }
        public List<SelectListItem> TrangThai { get; set; }
        public List<SelectListItem> ListLoaiVanBan { get; set; }
        public List<SelectListItem> ListDoKhanVanBan { get; set; }
        public List<SelectListItem> ListNguoiDung { get; set; }
        public bool HasRoleAssignChuyenVien { get; set; }
        public bool HasRoleAssignDepartment { get; set; }
        public bool HasRoleAssignUnit { get; set; }


    }
}