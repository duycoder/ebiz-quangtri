using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.QLXE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.QL_XEArea.Models
{
    public class XeBenhVienIndexViewModel
    {
        public List<SelectListItem> groupOfLoaiXes { set; get; }
        public PageListResultBO<XeBO> listXeBenhViens { set; get; }
        public XeBenhVienIndexViewModel()
        {
            groupOfLoaiXes = new List<SelectListItem>();
            groupOfLoaiXes.Add(new SelectListItem()
            {
                Value = LOAIXE_CONSTANT.XECHO_BENHNHAN.ToString(),
                Text = "Xe chở bệnh nhân"
            });
            groupOfLoaiXes.Add(new SelectListItem()
            {
                Value = LOAIXE_CONSTANT.XECHO_CANBO.ToString(),
                Text = "Xe chở cán bộ"
            });
        }
    }
}