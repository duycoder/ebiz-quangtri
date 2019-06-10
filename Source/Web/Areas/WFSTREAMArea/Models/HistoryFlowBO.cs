using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.CommonBusiness;
using Model.Entities;
using Business.CommonModel.WFLOG;

namespace Web.Areas.WFSTREAMArea.Models
{
    public class HistoryFlowBO
    {
        public WF_PROCESS Process { get; set; }
        public List<WF_LOG_BO> lstLog { get; set; }
    }
}