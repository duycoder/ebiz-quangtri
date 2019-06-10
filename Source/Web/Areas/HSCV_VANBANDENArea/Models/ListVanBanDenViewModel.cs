using Business.CommonBusiness;
using Business.CommonModel.HSCVVANBANDEN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.HSCV_VANBANDENArea.Models
{
    public class ListVanBanDenViewModel
    {
        public PageListResultBO<HSCV_VANBANDEN_BO> groupOfVanBanDens { set; get; }
        public List<SelectListItem> groupOfLoaiVanBans { set; get; }
        public List<SelectListItem> groupOfLinhVucVanBans { set; get; }
        public List<SelectListItem> groupOfDoKhans { set; get; }
        public List<SelectListItem> groupOfDoUuTiens { set; get; }
        public List<SelectListItem> groupSoVanBanDens { set; get; }
        public UserInfoBO userInfo { set; get; }
        public int typeOfLoaiVanBan { set; get; }
        public string title { set; get; }
        public bool canCreate { set; get; }
    }
}