using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.CommonModel.WFSTREAM;

namespace Web.Areas.WFSTREAMArea.Models
{
    public class KeThuaVM
    {
        public int ToaNhaID { get; set; }
        public List<WF_STREAM_BO> LstFlow { get; set; }
    }
}