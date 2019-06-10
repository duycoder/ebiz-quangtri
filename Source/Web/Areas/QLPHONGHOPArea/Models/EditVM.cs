using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Entities;
using System.Web.Mvc;
using Business.CommonModel.QLPHONGHOP;
using CommonHelper.DateExtend;
namespace Web.Areas.QLPHONGHOPArea.Models
{
    public class EditVM
    {
        public List<SelectListItem> LstUsers { get; set; }
        public QLPHONGHOP_BO GetEdit { get; set; }
        public List<DM_DANHMUC_DATA> LstPhong { get; set; }
        public DM_VAITRO objModel { get; set; }

        /// <summary>
        /// @author: duynn
        /// @description: cập nhật view model phòng họp
        /// </summary>

        public QUANLY_PHONGHOP roomEntity { set; get; }
        public List<SelectListItem> groupStartHours { set; get; } //danh sách giờ họp
        public List<SelectListItem> groupStartMinutes { set; get; } //danh sách phút họp

        public List<SelectListItem> groupEndHours { set; get; } //danh sách giờ họp
        public List<SelectListItem> groupEndMinutes { set; get; } //danh sách phút họp
        public List<SelectListItem> groupLeaders { set; get; } //danh sách lãnh đạo sẽ họp

        public EditVM()
        {
            this.roomEntity = new QUANLY_PHONGHOP();
            this.groupStartHours = Utility.GetHours(0, 8);
            this.groupStartMinutes = Utility.GetMinutes(0, 5);
            this.groupEndHours = this.groupStartHours;
            this.groupEndMinutes = this.groupEndMinutes;
        }

        public EditVM(QUANLY_PHONGHOP roomEntity)
        {
            this.roomEntity = roomEntity;
            this.groupStartHours = Utility.GetHours(roomEntity.GIOBATDAU.GetValueOrDefault(), 8);
            this.groupStartMinutes = Utility.GetMinutes(roomEntity.PHUTBATDAU.GetValueOrDefault(),5);

            this.groupEndHours = Utility.GetHours(roomEntity.GIOKETTHUC.GetValueOrDefault(), 8);
            this.groupEndMinutes = Utility.GetMinutes(roomEntity.PHUTKETTHUC.GetValueOrDefault(), 5);
        }
    }
}