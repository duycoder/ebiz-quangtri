using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model.Entities;
using Business.CommonModel.WFSTREAM;
namespace Web.Areas.WFSTREAMArea.Models
{
    public class EditVM
    {
        public WF_STREAM objModel { get; set; }
        public WF_STREAM_BO objBOModel { get; set; }
        public List<SelectListItem> LstLevel { get; set; }
    }
}
