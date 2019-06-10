using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.WFSTATEArea.Models
{
    public class FunctionStateBO
    {
        public WF_STATE State { get; set; }
        public WF_STATE_FUNCTION StateFunction { get; set; }
        public List<SelectListItem> DsFunction { get; set; }
    }
}