using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Entities;
using Business.CommonModel.WFSTEP;
using System.Web.Mvc;
namespace Web.Areas.WFSTEPArea.Models
{
    public class CreateVM
    {
        public WF_STEP objModel { get; set; }
        public WF_STREAM LuongXuLy { get; set; }
        public List<SelectListItem> DsTrangThai { get; set; }
    }
}
