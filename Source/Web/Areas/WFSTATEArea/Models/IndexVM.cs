using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Entities;
using Business.CommonBusiness;
using Business.CommonModel.WFSTATE;
using Business.CommonModel.WFSTREAM;

namespace Web.Areas.WFSTATEArea.Models
{
    public class IndexVM
    {
        public WF_STREAM_BO LuongXuLy { get; set; }
        public PageListResultBO<WF_STATE_BO> LstState { get; set; }
    }
}