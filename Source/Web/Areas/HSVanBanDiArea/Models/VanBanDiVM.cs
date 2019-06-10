using Business.CommonBusiness;
using Business.CommonModel.DMNguoiDung;
using Business.CommonModel.DMVAITRO;
using Business.CommonModel.HSCVVANBANDI;
using Business.CommonModel.QLNGUOINHANVANBAN;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.HSVanBanDiArea.Models
{
    public class VanBanDiVM
    {
        public string listTitle { set; get; }
        public int docType { set; get; }
        public string CodePublishDepartment { set; get; } //mã cơ quan ban hành
        public string CodeDocumentType { set; get; } //mã loại văn bản
        public List<SelectListItem> LstSoVanBanDi { get; set; }
        public List<SelectListItem> LstDoUuTien { get; set; }
        public List<SelectListItem> LstDoKhan { get; set; }
        public List<SelectListItem> LstLinhVucVanBan { get; set; }
        public List<SelectListItem> LstLoaiVanBan { get; set; }
        public List<SelectListItem> LstNguoiKyVanBan { get; set; }
        public List<SelectListItem> LstDonViNhan { get; set; }
        /// <summary>
        /// loại cơ quan
        /// </summary>
        public List<SelectListItem> GroupDeptTypes { set; get; }
        /// <summary>
        /// loại văn bản
        /// </summary>
        public List<SelectListItem> GroupDocTypes { set; get; }
        /// <summary>
        /// tác giả văn bản
        /// </summary>
        public List<SelectListItem> GroupDocAuthors { set; get; }
        public CCTCItemTreeBO TreeDonVi { get; set; }
        public HSCV_VANBANDI VanBan { get; set; }
        public HSCV_VANBANDEN VanBanDen { get; set; }
        public List<CCTC_THANHPHAN> ListDonVi { get; set; }
        public List<TAILIEUDINHKEM> ListTaiLieu { get; set; }
        public PageListResultBO<HSCV_VANBANDI_BO> ListResult { get; set; }
        public UserInfoBO UserInfoBO { get; set; }
        public DM_NGUOIDUNG NguoiKy { get; set; }
        public bool HasRoleThuHoi { get; set; }
        public int STATEBEGIN { get; set; }
        public int STARTSTATEBYUSER { get; set; }
        public bool IsAllowPublish { set; get; } //cho phép xuất bản
        public bool IsInternal { set; get; } //là văn bản đi nội bộ
        public List<DM_NGUOIDUNG_BO> GroupUsersReceiveDirectly { set; get; } //danh sách người dùng nhận văn bản gửi đích danh
        public List<SelectListItem> LstReceiveDirectly { get; set; }
        public List<long> GroupUserIdsReceiveDirectly { set; get; }
        public List<RoleReceiveDirectly> GroupRolesReceiveDirectly { set; get; }
        public List<SelectListItem> GroupUsersReceiveInternal { set; get; } //danh sách người dùng nhận bên nội bộ trong văn bản
        public List<long> GroupUserIdsReceiveInternal { set; get; }

        /// <summary>
        /// @người nhận văn bản trực tiếp
        /// </summary>
        public List<long> UsersReceived { set; get; }
        public List<QL_NGUOINHAN_VANBAN_BO> Recipients { set; get; }
    }
    public class ThongTinVanBanDiVM
    {
        public List<int> GroupDonViRead { set; get; } //danh sách đơn vị đã nhận văn bản
        public List<DM_NGUOIDUNG_BO> GroupUsersRead { set; get; } //danh sách người dùng đã đọc văn bản
        public List<DM_NGUOIDUNG_BO> GroupUsersReceiveDirectly { set; get; } //danh sách người dùng nhận văn bản đích danh
        public List<DM_NGUOIDUNG_BO> GroupUsersReceiveInternal { set; get; } //danh sách người dùng nhận văn bản nội bộ
        public string STR_DOKHAN { get; set; }
        public string STR_DOUUTIEN { get; set; }
        public string STR_LINHVUCVANBAN { get; set; }
        public string STR_LOAIVANBAN { get; set; }
        public string STR_NGUOIKY { get; set; }
        public string STR_LOAI_COQUAN { set; get; }
        public string STR_PHAN_LOAI_VANBAN { set; get; }
        public string STR_TACGIA { set; get; }
        public HSCV_VANBANDI VanBanTrinhKy { get; set; }
        public List<TAILIEUDINHKEM> ListTaiLieu { get; set; }
        public List<CCTC_THANHPHAN> ListDonVi { get; set; }
        public List<UserComment> LstNoiDungTraoDoi { get; set; }
        public List<UserComment> LstRootComment { get; set; }
        public List<TAILIEUDINHKEM> LstTaiLieuComment { get; set; }
        public  HSCV_VANBANDEN VanBanDen { get; set; }
        public List<DONVIREAD> LstDonViRead { get; set; }
        public bool HasPermissionMainProcess { get; set; }
        public int DocType { set; get; }
        public CCTCItemTreeBO TreeDonVi { get; set; }
        public List<QL_NGUOINHAN_VANBAN_BO> Recipients { set; get; }
        public bool CanSendOthers { set; get; }
        public UserInfoBO CurrentUser { set; get; }
    }

    public class DONVIREAD
    {
        public int DONVI_ID { get; set; }
        public int STATUS { get; set; }
    }

    public class RoleReceiveDirectly
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public List<DM_NGUOIDUNG_BO> Users { set; get; }
    }
}
