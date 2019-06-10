using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Entities;
using Business.CommonBusiness;
using System.Web.Mvc;
using Business.CommonModel;
using Business.CommonModel.QUANLYVANBAN;

namespace Web.Areas.QuanLyHoSoArea.Models
{
    public class VanBanDetailViewModel
    {
        public DetailVanBanBO VanBan { get; set; }
        public List<TAILIEUDINHKEM> ListTaiLieu { get; set; }
    }
    public class QuanLyVanBanIndexViewModel
    {
        public PageListResultBO<VanBanPageListBO> Source { get; set; }
        public List<SelectListItem> ListHoSoNam { get; set; }
        public List<SelectListItem> ListNamChinhLy { get; set; }
        public List<SelectListItem> ListKho { get; set; }
        public List<SelectListItem> ListPhongBan { get; set; }
    }
    public class FormVanBanModel
    {
        public List<SelectListItem> ListHoSo { get; set; }
        public List<SelectListItem> ListNgonNgu { get; set; }
        public List<SelectListItem> ListCoQuanBanHanh { get; set; }
        public List<SelectListItem> ListLinhVuc { get; set; }
        public List<SelectListItem> ListLoaiVanBan { get; set; }
        public List<SelectListItem> ListDoMat { get; set; }
        public List<SelectListItem> ListMucDoTruyCap { get; set; }
        public List<SelectListItem> ListTinhTrangVatLy { get; set; }
        public List<TAILIEUDINHKEM> ListTaiLieu { get; set; }
        public QUANLY_VANBAN VanBan { get; set; }
        public long? HOSO_ID { get; set; }

    }
}