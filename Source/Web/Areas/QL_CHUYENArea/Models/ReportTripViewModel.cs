using Business.CommonModel.CONSTANT;
using Business.CommonModel.QLCHUYEN;
using CommonHelper.DateExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.QL_CHUYENArea.Models
{
    public class ReportTripViewModel
    {
        public string title { set; get; }
        public string timeLine { set; get; }
        public List<SelectListItem> groupOfUsers { set; get; }
        public List<SelectListItem> groupOfTypes { set; get; }
        public List<SelectListItem> groupOfYears { set; get; }
        public List<SelectListItem> groupOfMonths { set; get; }

        public List<ChuyenReportBO> reportEntity { set; get; }
        public ReportTripViewModel()
        {
            groupOfMonths = Utility.GetMonths(DateTime.Now.Month);
            groupOfYears = Utility.GetYears(DateTime.Now.Year);

            groupOfTypes = new List<SelectListItem>();
            groupOfTypes.Add(new SelectListItem()
            {
                Value = LOAI_BAOCAO_THOIGIAN_CONSTANT.NGAY.ToString(),
                Text = "Theo ngày",
                Selected = true,
            });
            groupOfTypes.Add(new SelectListItem()
            {
                Value = LOAI_BAOCAO_THOIGIAN_CONSTANT.THANG.ToString(),
                Text = "Theo tháng"
            });
            groupOfTypes.Add(new SelectListItem()
            {
                Value = LOAI_BAOCAO_THOIGIAN_CONSTANT.NAM.ToString(),
                Text = "Theo năm"
            });
        }
    }
}