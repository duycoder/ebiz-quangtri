using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.Report.Models
{
    public class ReportVanBanDiResultViewModel: ReportVanBanDenResultViewModel
    {
        public List<SelectListItem> groupOfReportByHinhThucVanBanItems { set; get; }
        public List<SelectListItem> groupOfReportByDonViNhanVanBanItems { set; get; }
    }
}