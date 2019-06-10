using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Entities;
using Business.CommonModel.WFSTEP;
using System.Web.Mvc;

namespace Web.Areas.WFSTEPArea.Models
{
    public class ConfigVM
    {
        public WF_STEP_BO Step { get; set; }
        public WF_STEP_CONFIG ConfigStep { get; set; }
        public List<SelectListItem> DsVaiTro { get; set; }
        public List<SelectListItem> DSChucVu { get; set; }
        public List<SelectListItem> DSCap { get; set; }
        public List<SelectListItem> DSPhongBan { get; set; }
        public WF_STEP_USER_PROCESS MainProcess { get; set; }
        public WF_STEP_USER_PROCESS JoinProcess { get; set; }

    }
}