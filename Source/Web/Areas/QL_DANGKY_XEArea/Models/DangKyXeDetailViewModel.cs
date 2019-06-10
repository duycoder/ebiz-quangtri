using Business.CommonModel.QLDANGKYXE;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.QL_DANGKY_XEArea.Models
{
    public class DangKyXeDetailViewModel
    {
        public long currentUserId { set; get; }
        public DangKyXeBO entity { set; get; }
        public LICHCONGTAC entityCalendar { set; get; }
        public bool canSendRegistration { set; get; } //có thể gửi yêu cầu
        public bool canRecieveRegistratiion { set; get; } //có thể tiếp nhận yêu cầu
    }
}