using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Entities;
using Business.CommonModel.WFSTATE;
using System.Web.Mvc;
namespace Web.Areas.WFSTATEArea.Models
{
    public class CreateVM
    {
        public WF_STATE objModel { get; set; }
        public WF_STREAM LuongXuLy { get; set; }
        public List<SelectListItem> DsChucVu { get; set; }
        public List<SelectListItem> DsVaiTro { get; set; }
    }
}
