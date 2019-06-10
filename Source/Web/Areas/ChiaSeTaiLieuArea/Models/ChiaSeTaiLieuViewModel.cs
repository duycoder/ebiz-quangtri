using Business.CommonBusiness;
using Business.CommonModel.CHIASETAILIEU;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.ChiaSeTaiLieuArea.Models
{
    public class ChiaSeTaiLieuViewModel
    {
        public PageListResultBO<ChiaSeTaiLieuBO> PageList { get; set; }
        public List<SelectListItem> ListUserRequest { get; set; }
        public List<SelectListItem> ListUserShare { get; set; }
        public List<SelectListItem> ListStatus { get; set; }
    }
}