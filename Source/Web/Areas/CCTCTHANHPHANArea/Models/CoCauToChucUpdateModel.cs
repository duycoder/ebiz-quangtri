using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.CommonBusiness;
using Model.Entities;
using System.Web.Mvc;

namespace Web.Areas.CoCauToChucArea.Models
{
    public class CoCauToChucUpdateModel
    {
        public CCTC_THANHPHAN Item { get; set; }
        public List<CCTC_THANHPHAN> DS_PARENT { get; set; }
        public List<SelectListItem> DS_TYPE { get; set; }
        public List<SelectListItem> DS_CATEGORY { get; set; }
    }
}