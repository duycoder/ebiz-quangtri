using Business.CommonBusiness;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Business.CommonModel.HSCVVANBANDI;
using Web.Areas.HSVanBanDiArea.Models;

namespace Web.Areas.CongViecArea.Models
{
    public class CongViecIndexViewModel : ICloneable
    {
        public List<SelectListItem> ListDoKhan { get; set; }
        public List<SelectListItem> ListDoUuTien { get; set; }
        public List<SelectListItem> ListTrangThai { get; set; }
        public HSCV_CONGVIEC CongViecCreated { get; set; }
        public bool HAS_ROLE_GIAOVIEC { get; set; }
        public int SearchStaffMode { set; get; }
        public List<CongViecBO> ListCongViec { set; get; }
        public string TieuDe { set; get; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public CongViecCountModel CongViecCounter { set; get; }
        public int pageIndex { set; get; }
        public int TYPE { set; get; }
        public UserInfoBO UserInfo { set; get; }
        public PageListResultBO<CongViecBO> ListResult { get; set; }
        public string ROLE { get; set; }
        public long ParentId { get; set; }
        public string BackgroundColor { get; set; }
        public string Color { get; set; }
        public string RowNo { get; set; }
        public long RootId { get; set; }
        public int Level { get; set; }
    }
    public class CongViecViewModel
    {
        public HSCV_CONGVIEC CongViec { get; set; }
        public UserInfoBO UserInfo { get; set; }
        public VanBanDiVM VanBanDi { get; set; }
        public string TenCongViec { get; set; }
        public string TenCongViecGoc { set; get; }
        public string NoiDungCongViec { get; set; }
        public bool HasCongVan { get; set; }
        public List<SelectListItem> ListDoUuTien { get; set; }
        public List<SelectListItem> ListDoKhan { get; set; }
        public List<SelectListItem> ListTrangThai { get; set; }
        public List<TAILIEUDINHKEM> ListTaiLieu { get; set; }
        public List<TAILIEUDINHKEM> ListTaiLieuCongViec { get; set; }
        public bool HAS_ROLE_GIAOVIEC { set; get; }
        public bool HAS_ROLE_CHUYENTIEP_CONGVIEC { set; get; }
        public bool IS_LANHDAO_CUC { set; get; }
        public bool IS_IN_ROLE_TRUONGDONVI { set; get; }
        public bool IS_CONGVIEC_CHUYENTIEP { set; get; }
        public bool IS_CONGVIEC_PHONG { set; get; }
        public bool IS_CONGVIEC_NHANVIEN { set; get; }
        public bool IS_CONGVIEC_CANHAN { set; get; }
        public CongViecCountModel CongViecCounter { set; get; }
        public int TYPE { get; set; }
        public HSCV_VANBANDEN VanBanDenLienQuan { get; set; }
        public HSCV_VANBANDI_BO VanBanDiLienQuan { get; set; }
        public long IdVanBanLienQuan { get; set; }
        public string ROLE { get; set; }
        public UserInfoBO userInfo { get; set; }
        public DM_NGUOIDUNG NguoiDung { get; set; }
        public string LIST_ROLE { get; set; }
        public string IdxTr { get; set; }
    }
    public class CongViecDetailViewModel
    {
        public CongViecBO CongViec { get; set; }
        public HSCV_CONGVIEC CongViecGoc { set; get; }
        public UserInfoBO UserInfo { set; get; }
        public bool HasCongVan { get; set; }
        public List<TAILIEUDINHKEM> ListTaiLieuDinhKemCuaVanBan { get; set; }
        public List<TAILIEUDINHKEM> ListTaiLieuDinhKem { get; set; }
        public List<CongViecNoiDungTraoDoiBO> ListNoiDungTraoDoi { get; set; }
        public List<HSCV_CAPNHATTIENDO_CV> ListTienDoCongViec { get; set; }
        public List<HSCV_CONGVIEC> ListCongViecChuyenTiep { set; get; }
        public List<SelectListItem> ListTrangThai { get; set; }
        public List<CongViecLuiHanBO> ListLuiHan { get; set; }
        public bool HAS_ROLE_BOSUNGTHANHVIEN { get; set; }
        public bool HAS_ROLE_THAYDOITRANGTHAI { get; set; }
        public bool HAS_ROLE_GHINOIDUNGCONGVIEC { get; set; }
        public bool HAS_ROLE_EDITCONGVIEC { get; set; }
        public bool HAS_ROLE_DELETE_CONGVIEC { set; get; }
        public bool HAS_ROLE_XINLUIHAN { get; set; }
        public bool HAS_ROLE_GIAOVIEC { get; set; }
        public bool HAS_ROLE_CHUYENTIEP_CONGVIEC { set; get; }
        public bool IS_CONGVIEC_CHUYENTIEP { set; get; }
        public bool HAS_ROLE_PHEDUYET_LUIHAN { set; get; }
        public CongViecCountModel CongViecCounter { set; get; }
        //tiến độ công việc
        public int CURRENT_PROGRESS { set; get; }
    }
    public class Permission
    {
        public const string GIAOVIEC_CANHAN = "GIAOVIEC_CANHAN";
        public const string GIAOVIEC_PHONGBAN = "GIAOVIEC_PHONGBAN";
        public const string GIAOVIEC_DONVI = "GIAOVIEC_DONVI";
        public const string CANHAN = "CANHAN";
        public const string XACNHAPHONGHOP = "XACNHAN_PHONGHOP";
    }
    public class DefaultVaiTro
    {
        public const int TRUONGPHONG = 1005;
        public const int GIAMDOCDONVI = 1004;
        public const int BANTONGGIAMDOC = 1003;
    }
    public class TrangThaiCongViecConstant
    {
        public const int PENDING = 1;
        public const int APPROVED = 2;
        public const int UNAPPROVAL = 3;
    }
    public class XepLoaiCongViecConstant
    {
        public const int TOT = 1;
        public const int KHA = 2;
        public const int DAT = 3;
        public const int KHONGDAT = 4;
    }
}