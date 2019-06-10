using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.QLDANGKYXE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.QL_DANGKY_XEArea.Models
{
    public class DangKyXeIndexViewModel
    {
        public long currentUserId { set; get; }
        public bool canReceiveRegistration { set; get; }
        public List<SelectListItem> groupOfLoaiXes { set; get; }
        public PageListResultBO<DangKyXeBO> listDangKyXeBenhViens { set; get; }

        public DangKyXeIndexViewModel()
        {
            this.groupOfLoaiXes = new List<SelectListItem>();
            this.groupOfLoaiXes.Add(new SelectListItem()
            {
                Value = LOAIXE_CONSTANT.XECHO_BENHNHAN.ToString(),
                Text =  TENLOAIXE_CONSTANT.XECHO_BENHNHAN
            });
            this.groupOfLoaiXes.Add(new SelectListItem()
            {
                Value = LOAIXE_CONSTANT.XECHO_CANBO.ToString(),
                Text = TENLOAIXE_CONSTANT.XECHO_CANBO
            });
        }
    }
}