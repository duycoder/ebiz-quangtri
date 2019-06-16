using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Models
{
    public class RecipientDocViewModel
    {
        /// <summary>
        /// danh sách nhóm người nhận văn bản
        /// </summary>
        public IEnumerable<SelectListItem> GroupRecipients { get; set; }

        /// <summary>
        /// văn bản đi
        /// </summary>
        public HSCV_VANBANDI EntityVanBanDi { get; set; }

        /// <summary>
        /// lưu thông tin trên để gửi cá nhân khác
        /// </summary>
        public bool IsSendOthers { get; set; }
    }
}