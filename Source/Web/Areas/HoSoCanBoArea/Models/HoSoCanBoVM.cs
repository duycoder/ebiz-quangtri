using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model.Entities;

namespace Web.Areas.HoSoCanBoArea.Models
{
    public class HoSoCanBoVM
    {
        public HOSO_CANBO_THONGTINCHUNG HoSo { get; set; }
        public List<SelectListItem> LstGioiTinh { get; set; }
        public List<SelectListItem> LstDanToc { get; set; }
        public List<SelectListItem> LstTonGiao { get; set; }
        public List<SelectListItem> LstNgach { get; set; }
        public List<SelectListItem> LstTrinhDoGiaoDuc { get; set; }
        public List<SelectListItem> LstTrinhDoChuyenMon { get; set; }
        public List<SelectListItem> LstLyLuanChinhTri { get; set; }
        public List<SelectListItem> LstQuanLyNhaNuoc { get; set; }
        public List<SelectListItem> LstNgoaiNgu { get; set; }
        public List<SelectListItem> LstTinHoc { get; set; }
        public List<SelectListItem> LstTinhTrangSucKhoe { get; set; }
        public List<SelectListItem> LstNhomMau { get; set; }
        public List<SelectListItem> LstGiaDinhChinhSach { get; set; }
        public List<SelectListItem> LstChucVu { get; set; }
        public List<SelectListItem> LstDonViHienTai { get; set; }
    }
}