using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.CCTCTHANHPHANArea.Models
{
    public class DeptUserEditViewModel
    {
        public CCTC_THANHPHAN EntityDepartment { get; set; }
        public List<SelectListItem> GroupPositions { get; set; }
        public List<SelectListItem> GroupRoles { get; set; }
    }
}