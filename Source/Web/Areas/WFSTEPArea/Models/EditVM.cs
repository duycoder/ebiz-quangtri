using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Entities;
using Business.CommonModel.WFSTEP;
using System.Web.Mvc;
namespace Web.Areas.WFSTEPArea.Models
{
    public class EditVM
    {
        public WF_STEP objModel { get; set; }
        public WF_STEP_BO objBOModel { get; set; }
        public List<SelectListItem> dsTrangThaiStart { get; set; }
        public List<SelectListItem> dsTrangThaiEnd { get; set; }
    }
}
