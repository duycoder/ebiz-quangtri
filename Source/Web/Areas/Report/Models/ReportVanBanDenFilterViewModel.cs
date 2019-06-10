using Business.CommonModel.CONSTANT;
using CommonHelper.DateExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonHelper;
namespace Web.Areas.Report.Models
{
    public class ReportVanBanDenFilterViewModel
    {
        public int timeFilterType { set; get; }
        public int reportType { set; get; }
        public string itemCategoryName { set; get; }
        public List<SelectListItem> groupOfDeparmtents { set; get; }
        public List<SelectListItem> groupOfItemCategoryFilter { set; get; }
        public List<SelectListItem> groupOfTimeTypesFilter { set; get; }
        public List<SelectListItem> groupOfYears { set; get; }
        public List<SelectListItem> groupOfMonths { set; get; }
        public bool filterTimeByDay { set; get; }
        public bool filterTimeByMonth { set; get; }
        public bool filterTimeByYear { set; get; }

        public ReportVanBanDenFilterViewModel()
        {
            this.timeFilterType = LOAI_BAOCAO_THOIGIAN_CONSTANT.NGAY;
            this.filterTimeByDay = this.timeFilterType == LOAI_BAOCAO_THOIGIAN_CONSTANT.NGAY;
            this.filterTimeByMonth = this.timeFilterType == LOAI_BAOCAO_THOIGIAN_CONSTANT.THANG;
            this.filterTimeByYear = this.timeFilterType == LOAI_BAOCAO_THOIGIAN_CONSTANT.NAM;

            this.groupOfMonths = Utility.GetMonths(DateTime.Now.Month);
            this.groupOfYears = Utility.GetYears(DateTime.Now.Year);

            groupOfTimeTypesFilter = new List<SelectListItem>();
            groupOfTimeTypesFilter.Add(new SelectListItem()
            {
                Value = LOAI_BAOCAO_THOIGIAN_CONSTANT.NGAY.ToString(),
                Text = "Theo ngày",
                Selected = true
            });
            groupOfTimeTypesFilter.Add(new SelectListItem()
            {
                Value = LOAI_BAOCAO_THOIGIAN_CONSTANT.THANG.ToString(),
                Text = "Theo tháng"
            });
            groupOfTimeTypesFilter.Add(new SelectListItem()
            {
                Value = LOAI_BAOCAO_THOIGIAN_CONSTANT.NAM.ToString(),
                Text = "Theo năm"
            });
        }

        public ReportVanBanDenFilterViewModel(int reportType)
        {
            this.reportType = reportType;
            switch (reportType)
            {
                case LOAI_BAOCAO_VANBAN_CONSTANT.BAOCAO_LOAI_VANBANDEN:
                    itemCategoryName = "Loại văn bản đến";
                    break;
                case LOAI_BAOCAO_VANBAN_CONSTANT.BAOCAO_LINHVUC_VANBANDEN:
                    itemCategoryName = "Lĩnh vực văn bản đến";
                    break;
                default:
                    itemCategoryName = "Đơn vị gửi văn bản đến";
                    break;
            }
            this.timeFilterType = LOAI_BAOCAO_THOIGIAN_CONSTANT.NGAY;
            this.filterTimeByDay = this.timeFilterType == LOAI_BAOCAO_THOIGIAN_CONSTANT.NGAY;
            this.filterTimeByMonth = this.timeFilterType == LOAI_BAOCAO_THOIGIAN_CONSTANT.THANG;
            this.filterTimeByYear = this.timeFilterType == LOAI_BAOCAO_THOIGIAN_CONSTANT.NAM;

            this.groupOfMonths = Utility.GetMonths(DateTime.Now.Month);
            this.groupOfYears = Utility.GetYears(DateTime.Now.Year);

            groupOfTimeTypesFilter = new List<SelectListItem>();
            groupOfTimeTypesFilter.Add(new SelectListItem()
            {
                Value = LOAI_BAOCAO_THOIGIAN_CONSTANT.NGAY.ToString(),
                Text = "Theo ngày",
                Selected = true
            });
            groupOfTimeTypesFilter.Add(new SelectListItem()
            {
                Value = LOAI_BAOCAO_THOIGIAN_CONSTANT.THANG.ToString(),
                Text = "Theo tháng"
            });
            groupOfTimeTypesFilter.Add(new SelectListItem()
            {
                Value = LOAI_BAOCAO_THOIGIAN_CONSTANT.NAM.ToString(),
                Text = "Theo năm"
            });
        }
    }
}