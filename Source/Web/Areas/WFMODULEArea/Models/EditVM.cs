using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Entities;
using Business.CommonModel.WFMODULE;
using System.Web.Mvc;
namespace Web.Areas.WFMODULEArea.Models
{
    public class EditVM
    {
        public WF_MODULE objModel { get; set; }
        public WF_MODULE_BO objBOModel { get; set; }
        public List<SelectListItem> DsModule { get; set; }
        public List<SelectListItem> DsLuongXuLy { get; set; }
    }
}
