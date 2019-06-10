using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.Report.Models
{
    public class ReportVanBanDenResultViewModel
    {
        public int reportType { set; get; }
        public string title { set; get; }
        public int total { set; get; }
        public List<SelectListItem> groupOfReportResultItems { set; get; }
        public List<SelectListItem> groupOfReportByLoaiVanBanItems { set; get; }
        public List<SelectListItem> groupOfReportByLinhVucVanBanItems { set; get; }
        public List<SelectListItem> groupOfReportByDonViGuiVanBanItems { set; get; }
    }
}