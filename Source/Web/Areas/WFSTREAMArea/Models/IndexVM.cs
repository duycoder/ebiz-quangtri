using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.CommonBusiness;
using Business.CommonModel.WFSTREAM;
namespace Web.Areas.WFSTREAMArea.Models
{
    public class IndexVM
    {
        public PageListResultBO<WF_STREAM_BO> LstLuong { get; set; }
    }
}