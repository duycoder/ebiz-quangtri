using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.CommonBusiness;
using System.Web.Mvc;
using Model.Entities;

namespace Web.Areas.QLPHONGHOPArea.Models
{
    public class LstUsersVM
    {
        public List<SelectListItem> lstusers { get; set; }

        public List<QUANLY_PHONGHOP> ListDatPhong { get; set; }
    }
}