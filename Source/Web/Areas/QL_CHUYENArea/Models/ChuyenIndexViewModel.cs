using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.QLCHUYEN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.QL_CHUYENArea.Models
{
    public class ChuyenIndexViewModel
    {
        public long currentUserId { set; get; }
        public PageListResultBO<ChuyenBO> listChuyens { set; get; }
        public List<SelectListItem> groupCarTypes { set; get; }
        public List<SelectListItem> groupCars { set; get; }
        public List<SelectListItem> groupDrivers { set; get; }

        public ChuyenIndexViewModel()
        {
            this.groupCarTypes = new List<SelectListItem>();
            this.groupCarTypes.Add(new SelectListItem()
            {
                Value = LOAIXE_CONSTANT.XECHO_BENHNHAN.ToString(),
                Text = TENLOAIXE_CONSTANT.XECHO_BENHNHAN
            });
            this.groupCarTypes.Add(new SelectListItem()
            {
                Value = LOAIXE_CONSTANT.XECHO_CANBO.ToString(),
                Text = TENLOAIXE_CONSTANT.XECHO_CANBO
            });
        }
    }
}