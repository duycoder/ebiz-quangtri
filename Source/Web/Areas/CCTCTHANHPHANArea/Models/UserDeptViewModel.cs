using Business.CommonBusiness;
using Business.CommonModel.DMNguoiDung;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.CCTCTHANHPHANArea.Models
{
    public class UserDeptViewModel
    {
        /// <summary>
        /// phòng ban hiện tại
        /// </summary>
        public CCTC_THANHPHAN EntityDept { get; set; }

        /// <summary>
        /// danh sách vai trò
        /// </summary>
        public List<SelectListItem> GroupRoles { get; set; }

        /// <summary>
        /// danh sách chức vụ
        /// </summary>
        public List<SelectListItem> GroupPositions { get; set; }

        /// <summary>
        /// danh sách người dùng
        /// </summary>
        public PageListResultBO<DM_NGUOIDUNG_BO> GroupUsers { get; set; }
    }
}