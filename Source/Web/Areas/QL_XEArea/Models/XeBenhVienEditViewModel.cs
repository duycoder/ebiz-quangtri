using Business.CommonModel.CONSTANT;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.QL_XEArea.Models
{
    public class XeBenhVienEditViewModel
    {
        public List<SelectListItem> groupOfLoaiXes { set; get; }
        public QL_XE xeEntity { set; get; }
        public XeBenhVienEditViewModel()
        {
            xeEntity = new QL_XE();
        }

        public XeBenhVienEditViewModel(QL_XE entity)
        {
            xeEntity = entity;
            groupOfLoaiXes = new List<SelectListItem>();
        }
    }
}