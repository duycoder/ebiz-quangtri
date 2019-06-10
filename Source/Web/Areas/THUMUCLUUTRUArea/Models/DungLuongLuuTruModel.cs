using Business.CommonBusiness;
using Business.CommonModel.DMNguoiDung;
using Model.Entities;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Web.Areas.THUMUCLUUTRUArea.Models
{
    public class DungLuongLuuTruModel
    {
        public List<SelectListItem> DS_TYPE { get; set; }
        public List<CCTC_THANHPHAN> ListCoCau { get; set; }
        public CCTCItemTreeBO TreeData { get; set; }
        public CCTC_THANHPHAN Item { get; set; }
        public List<DM_NGUOIDUNG_BO> ListNguoiDung { get; set; }
        public DUNGLUONG_LUUTRU Storage { get; set; }
        public DM_NGUOIDUNG NguoiDung { get; set; }
        public CCTC_THANHPHAN DonVi { get; set; }
    }
}