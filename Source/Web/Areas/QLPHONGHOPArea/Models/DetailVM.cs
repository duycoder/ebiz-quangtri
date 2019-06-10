using System;
using System.Collections.Generic;
using System.Linq;
using Model.Entities;
using System.Web;
using System.Web.Mvc;
using Business.CommonModel.QLPHONGHOP;

namespace Web.Areas.QLPHONGHOPArea.Models
{
    public class DetailVM
    {
        public QLPHONGHOP_BO roomEntity { set; get; }
        public List<SelectListItem> groupOfRooms { set; get; }
    }
}