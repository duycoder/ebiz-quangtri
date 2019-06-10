using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model.Entities;
using Business.CommonBusiness;

namespace Web.Areas.CoCauToChucArea.Models
{
    public class CoCauToChucIndexModel
    {
        public List<SelectListItem> DS_TYPE { get; set; }
        public List<SelectListItem> DS_CATEGORY { get; set; }
        public List<CCTC_THANHPHAN> ListCoCau { get; set; }
        public CCTCItemTreeBO TreeData { get; set; }
    }
}