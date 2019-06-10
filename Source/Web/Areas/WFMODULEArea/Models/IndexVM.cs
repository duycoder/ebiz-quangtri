using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.CommonBusiness;
using Web.Areas.WFMODULEArea.Models;
using Business.CommonModel.WFMODULE;
using Model.Entities;

namespace Web.Areas.WFMODULEArea.Models
{
    public class IndexVM
    {
        public PageListResultBO<WF_MODULE_BO> LstModule { get; set; }
    }
}