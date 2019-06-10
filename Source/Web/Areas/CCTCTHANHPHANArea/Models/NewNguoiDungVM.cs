using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Entities;
using Business.CommonBusiness;
using Business.CommonModel.DMNguoiDung;

namespace Web.Areas.CCTCTHANHPHANArea.Models
{
    public class NewNguoiDungVM
    {
        public CCTC_THANHPHAN PhongBan { get; set; }
        public PageListResultBO<DM_NGUOIDUNG_BO> lstNguoiDung { get; set; }
    }
}