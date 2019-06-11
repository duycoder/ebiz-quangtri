using Business.CommonBusiness;
using Business.CommonModel.QLNGUOINHANVANBAN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.QL_NGUOINHAN_VANBANArea.Models
{
    public class QLNguoiNhanVanBanIndexViewModel
    {
        public bool IsSystemAdmin { get; set; }
        public int DepartmentId { get; set; }
        public PageListResultBO<QL_NGUOINHAN_VANBAN_BO> GroupRecipients { get; set; }
    }
}