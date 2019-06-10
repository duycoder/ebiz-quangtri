using Business.CommonBusiness;
using Business.CommonModel.QUANLYVANBAN;
using Business.CommonModel.QUANLYHOSO;
using Model.Entities;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Web.Areas.QuanLyHoSoArea.Models
{
    public class QuanLyHoSoDetailViewModel
    {
        public QuanLyHoSoDetailBO HoSo { get; set; }
        public string NguoiNhap { get; set; }
        public ListVanBanBO ListVanBan { get; set; }
    }

    public class QuanLyHoSoIndexViewModel
    {
        public PageListResultBO<QuanLyHoSoBO> Source { get; set; }
        public List<SelectListItem> ListHoSoNam { get; set; }
        public List<SelectListItem> ListNamChinhLy { get; set; }
        public List<SelectListItem> ListKho { get; set; }
        public List<SelectListItem> ListPhongBan { get; set; }
    }

    public class QuanLyHoSoModel
    {
        //public List<QuanLyHoSoBO> ListHoSo { get; set; }
        public int Limit { get; set; }

        public PageListResultBO<QuanLyHoSoBO> ListHoSo { get; set; }
        public List<SelectListItem> ListPhong { get; set; }
        public List<SelectListItem> ListKho { get; set; }
        public List<SelectListItem> NamChinhLy { get; set; }
        public List<SelectListItem> ListNamHoSo { get; set; }
        public List<SelectListItem> ListThoiHan { get; set; }
        public QUANLY_HOSO HoSo { get; set; }

        /// <summary>
        /// Muc do truy cap
        /// </summary>
        public List<SelectListItem> ListAccessModifier { get; set; }

        public List<SelectListItem> ListNguoiNhap { get; set; }
        public int CountTotal { get; set; }
    }

    public class ImportHoSoModel
    {
        public List<DM_DANHMUC_DATA> ListHanBaoQuan { get; set; }
    }

    public class ImportHoSoPreviewModel
    {
        public List<HoSoImportBO> ListSuccess { get; set; }
        public List<HoSoImportBO> ListError { get; set; }
    }

    public class HoSoImportBO
    {
        public string HOPSO { get; set; }
        public string HOSO_SO { get; set; }
        public string TIEUDE_HOSO { get; set; }
        public string SOTO { get; set; }
        public string THOIHAN_BAOQUAN { get; set; }
        public string THOIGIAN_TAILIEU { get; set; }
        public string GHICHU { get; set; }
        public string MESSAGE { get; set; }
        public bool IS_ERROR { get; set; }
    }
}