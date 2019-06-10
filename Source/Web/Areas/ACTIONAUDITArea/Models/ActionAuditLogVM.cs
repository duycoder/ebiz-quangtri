using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business.CommonBusiness;
using Model.Entities;

namespace Web.Areas.ACTIONAUDITArea.Models
{
    public class ActionAuditLogVM
    {
        public PageListResultBO<ACTION_AUDIT> ListResult { get; set; }
        public List<SelectListItem> ListUser { get; set; }
    }
}