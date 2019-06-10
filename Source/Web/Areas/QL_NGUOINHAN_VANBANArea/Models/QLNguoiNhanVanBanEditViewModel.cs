using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.QL_NGUOINHAN_VANBANArea.Models
{
    public class QLNguoiNhanVanBanEditViewModel
    {
        public QL_NGUOINHAN_VANBAN Entity { set; get; }
        public List<SelectListItem> GroupUsers { set; get; }

        public QLNguoiNhanVanBanEditViewModel()
        {
            this.Entity = new QL_NGUOINHAN_VANBAN();
            this.GroupUsers = new List<SelectListItem>();
        }
    }
}