using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Entities;
using Business.CommonBusiness;
using Business.CommonModel.DMNguoiDung;

namespace Web.Areas.CoCauToChucArea.Models
{
    public class CoCauToChucNguoiDungModel
    {
        public CCTC_THANHPHAN Item { get; set; }
        public List<DM_NGUOIDUNG_BO> ListNguoiDung { get; set; }
    }
}