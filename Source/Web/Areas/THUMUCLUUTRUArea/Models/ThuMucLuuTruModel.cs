using Business.CommonBusiness;
using Business.CommonModel.DUNGLUONGLUUTRU;
using Business.CommonModel.EFILECHIASE;
using Business.CommonModel.TAILIEUDINHKEM;
using Business.CommonModel.TAILIEUDINHKEMVERSION;
using Business.CommonModel.THUMUCLUUTRU;
using Model.Entities;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Web.Areas.THUMUCLUUTRUArea.Models
{
    public class ThuMucLuuTruModel
    {
        public List<int?> listNam { get; set; }
        public List<SelectListItem> lstDonvi { get; set; }
        public List<SelectListItem> lstPhongBan { get; set; }
        //Danh sách thư mục và tài liệu đã bị xóa
        public List<THUMUC_LUUTRU_BO> lstFolders { get; set; }
        public THUMUC_LUUTRU ThuMuc { get; set; }
        public int FileSize { get; set; }
        public string Extension { get; set; }
        public long FolderId { get; set; }
        public TAILIEUDINHKEM TaiLieu { get; set; }
        public string UrlNavigation { get; set; }
        public List<TAILIEUDINHKEM_VERSION_BO> ListVersion { get; set; }
        public List<SelectListItem> ListAccessModifier { get; set; }
        public List<DM_DANHMUC_DATA> ListLoaiTaiLieu { get; set; }
        public List<SelectListItem> ListFolderPermission { get; set; }
        public List<TAILIEUTHUOCTINH_BO> ListThuocTinhBO { get; set; }
        public List<TAILIEU_THUOCTINH> ListThuocTinh { get; set; }
        public DM_DANHMUC_DATA LoaiTaiLieu { get; set; }
        public bool IsDetail { get; set; }
        public PageListResultBO<THUMUC_LUUTRU_BO> ListThuMuc { get; set; }
        public bool IsFolder { get; set; }
        public int DONVI_ID { get; set; }
        public List<EFILE_CHIASE_BO> ListChiaSe { get; set; }
        public UserInfoBO userInfoBo { get; set; }
        public EFILE_CHIASE ChiaSe { get; set; }
        public DM_NGUOIDUNG NguoiDung { get; set; }
        public DUNGLUONG_LUUTRU_BO DungLuong { get; set; }
        public List<string> ListClass { get; set; }
        public int AccessModifer { get; set; }
        public List<long> Ids { get; set; }

    }
    public class ThuMucLuuTruConstant
    {
        /// <summary>
        /// Danh mục loại tài liệu
        /// </summary>
        public const int LoaiTaiLieu = 21;
        public const int Gigabyte = 2;
        public const int Terabyte = 1;
        public const int Megabyte = 3;
        public const int Kilobyte = 4;
        /// <summary>
        /// Id thư mục văn bản chờ ký
        /// </summary>
        public const int DefaultVanban = 12;
        /// <summary>
        /// ID thư mục công việc
        /// </summary>
        public const int DefaultCongViec = 13;
        /// <summary>
        /// Id thư mục cá nhân
        /// </summary>
        public const long DefaultPrivate = 8;
        /// <summary>
        /// Id thư mục toàn đơn vị
        /// </summary>
        public const int DefaultUnit = 9;
        /// <summary>
        /// Id thư mục phòng ban
        /// </summary>
        public const int DefaultDept = 10;
        /// <summary>
        /// Id thư mục toàn bộ hệ thống
        /// </summary>
        public const int DefaultSys = 11;
        /// <summary>
        /// Id thư mục văn bản đã phát hành
        /// </summary>
        public const int DefaultVbDen = 40;
        /// <summary>
        /// Mặc định là 2Gb
        /// </summary>
        public const long DefaultStorage = 2;
        /// <summary>
        /// Đơn vị tính dung lượng lưu trữ
        /// </summary>
        public const int DetaultType = 2;
    }
    public class CommonFolder
    {
        public const string IF_FOLDER_BLUE = "if_folder_blue";
        public const string IF_FOLDER_CLOSE = "if_folder_close";
        public const string IF_FOLDER_GREEN = "if_folder_green";
        public const string IF_FOLDER_HOUSE = "if_folder_house";
        public const string IF_FOLDER_MUSIC = "if_folder_music";
        public const string IF_FOLDER_OPEN = "if_folder_open";
        public const string IF_FOLDER_PICTURE = "if_folder_picture";
        public const string IF_FOLDER_SEARCH = "if_folder_search";
        public const string IF_FOLDER_VIDEO = "if_folder_video";
        public const string IF_FOLDER_INFORMATION = "if_folder_information";
        public const string IF_FOLDER_LOCKEDC = "if_folder_lockedc";
        public const string IF_FOLDER_CHECKED = "if_folder_checked";
        public const string IF_FOLDER_REMOVE = "if_folder_remove";
    }
}