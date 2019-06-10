using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonHelper.DateExtend;
using Business.CommonModel.CONSTANT;

namespace Web.Areas.QL_DANGKY_XEArea.Models
{
    public class DangKyXeEditViewModel
    {
        public List<SelectListItem> groupOfLanhDaos { set; get; }
        public List<SelectListItem> groupOfLoaiXes { set; get; }
        public List<SelectListItem> groupOfLoaiChuyens { set; get; }
        public List<SelectListItem> groupOfHours { set; get; }
        public List<SelectListItem> groupOfMinutes { set; get; }

        public List<SelectListItem> groupOfCars { set; get; }
        public List<SelectListItem> groupOfDrivers { set; get; }
        public List<string> groupOfStartPoints { set; get; } //điểm xuất phát gợi ý
        public List<string> groupOfDestinations { set; get; } //điểm kết thúc gợi ý

        public QL_DANGKY_XE dangKyXeEntity { set; get; }

        public DangKyXeEditViewModel()
        {
            dangKyXeEntity = new QL_DANGKY_XE();
            groupOfHours = Utility.GetHours();
            groupOfMinutes = Utility.GetMinutes();

            groupOfLoaiChuyens = new List<SelectListItem>();
            groupOfLoaiChuyens.Add(new SelectListItem()
            {
                Value = LOAICHUYEN_CONSTANT.CHUYEN_NGANG_TUYEN.ToString(),
                Text = TENLOAICHUYEN_CONSTANT.CHUYEN_NGANG_TUYEN,
            });
            groupOfLoaiChuyens.Add(new SelectListItem()
            {
                Value = LOAICHUYEN_CONSTANT.CHUYEN_VE.ToString(),
                Text = TENLOAICHUYEN_CONSTANT.CHUYEN_VE
            });
        }

        public DangKyXeEditViewModel(QL_DANGKY_XE dangKyXeEntity)
        {
            this.dangKyXeEntity = dangKyXeEntity;
            groupOfHours = Utility.GetHours(dangKyXeEntity.GIO_XUATPHAT.GetValueOrDefault());
            groupOfMinutes = Utility.GetMinutes(dangKyXeEntity.PHUT_XUATPHAT.GetValueOrDefault());
            groupOfLoaiChuyens = new List<SelectListItem>();
            groupOfLoaiChuyens.Add(new SelectListItem()
            {
                Value = LOAICHUYEN_CONSTANT.CHUYEN_NGANG_TUYEN.ToString(),
                Text = TENLOAICHUYEN_CONSTANT.CHUYEN_NGANG_TUYEN,
                Selected = (dangKyXeEntity.LOAI_CHUYEN_ID == LOAICHUYEN_CONSTANT.CHUYEN_NGANG_TUYEN)
            });
            groupOfLoaiChuyens.Add(new SelectListItem()
            {
                Value = LOAICHUYEN_CONSTANT.CHUYEN_VE.ToString(),
                Text = TENLOAICHUYEN_CONSTANT.CHUYEN_VE,
                Selected = (dangKyXeEntity.LOAI_CHUYEN_ID == LOAICHUYEN_CONSTANT.CHUYEN_VE)
            });
        }
    }
}