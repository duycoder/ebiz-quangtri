using Business.CommonModel.CHIASETAILIEU;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.ChiaSeTaiLieuArea.Models
{
    public class ShareRequestFormModel
    {
        public CHIASE_TAILIEU Share { get  ; set; }
    }
    public class ShareApproveFormModel
    {
        public ChiaSeTaiLieuBO Share { get; set; }
        public List<SelectListItem> ListUser { get; set; }
    }
}