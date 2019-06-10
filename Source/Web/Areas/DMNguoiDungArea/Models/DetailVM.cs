using Business.CommonBusiness;
using Business.CommonModel.DMCHUCNANG;
using Business.CommonModel.DMNguoiDung;
using Model.Entities;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Web.Areas.DMNguoiDungArea.Models
{
    public class DetailVM
    {
        public DM_NGUOIDUNG_BO NguoiDung { get; set; }
        public List<DM_VAITRO> ListVaiTro { get; set; }
        public List<DM_CHUCNANG_BO> ListChucNang { get; set; }
    }
    public class UserModel
    {
        public PageListResultBO<DM_NGUOIDUNG_BO> ListResult { get; set; }
        public CCTCItemTreeBO TreeData { get; set; }
        public List<SelectListItem> ListPhongBan { get; set; }
        public List<SelectListItem> ListVaiTro { get; set; }
        public List<SelectListItem> ListChucVu { get; set; }
    }
}