using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Entities;
using Business.CommonBusiness;
using Business.CommonModel.WFSTEP;

namespace Web.Areas.WFSTEPArea.Models
{
    public class IndexVM
    {
        public WF_STREAM LuongXuLy { get; set; }
        public PageListResultBO<WF_STEP_BO> LstStep { get; set; }
        public GoModel GoData { get; set; }
    }
}