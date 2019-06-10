using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Business.CommonModel.CONSTANT
{
    public class PLANJOB_CONSTANT
    {
        public static int CHUALAPKEHOACH = 0;
        public static int CHUATRINHKEHOACH = 1;
        public static int DATRINHKEHOACH = 2;
        public static int DAPHEDUYETKEHOACH = 3;
        public static int LAPLAIKEHOACH = 4;
    }
    public class DMLOAI_CONSTANT
    {
        public static string LOAITANG = "DMLOAITANG";
        public static string QUANHECHUHO = "QUANHEVOICHUHO";
        public static string LOAITHIETBI = "LOAITHIETBI";
        //Chức vụ
        public static string CHUCVU = "DMCHUCVU";

        //Danh sách loại thẻ
        public static string LOAITHE = "DMLOAITHE";
        public static string THE_LEVEL = "DMTHELEVEL";
        //Trạng thái sửa chữa bảo trì
        public static string TT_SUACHUABAOTRI = "DMSUACHUABAOTRI";
        public static string LOAIYEUCAU = "DMLOAIYEUCAU";
        public static string LOAIKHIEUNAI = "DMLOAIKHIEUNAI";
        //public static string DOKHAN = "DOKHAN";
        public static string DOKHAN = "DOQUANTRONG";
        public static string DOMAT = "DOMAT";
        public static string DOUUTIEN = "DOUUTIEN";
        public static string DOQUANTRONG = "DOQUANTRONG";
        public static string LINHVUCVANBAN = "LINHVUCVANBAN";
        public static string LOAI_VANBAN = "LOAIVANBAN";
        //đơn vị gửi văn bản đến
        public static string DONVIGUI_VANBAN = "DONVIGUI_VANBAN";
        //sổ văn bản đến và đi
        public static string SOVANBANDEN = "SOVANBANDEN";
        public static string SOVANBANDI = "SOVANBANDI";
        public static string DIEM_XUATPHAT = "DIEM_XUATPHAT";
        public static string DIEM_DEN = "DIEM_DEN";
        public static string DMPHONGHOP = "DMPHONGHOP";

        //Danh mục kho của quản lý hồ sơ
        public const string QLHS_KHO = "QLHS_KHO";

        //Danh mục mức độ truy cập của quản lý hồ sơ
        public const string QLHS_MUC_DO_TRUY_CAP = "QLHS_MUC_DO_TRUY_CAP";

        //Danh mục phông của quản lý hồ sơ
        public const string QLHS_PHONG = "QLHS_PHONG";

        //Danh mục thời hạn bảo quản của hồ sơ
        public const string QLHS_THOI_HAN_BAO_QUAN = "QLHS_THOI_HAN_BAO_QUAN";

        //Danh mục cơ quan banh hành của quản lý hồ sơ
        public const string QLHS_CO_QUAN_BAN_HANH = "QLHS_CO_QUAN_BAN_HANH";
        //Danh mục lĩnh vực văn bản
        //public const string QLHS_LINH_VUC_VAN_BAN = "QLHS_LINH_VUC_VAN_BAN";
        //Danh mục loại văn bản
        //public const string QLHS_LOAI_VAN_BAN = "QLHS_LOAI_VAN_BAN";
        //Danh mục ngôn ngữ
        public const string QLHS_NGON_NGU = "QLHS_NGON_NGU";
        //Danh mục tình trạng vật lý
        public const string QLHS_TINH_TRANG_VAT_LY = "QLHS_TINH_TRANG_VAT_LY";

    }
    public class MODULE_INFO
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Link { get; set; }
    }
    public class MODULE_CONSTANT
    {
        public static string VANBANTRINHKY = "MD_VANBANTRINHKY";
        public static string VANBANDEN = "MD_VANBANDEN";
        public static string VANBANDENNOIBO = "MD_VANBANDENNOIBO";
        public static string THE_THANGMAY = "MD_THANGMAY";
        public static int ID_THE_THANGMAY = 47;
        public static string THE_BOI = "MD_THEBOI";
        public static int ID_THE_BOI = 46;
        public static string THE_XE = "MD_THEXE";
        public static int ID_THE_XE = 48;
        public static string THE_GYM = "MD_THEGYM";
        public static int ID_THE_GYM = 49;
        //Sửa chữa bảo trì
        public static string SUACHUA_BAOTRI = "MD_SUACHUA_BAOTRI";
        //Yêu cầu của người dân
        public static string YEUCAU_NGUOIDAN = "MD_YEUCAU_NGUOIDAN";
        //Khiếu nại tố cáo
        public static string KHIEUNAI_TOCAO = "MD_KHIEUNAI_TOCAO";
        public static List<SelectListItem> GetList()
        {
            var listItem = new List<SelectListItem>();
            listItem.Add(new SelectListItem()
            {
                Text = "Văn bản đi",
                Value = VANBANTRINHKY
            });
            listItem.Add(new SelectListItem()
            {
                Text = "Văn bản đến",
                Value = VANBANDEN
            });
            listItem.Add(new SelectListItem()
            {
                Text = "Văn bản đến nội bộ",
                Value = VANBANDENNOIBO
            });
            return listItem;
        }
        public static MODULE_INFO GetNameModule(string module, long ItemId)
        {
            var mo = new MODULE_INFO();
            if (module.Equals(VANBANTRINHKY))
            {
                mo.Code = module;
                mo.Name = "Văn bản trình ký";
                mo.Link = "/HSVanBanDiArea/HSVanBanDi/DetailVanBan/" + ItemId.ToString();
                return mo;
            }
            if (module.Equals(THE_THANGMAY))
            {
                mo.Code = module;
                mo.Name = "thẻ thang máy";
                mo.Link = "/svthearea/svthe/viewdetail/" + ItemId.ToString();
                return mo;
            }
            if (module.Equals(THE_XE))
            {
                mo.Code = module;
                mo.Name = "thẻ xe";
                mo.Link = "/svthearea/svthe/viewdetail/" + ItemId.ToString();
                return mo;
            }
            if (module.Equals(THE_GYM))
            {
                mo.Code = module;
                mo.Name = "thẻ GYM";
                mo.Link = "/svthearea/svthe/viewdetail/" + ItemId.ToString();
                return mo;
            }
            if (module.Equals(SUACHUA_BAOTRI))
            {
                mo.Code = module;
                mo.Name = "sửa chữa bảo trì";
                mo.Link = "/SVSUACHUABAOTRIArea/SVSUACHUABAOTRI/ViewDetail/" + ItemId.ToString();
                return mo;
            }
            if (module.Equals(YEUCAU_NGUOIDAN))
            {
                mo.Code = module;
                mo.Name = "yêu cầu của người dân";
                mo.Link = "/svyeucaunguoidanarea/svyeucaunguoidan/viewdetail/" + ItemId.ToString();
                return mo;
            }
            if (module.Equals(KHIEUNAI_TOCAO))
            {
                mo.Code = module;
                mo.Name = "khiếu nại của người dân";
                mo.Link = "/FEKHIEULAITOCAOArea/FEKHIEULAITOCAO/ViewDetail/" + ItemId.ToString();
                return mo;
            }
            return mo;
        }
        public static string GetNameCard(int? idLoaiThe)
        {
            switch (idLoaiThe)
            {
                case 46:
                    return "Thẻ bơi";
                case 47:
                    return "Thẻ thang máy";
                case 48:
                    return "Thẻ xe";
                case 49:
                    return "Thẻ GYM";
                default:
                    break;
            }
            return "";
        }
        public static string GetCode(int idLoaiThe)
        {
            switch (idLoaiThe)
            {
                case 46:
                    return THE_BOI;
                case 47:
                    return THE_THANGMAY;
                case 48:
                    return THE_XE;
                case 49:
                    return THE_GYM;
                default:
                    break;
            }
            return "";
        }
    }
    public class FUNCTION_CONSTANT
    {
        public static string LUUSOPHATHANH = "LUUSOPHATHANH";
        public static string KYDUYETVANBAN = "KYDUYETVANBAN";
        public static string BDTHUCHIEN = "BDTHUCHIEN";
        public static string KTKEHOACH = "KTKEHOACH";
        //Cập nhật nội dung xử lý cho yêu cầu
        public static string NOIDUNGXULY = "YC_NOIDUNG";
        public static string BAOCAOKHIEUNAI = "BCKQ_KHIEUNAI";
        public static List<SelectListItem> GetList()
        {
            var listItem = new List<SelectListItem>();
            listItem.Add(new SelectListItem()
            {
                Text = "Lưu sổ phát hành",
                Value = LUUSOPHATHANH
            });
            listItem.Add(new SelectListItem()
            {
                Text = "Thực hiện kế hoạch",
                Value = BDTHUCHIEN
            });
            listItem.Add(new SelectListItem()
            {
                Text = "Ký duyệt văn bản",
                Value = KYDUYETVANBAN
            });
            //listItem.Add(new SelectListItem()
            //{
            //    Text = "Nội dung xử lý",
            //    Value = NOIDUNGXULY
            //});
            //listItem.Add(new SelectListItem()
            //{
            //    Text = "Báo cáo khiếu nại",
            //    Value = BAOCAOKHIEUNAI
            //});

            return listItem;
        }

    }
    public class LOAIYEUCAU_CONSTANT
    {
        public static int? TIEPNHAN = 1;
        public static int? TUCHOI = 2;
        public static int? CHOTIEPNHAN = null;
    }
    public class LOAITAILIEU
    {
        public const int TM_UPLOAD = 1;
        //Dùng cho văn bản trinh ky
        public const int VANBAN = 2;
        //Dùng cho công việc
        public const int CONGVIEC = 3;
        /// <summary>
        /// văn bản đã phát hành
        /// </summary>
        public const int VANBANDEN = 4;
        /// <summary>
        /// Danh mục dùng chung
        /// </summary>
        public const int MST_DIVISION = 5;
        /// <summary>
        /// Công việc
        /// </summary>
        public const int NOIDUNGTRAODOICONGVIEC = 14;
        public const int NOIDUNGTRAODOIVANBANDI = 140;
        public const int REVIEWVANBAN = 1400;
        /// <summary>
        /// Công việc
        /// </summary>
        public const int PLANCONGVIEC = 15;
        /// <summary>
        /// Công việc
        /// </summary>
        public const int CHAT = 16;

        //Quản lý hồ sơ
        public const int QUANLY_HOSO = 1999;
        //Quản lý văn bản
        public const int QUANLY_VANBAN = 2000;
    }
    public class PermissionVanbanModel
    {
        public const string DONVI = "QUANLY_DONVI";
        public const string PHONGBAN = "QUANLY_PHONGBAN";
        public const string CANHAN = "QUANLY_CANHAN";

        public const string QLHT_HUYENUY = "QLHT_HUYENUY";
        public const string QLHT_XAPHUONG = "QLHT_XAPHUONG";
        public const string QLHT = "QLHT";
    }
    public class LOAI_CONGVIEC
    {
        public const int CANHAN = 1;
        public const int DUOCGIAO = 2;
        public const int PHOIHOP_XULY = 3;
    }
    public class FcmConstant
    {
        public const string ServerKey = "AAAA5zInLc0:APA91bHVTUusNICFNHmAGBYksuq01wsA0WVCySVfLL3Y7z6qWoBBF5V47mE0TWm2s5wAr-i9eEK9iE3Fg2I646Yl2ihmZKPhrr4l47NwqbxVGioAdaJ8jHEzJmhxaL06sQhexGR0qRr3";
        public const string SenderId = "992978873805";
    }
    public class ElasticType
    {
        public const string CongViec = "JOB";
        public const string VanBanDi = "VANBANDI";
        public const string VanBanDen = "VANBANDEN";
    }
    public class TargetDocType
    {
        //Ca nhan
        public const int PRIVATE = 1;
        //Duoc giao xu ly
        public const int ASSIGNED = 2;
        //Phoi hop xu ly
        public const int COORDINATED = 3;
        //Da xu ly
        public const int PROCESSED = 4;
    }

    public class THONGBAO_CONSTANT
    {
        public const int CONGVIEC = 1;
        public const int VANBAN = 2;
    }

    public class VANBANDEN_CONSTANT
    {
        public const int CHUA_XULY = 1;
        public const int DA_XULY = 2;
        public const int THAMGIA_XULY = 3;
        public const int NOIBO_CHUAXULY = 4;
        public const int NOIBO_DAXULY = 6;
    }

    public class VANBANDI_CONSTANT
    {
        public const int CHUA_XULY = 1;
        public const int DA_XULY = 2;
        public const int THAMGIA_XULY = 3;
        public const int TAT_CA = 4;
        public const int DA_BANHANH = 5;
        public const int NOIBO = 6;
    }

    public class LICH_CONSTANT
    {
        public const int THANG = 1;
        public const int TUAN = 2;
        public const int NGAY = 3;
    }

    public class LOAIXE_CONSTANT
    {
        public const int XECHO_BENHNHAN = 1;
        public const int XECHO_CANBO = 2;
    }

    public class TENLOAIXE_CONSTANT
    {
        public const string XECHO_BENHNHAN = "Xe chở bệnh nhân";
        public const string XECHO_CANBO = "Xe chở cán bộ";
    }

    public class LOAICHUYEN_CONSTANT
    {
        public const int CHUYEN_NGANG_TUYEN = 1;
        public const int CHUYEN_VE = 2;
    }

    public class TENLOAICHUYEN_CONSTANT
    {
        public const string CHUYEN_NGANG_TUYEN = "Chuyển ngang tuyến";
        public const string CHUYEN_VE = "Chuyển về";
    }

    //trạng thái yêu cầu đăng ký xe
    public class TRANGTHAI_DANGKY_XE_CONSTANT
    {
        public const int MOITAO_ID = 1;
        public const int DAGUI_ID = 2;
        public const int DA_HUY_ID = 3;
        public const int DA_TIEPNHAN_ID = 4;
        public const int DANG_THUCHIEN_ID = 5;
        public const int DA_HOANTHANH_ID = 6;

        public const string MOITAO_TEXT = "Mới tạo";
        public const string DAGUI_TEXT = "Đã gửi";
        public const string DA_HUY_TEXT = "Đã hủy";
        public const string DA_TIEPNHAN_TEXT = "Đã tiếp nhận";
        public const string DANG_THUCHIEN_TEXT = "Đang thực thi";
        public const string DA_HOANTHANH_TEXT = "Đã hoàn thành";

        public const string MOITAO_COLOR = "#3276b1";
        public const string DAGUI_COLOR = "orange";
        public const string DA_HUY_COLOR = "red";
        public const string DA_TIEPNHAN_COLOR = "#0984e3";
        public const string DANG_THUCHIEN_COLOR = "#00b894";
        public const string DA_HOANTHANH_COLOR = "blue";
    }


    //trạng thái chuyến

    public class TRANGTHAI_CHUYEN_CONSTANT
    {
        public const int MOITAO_ID = 1;
        public const int DANGCHAY_ID = 2;
        public const int DA_HOANTHANH_ID = 3;

        public const string MOITAO_TEXT = "Mới tạo";
        public const string DANGCHAY_TEXT = "Đang chạy";
        public const string DA_HOANTHANH_TEXT = "Đã hoàn thành";

        public const string MOITAO_COLOR = "blue";
        public const string DANGCHAY_COLOR = "green";
        public const string DA_HOANTHANH_COLOR = "red";
    }

    //kiểu báo cáo chuyến
    public class LOAI_BAOCAO_THOIGIAN_CONSTANT
    {
        public const int NGAY = 1;
        public const int THANG = 2;
        public const int NAM = 3;
    }

    //kiểu báo cáo văn bản
    public class LOAI_BAOCAO_VANBAN_CONSTANT
    {
        public const int BAOCAO_LOAI_VANBANDEN = 1;
        public const int BAOCAO_LINHVUC_VANBANDEN = 2;

        public const int BAOCAO_HINHTHUC_VANBANDI = 3;
        public const int BAOCAO_LINHVUC_VANBANDI = 4;

        public const int BAOCAO_DONVIGUI_VANBANDEN = 5;
        public const int BAOCAO_DONVINHAN_VANBANDI = 6;
    }
    //model chia sẻ tài liệu
    public class SHARE_USER_CONSTANT
    {
        public const int REQUEST = 1;
        public const int SHARE = 2;
        public const int APPROVE = 3;
    }
    public class SHARE_STATUS_CONSTANT
    {
        public const int YEU_CAU_CHIA_SE = 1;
        public const int PHE_DUYET_CHIA_SE = 2;
        public const int DA_CHIA_SE = 3;
        public const int KHONG_CHIA_SE = 4;
    }
}
