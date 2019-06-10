using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.CommonModel.LOGSMS;
using Business.CommonBusiness;

namespace Web.Areas.LOGSMSArea.Models
{
    public class LogsmsVM
    {
        public PageListResultBO<LOGSMS_BO> OjectModel { get; set; }
    }
}