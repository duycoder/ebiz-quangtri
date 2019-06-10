using Business.CommonModel.CONSTANT;
using CommonHelper.DateExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.Report.Models
{
    public class ReportVanBanDiFilterViewModel : ReportVanBanDenFilterViewModel
    {
        public ReportVanBanDiFilterViewModel() : base()
        {

        }

        public ReportVanBanDiFilterViewModel(int reportType) : base(reportType)
        {
            switch (reportType)
            {
                case LOAI_BAOCAO_VANBAN_CONSTANT.BAOCAO_HINHTHUC_VANBANDI:
                    this.itemCategoryName = "Hình thức văn bản";
                    break;
                case LOAI_BAOCAO_VANBAN_CONSTANT.BAOCAO_LINHVUC_VANBANDI:
                    this.itemCategoryName = "Lĩnh vực văn bản";
                    break;
                default:
                    this.itemCategoryName = "Đơn vị nhận văn bản";
                    break;
            }
        }


    }
}