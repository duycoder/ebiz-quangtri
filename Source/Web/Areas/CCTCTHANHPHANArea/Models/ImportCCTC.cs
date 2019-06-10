using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Entities;
using System.Web.Mvc;

namespace Web.Areas.CCTCTHANHPHANArea.Models
{
    public class ImportCCTC
    {
        public string PathTemplate { get; set; }
        public CCTC_THANHPHAN objectModel { get; set; }
        public int order { get; set; }
        public List<SelectListItem> DS_TYPE { get; set; }
        public List<SelectListItem> DS_CATEGORY { get; set; }
        public List<SelectListItem> ListCode { get; set; }
        public List<SelectListItem> ListTenPhong { get; set; }
    }
}