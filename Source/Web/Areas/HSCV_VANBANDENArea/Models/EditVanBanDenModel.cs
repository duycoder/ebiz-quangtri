using Business.CommonBusiness;
using Business.CommonModel.QLNGUOINHANVANBAN;
using CommonHelper.DateExtend;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.HSCV_VANBANDENArea.Models
{
    public class EditVanBanDenModel
    {
        public List<SelectListItem> groupDoKhans { set; get; }
        public List<SelectListItem> groupDoUuTiens { set; get; }
        public List<SelectListItem> groupLoaiVanBans { set; get; }
        public List<SelectListItem> groupLinhVucVanBans { set; get; }
        public List<SelectListItem> groupSoVanBanDens { set; get; }
        public List<SelectListItem> groupDonViGuis { set; get; }
        public List<TAILIEUDINHKEM> groupTaiLieuDinhKems { set; get; }
        public HSCV_VANBANDEN entityVanBanDen { set; get; }
        public List<CCTC_THANHPHAN> groupThanhPhans { set; get; }
        public List<CCTC_THANHPHAN> groupDonVis { set; get; }
        /// <summary>
        /// loại cơ quan
        /// </summary>
        public List<SelectListItem> GroupDeptTypes { set; get; }
        /// <summary>
        /// loại văn bản
        /// </summary>
        public List<SelectListItem> GroupDocTypes { set; get; }
        /// <summary>
        /// loại công văn đến
        /// </summary>
        public List<SelectListItem> GroupDispatchTypes { set; get; }

        /// <summary>
        /// @người nhận văn bản trực tiếp
        /// </summary>
        public List<long> UsersReceived { set; get; }
        public List<QL_NGUOINHAN_VANBAN_BO> Recipients { set; get; }



        public CCTCItemTreeBO treeDonVi { set; get; }
        public int fileMaxSize { set; get; }
        public string fileExtension { set; get; }
        public int typeOfVanBanDen { set; get; }
        public List<SelectListItem> groupHours { set; get; }
        public List<SelectListItem> groupMinutes { set; get; }
        
        public EditVanBanDenModel()
        {
            this.groupHours = Utility.GetHours();
            this.groupMinutes = Utility.GetMinutes();
        }

        public EditVanBanDenModel(HSCV_VANBANDEN entity)
        {
            this.groupHours = Utility.GetHours(entity.GIO_CONGTAC.GetValueOrDefault());
            this.groupMinutes = Utility.GetMinutes(entity.PHUT_CONGTAC.GetValueOrDefault());
        }
    }
}