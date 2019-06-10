using Business.CommonBusiness;
using Business.CommonModel.QLLAIXE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.QL_LAIXEArea.Models
{
    public class LaiXeBenhVienIndexViewModel
    {
        public PageListResultBO<LaiXeBO> listLaiXeBenhViens { set; get; }
        public LaiXeBenhVienIndexViewModel()
        {
            
        }
    }
}