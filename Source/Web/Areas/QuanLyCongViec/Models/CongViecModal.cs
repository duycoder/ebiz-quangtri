using Business.CommonBusiness;
using Model.Entities;
using System.Collections.Generic;
using System.Web.Mvc;
using Business.CommonModel.HSCVVANBANDI;

namespace Web.Areas.CongViecArea.Models
{
    public class CongViecModal
    {        
        public PHIEUDANHGIACONGVIEC PhieuDanhGia { get; set; }
        //public VanBanDiBO VanBanDi { get; set; }
        //public VanBanDenBO VanBanDen { get; set; }
        public HSCV_CONGVIEC CongViec { get; set; }
        public List<SubTaskBO> LstImportantTask { get; set; }
        public List<SubTaskBO> LstNormalTask { get; set; }
        public List<SubTaskBO> LstCompletedTask { get; set; }
        public List<HSCV_CAPNHATTIENDO_CV> LstCapNhat { get; set; }
        public List<UserComment> LstNoiDungTraoDoi { get; set; }
        public List<UserComment> LstRootComment { get; set; }
        public List<long> LstFileId { get; set; }
        public string BACKURL { get; set; }
        public bool HasRoleAssignTask { get; set; }
        public bool HasRoleAssignChuyenVien { get; set; }
        public bool HasRoleAssignDepartment { get; set; }
        public string NGUOIGIAOVIEC { get; set; }
        public List<string> LstNguoiThamGia { get; set; }
        public List<DM_NGUOIDUNG> LstNewNguoiThamGia { get; set; }
        public string NGUOIXULYCHINH { get; set; }        
        public bool IsNguoiGiaoViec { get; set; }
        public bool IsNguoiThucHienChinh { get; set; }
        public bool IsNguoiThamgia { get; set; }
        public List<TAILIEUDINHKEM> ListTaiLieuDinhKemVanBan { get; set; }
        public List<TAILIEUDINHKEM> ListTaiLieuDinhKem { get; set; }
        public List<TAILIEUDINHKEM> LstTaiLieuComment { get; set; }
        public List<CongViecLuiHanBO> LstXinLuiHan { get; set; }
        public List<HSCV_TRINHDUYETCONGVIEC> LstTrinhDuyet { get; set; }
        public string DOUUTIEN { get; set; }
        public string DOKHAN { get; set; }
        public List<HSCV_CONGVIEC_LAPKEHOACH> LstKeHoachCongViec { get; set; }
        public List<TAILIEUDINHKEM> ListTaiLieuDinhKemKeHoach { get; set; }
        public List<SelectListItem> ListDoKhan { get; set; }
        public List<SelectListItem> ListDoUuTien { get; set; }
        public List<SubTaskBO> LstTask { get; set; }
        public int TrangThaiKeHoach { get; set; }
        public List<SelectListItem> LstDanhGiaCongViec { get; set; }
        public List<DM_DANHMUC_DATA> LstDanhGiaCongViecObj { get; set; }
        public HSCV_VANBANDEN VanBanDenLienQuan { get; set; }
        public HSCV_VANBANDI_BO VanBanDiLienQuan { get; set; }
    }
}