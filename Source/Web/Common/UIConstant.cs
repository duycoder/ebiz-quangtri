
namespace Web.Common
{
    public class UIConstant
    {
        public const string COMBOBOX_ALL_VALUE = "[Tất cả]"; // gia tri tat ca
        public const string COMBOBOX_CHOOSE_VALUE = "[Lựa chọn]"; // gia tri lua chon
        //Result Action
        public const string RESULT_DELETE = "RESULT_DELETE";

        //Indicator Group
        public const string LIST_INDICATOR_GROUP = "LIST_INDICATOR_GROUP"; // danh sach nhom tieu chi
        public const string LIST_INDICATOR = "LIST_INDICATOR"; // danh sach tieu chi
        public const string COMBOBOX_STATUS = "LIST_STATUS"; // danh sach trang thai
        public const string COMBOBOX_SINGLE_STATUS = "COMBOBOX_SINGLE_STATUS"; // danh sach trang thai khong co tat ca
        public const string COMBOBOX_INDICATOR = "COMBOBOX_INDICATOR"; // danh sach tieu chi

        //Indicator 
        public const string CREATE_OR_EDIT_INDICATOR = "CREATE_OR_EDIT_INDICATOR"; // tieu chi

        //Active

        public const string COMBOBOX_STATUS_ITEM2_VALUE = "Hiệu lực"; // trang thai hieu luc
        public const string COMBOBOX_STATUS_ITEM3_VALUE = "Hết hiệu lực"; // trang thai het hieu luc

        public const string COMBOBOX_STATUS_ITEM1_NAME = "-1"; // trang thai tat ca
        public const string COMBOBOX_STATUS_ITEM2_NAME = "1"; // trang thai hieu luc
        public const string COMBOBOX_STATUS_ITEM3_NAME = "0"; // trang thai het hieu luc

        //App param
        public const string APP_PARAM_NAME = "APP_PARAM_NAME"; //Ten session AppParam
        public const int APP_TYPE_UNIT = 1; //Đơn vị tính UNIT_ID
        public const int APP_TYPE_VALUE_TYPE = 3; //Kiểu dữ liệu VALUE_TYPE
        public const int APP_TYPE_INPUT_TYPE = 4; //Nhập liệu INPUT_TYPE

        //Name of AppType
        public const string APP_TYPE_NAME_UNIT = "UNIT_ID"; //Đơn vị tính UNIT_ID
        public const string APP_TYPE_NAME_VALUE_TYPE = "VALUE_TYPE"; //Kiểu dữ liệu VALUE_TYPE
        public const string APP_TYPE_NAME_INPUT_TYPE = "INPUT_TYPE"; //Nhập liệu INPUT_TYPE

        //Period
        public const string PERIOD_NAME = "PERIOD_NAME"; //Ten viewdata PERIOD_NAME
        public const string DEPT_NAME = "DEPT_NAME";
        public const string PERIOD_ID = "PERIOD_ID"; //Ten viewdata PERIOD_ID

        //Loai DVQL        
        public const string COMBOBOX_DVQL_ITEM0_NAME = "Bộ Giáo dục Đào tạo";
        public const string COMBOBOX_DVQL_ITEM1_NAME = "Sở Giáo dục Đào tạo";
        public const string COMBOBOX_DVQL_ITEM2_NAME = "Trường PTTH";

        public const string COMBOBOX_DVQL_ITEM0_VALUE = "0";
        public const string COMBOBOX_DVQL_ITEM1_VALUE = "1";
        public const string COMBOBOX_DVQL_ITEM2_VALUE = "2";

        //District
        public const string MA_QUANHUYEN = "MA_QUANHUYEN";
        public const string TEN_QUANHUYEN = "TEN_QUANHUYEN";


        //Dia diem thi
        public const string DM_DIADIEMTHI_ID = "DM_DIADIEMTHI_ID";
        public const string TEN_DIADIEMTHI = "TEN_DIADIEMTHI";

        //Commune
        public const string MA_XAPHUONG = "MA_XAPHUONG";
        public const string TEN_XAPHUONG = "TEN_XAPHUONG";

        //NhomUT
        public const string MA_NHOMUT = "MA_NHOMUT";
        public const string TEN_NHOMUT = "TEN_NHOMUT";

        //Loại cụm        
        public const string COMBOBOX_LOAICUM_ITEM1_VALUE = "Địa phương";
        public const string COMBOBOX_LOAICUM_ITEM2_VALUE = "Đại học";

        public const string COMBOBOX_LOAICUM_ITEM0_NAME = "-1";
        public const string COMBOBOX_LOAICUM_ITEM1_NAME = "1";
        public const string COMBOBOX_LOAICUM_ITEM2_NAME = "2";

        //Đơn vị quản lý
        public const string MA_DVQL = "MA_DVQL";
        public const string TEN_DVQL = "TEN_DVQL";

        //Province
        public const string MA_TINH = "MA_TINH";
        public const string TEN_TINH = "TEN_TINH";

        //Đối tượng ưu tiên
        public const string MA_DTUT = "MA_DTUT";
        public const string TEN_DTUT = "TEN_DTUT";

        //Template
        public const string TEMPLATE_ID = "TEMPLATE_ID"; //Ten viewdata TEMPLATE_ID
        public const string IS_LOAD_ALL = "IS_LOAD_ALL"; //Ten viewdata IS_LOAD_ALL

        //Data version detail 
        public const string DATA_VERSION_ID = "DATA_VERSION_ID"; //Ten viewdata DATA_VERSION_ID
        public const string DATA_VERSION_STATUS = "DATA_VERSION_STATUS"; //Ten viewdata trang thai DATA_VERSION
        public const string TEMPLATE_TREE = "TEMPLATE_TREE"; //Ten viewdata cay bieu mau
        public const string TEMPLATE_DETAIL = "TEMPLATE_DETAIL"; //Ten  viewdata chi tiet bieu mau
        public const string MAPPING_INDICATOR = "MAPPING_INDICATOR"; //MappingObject viewdata dung de luu du lieu mapping indicator va gia tri
        public const string VISIBLE_BUTTON = "VISIBLE_BUTTON";//dung de an hien cac button luu thanh ban khac, chot phien ban, xuat excel

        //Xem du lieu
        public const string Viewdata = "Xem dữ liệu";
        public const string STATUS_NOTSEND = "Chưa gửi dữ liệu";
        public const string STATUS_SEND = "Đã gửi dữ liệu";
        public const string STATUS_APROVED = "Đã chốt dữ liệu";
        public const string STATUS_REFUSE = "Yêu cầu tổng hợp lại";

        //Reporting Template
        public const string REPORTING_TEMPLATE_TREE_AREA = "REPORTING_TEMPLATE_TREE_AREA"; // Ten du lieu tree table khu vuc
        public const string REPORTING_TEMPLATE_GRID_PERIOD = "REPORTING_TEMPLATE_GRID_PERIOD"; // Ten du lieu gridview dot
        public const string REPORTING_TEMPLATE_ERROR_ATTACH = "REPORTING_TEMPLATE_ERROR_ATTACH"; // Ten du lieu file loi load len gridview
        public const string REPORTING_TEMPLATE_TEMPLATE_NAME = "REPORTING_TEMPLATE_TEMPLATE_NAME"; // Ten du lieu report template load combobox

        //SUPERVISING_DEPT
        public const string EMULATION_FLAG_VALUE1 = "1";
        public const string EMULATION_FLAG_VALUE2 = "2";
        public const string EMULATION_FLAG_NAME1 = "Chính phủ";
        public const string EMULATION_FLAG_NAME2 = "Bộ GD-ĐT";

        public const string COLLECTIVE_VALUE1 = "1";
        public const string COLLECTIVE_VALUE2 = "2";
        public const string COLLECTIVE_NAME1 = "LĐ xuất sắc";
        public const string COLLECTIVE_NAME2 = "LĐ tiên tiến";

        public const string TRAINING_TYPE_NAME1 = "Công lập";
        public const string TRAINING_TYPE_NAME2 = "Dân lập";
        public const string TRAINING_TYPE_NAME3 = "Tư thục";
        public const string TRAINING_TYPE_NAME4 = "Bán công";

        public const string TRAINING_TYPE_VALUE1 = "1";
        public const string TRAINING_TYPE_VALUE2 = "2";
        public const string TRAINING_TYPE_VALUE3 = "3";
        public const string TRAINING_TYPE_VALUE4 = "4";

        public const string NATIONAL_SCHOOL_NAME1 = "Không";
        public const string NATIONAL_SCHOOL_NAME2 = "Mức 1";
        public const string NATIONAL_SCHOOL_NAME3 = "Mức 2";

        public const string NATIONAL_SCHOOL_VALUE1 = "0";
        public const string NATIONAL_SCHOOL_VALUE2 = "1";
        public const string NATIONAL_SCHOOL_VALUE3 = "2";

        //GLOBAL Message
        public const string LIST_AREA_1 = "LOGIN";
        public const string LIST_AREA_2 = "HOME";
        public const string LIST_AREA_3 = "LOGIN/HOME";

        public const string LIST_AREA_VALUE_1 = "1";
        public const string LIST_AREA_VALUE_2 = "2";
        public const string LIST_AREA_VALUE_3 = "3";

        //dung cho ghi log
        public const string WILD_LOG = "*#*";
        public const string ACTION_AUDIT_OLD_JSON = "old json object";
        public const string ACTION_AUDIT_NEW_JSON = "new json object";
        public const string ACTION_AUDIT_OBJECTID = "id cua data modify";
        public const string ACTION_AUDIT_DESCRIPTION = "description";

        //Hinh thuc tiem chung
        public const int HINHTHUC_TCMR = 0; //Tiêm chủng mở rộng
        public const int HINHTHUC_TCCD = 1; //Tiêm chủng chiến dịch
        public const int HINHTHUC_TCDV = 2; //Tiêm chủng dịch vụ

        //Loai bao cao
        public const int BAOCAO_TUAN = 0;
        public const int BAOCAO_THANGTHEOCOSO = 1;
        public const int BAOCAO_THANGTHEODONVI = 2;
        public const int BAOCAO_NAMTHEODIAPHUONG = 3;
        public const int BAOCAO_NAMTHEOTHANG = 4;

        //So thang trong nam
        public const int THANG = 12;


        public const string FORGET_PASS_CAPTCHA = "forgetpassword";
        public const string MESSAGE_CONFIMATION_EMPTY = "Bạn chưa nhập mã xác nhận";
        public const string MESSAGE_CONFIMATION_NOT_CORRECT = "Mã xác nhận không đúng";
        public const string MESSAGE_DUPLICATE_EMAIL = "Email đã tồn tại!";
        public const string MESSAGE_CREATE_NEW_USER = "Tạo mới người dùng thành công!";
        public const string MESSAGE_UPDATE_USER = "Sửa người dùng thành công!";
        public const string MESSAGE_EMAIL_NOT_EXIST = "Email không tồn tại!";

        public const string HUONG_DAN_PDF = "~/TemplateFile/HUONG_DAN_SU_DUNG0408.pdf";

        public const string NAME_HUONG_DAN_PDF = "HUONG_DAN_SU_DUNG.pdf";
        public const string BO_Y_TE = "Bộ Y Tế Dự Phòng";
        public const string CUC_Y_TE = "Cục Y Tế Dự Phòng";
    }
    public class CAPTINH_HUYEN_XA
    {
        /// <summary>
        /// CẤP TỈNH
        /// </summary>
        public const int CAPTINH = 1;
        /// <summary>
        /// CẤP HUYỆN
        /// </summary>
        public const int CAPHUYEN = 2;
        /// <summary>
        /// CẤP XÃ
        /// </summary>
        public const int CAPXA = 3;
    }
    public class TRUONGHOPBENHCONSTANT
    {
        //Trạng thái theo dõi của trường hợp bệnh
        public const int DANGTHEODOI = 0; //Đang theo dõi
        public const int NGUNGTHEODOI = 1; //Ngừng theo dõi
        public const int TUVONG = 2; //Tử vong


        //Trạng thái phản hồi của trường hợp bệnh
        public const int CHUAPHANHOI = 0; //Chưa phản hồi
        public const int DAPHANHOI = 1; //Đã phản hồi

        //Thông tin cơ sở phát hiện bệnh
        public const int COSOPHATHIEN = 0; //Cơ sở phát hiện
        public const int COSODIEUTRI = 1; //Cơ sở điều trị

        //Thông tin sử dụng vắc xin
        public const int COSUDUNGVACXIN = 0; //Có sử dụng
        public const int KHONGSUDUNGVACXIN = 1; //Không sử dụng
        public const int KHONGROVACXIN = 2; //Không rõ

        //Thông tin phân loại chẩn đoán
        public const int LAMSANG = 0; //chẩn đoán lâm sàng
        public const int XETNGHIEM = 1; //Xác định phòng xét nghiệm

        //Thông tin lấy mẫu xét nghiệm
        public const int COLAYMAU = 0; //Có lấy mẫu xét nghiệm
        public const int KHONGLAYMAU = 1; //Không lấy mẫu xét nghiệm

        //Thông tin loại xét nghiệm
        public const int LOAITESTNHANH = 0; //Loại test nhanh
        public const int LOAIMACELISA = 1; //Loại Mac-elisa
        public const int LOAIPCR = 2; //Loại PCR
        public const int LOAIKHAC = 3; //Loại khác

        //Thông tin kết quả xét nghiệm
        public const int KQDUONGTINH = 0; //dương tính
        public const int KQAMTINH = 1; //âm tính
        public const int KQCHUACOKQ = 2; //chưa có KQ

        //Thông tin tình trạng
        public const int DIEUTRINGOAITRU = 0; //điều trị ngoại trú
        public const int DIEUTRINOITRU = 1; //điều trị nội trú
        public const int RAVIEN = 2; //ra viện
        public const int TINHTRANGTUVONG = 3; //tử vong
        public const int CHUYENVIEN = 4; //Chuyển viện
        public const int TINHTRANGKHAC = 5; //Khác

        //Ẩn hiện thông tin xét nghiệm
        public const int HIENTHIXN = 1;
        public const int AN_HIENTHIXN = 0;

        //GIỚI TÍNH
        public const int NAM = 1;
        public const int NU = 0;
    }

    public class TINHTRANGRAVIEN
    {
        //khỏi bệnh, ra viện
        public const int KHOI_RAVIEN = 0;
        //Đỡ, giảm chuyển tuyến dưới
        public const int DO_GIAM_CHUYENTUYENDUOI = 1;
        //Nặng, chuyển tuyến trên
        public const int NANG_CHUYENTUYENTREN = 2;
        //Tiên lượng tử vong xin về
        public const int TIENLUONG_TV_XINVE = 3;
    }
    public class BENHCHUANDOANCONSTANT
    {
        //Bệnh phải báo cáo trong vòng 24h
        public const int BAOCAO24H = 1;
        //Bệnh phải báo cáo trong vòng 72h
        public const int BAOCAO72H = 1;
        //Bệnh phải báo cáo trong tháng
        public const int BAOCAOTHANG = 1;
    }
    //public class CAPDONVI
    //{
    //    /// <summary>
    //    /// Đơn vị Cấp trung ương
    //    /// </summary>
    //    public const int CAPTRUNGUONG = 0;
    //    public const int CAPBENHVIENTRUNGUONG = 1;
    //    public const int CAPTINH = 2;
    //    public const int CAPBENHVIENTINH = 3;
    //    public const int CAPHUYEN = 4;
    //    public const int CAPBENHVIENHUYEN = 5;
    //    public const int CAPXA = 6;
    //}
    public class HIENTHIBAOCAO
    {
        public const int BENHHIENTHI = 1;//Bênh được hiển thị
        public const int BENHKHONGHIENTHI = 0;//Bệnh không được hiển thị
        public const int DIAPHUONGHIENTHI = 1;// Địa phương được hiển thị
        public const int DIAPHUONGKHONGHIENTHI = 0;//Địa phương không được hiển thị
    }
    public class PHANLOAIDONVI
    {
        /// <summary>
        /// CẤP ĐỊA PHƯƠNG TRONG DM_BENHVIEN
        /// </summary>
        public const int CAPDIAPHUONG = 1;
        /// <summary>
        /// CẤP TRUNG ƯƠNG TRONG DM_BENHVIEN
        /// </summary>
        public const int CAPTW = 0;

        /// <summary>
        /// Đơn vị Cấp trung ương
        /// </summary>
        public const int CAPTRUNGUONG = 0;
        /// <summary>
        /// đơn vị cấp tỉnh
        /// </summary>
        public const int CAPTINH = 0;
        /// <summary>
        /// đơn vị cấp bệnh viện tỉnh
        /// </summary>
        public const int CAPBENHVIENTINH = 1;
        /// <summary>
        /// đơn vị cấp huyện
        /// </summary>
        public const int CAPHUYEN = 0;
        /// <summary>
        /// đơn vị cấp bệnh viện huyện
        /// </summary>
        public const int CAPBENHVIENHUYEN = 1;
        /// <summary>
        /// đơn vị cấp xã
        /// </summary>
        public const int CAPXA = 0;
        /// <summary>
        /// đơn vị cấp bệnh viện trung ương
        /// </summary>
        public const int CAPBVTRUNGUONG = 1;

        /// <summary>
        /// Cấp đơn vị
        /// </summary>
        public const int CAPDONVI = 0;
        /// <summary>
        /// Câp bệnh viện
        /// </summary>
        public const int CAPBENHVIEN = 1;
    }
    public class PHANCAPDONVI
    {
        /// <summary>
        /// Đơn vị cấp trung ương
        /// </summary>
        public const int CAPTW = 0;
        /// <summary>
        /// đơn vị cấp bệnh viện trung ương
        /// </summary>
        public const int CAPBVTW = 1;
        /// <summary>
        /// đơn vị cấp tỉnh
        /// </summary>
        public const int CAPTINH = 2;
        /// <summary>
        /// đơn vị cấp bệnh viện tỉnh
        /// </summary>
        public const int CAPBVTINH = 3;
        /// <summary>
        /// đơn vị cấp huyện
        /// </summary>
        public const int CAPHUYEN = 4;
        /// <summary>
        /// đơn vị cấp bệnh viện huyện
        /// </summary>
        public const int CAPBVHUYEN = 5;
        /// <summary>
        /// đơn vị cấp xã
        /// </summary>
        public const int CAPXA = 6;
        /// <summary>
        /// cấp quản trị hệ thống
        /// COSO_ID=0 LA CAP QUAN TRI HE THONG
        /// </summary>
        public const int CAPQTHT = 7;
    }

    public class CAPDONVI
    {
        /// <summary>
        /// CẤP CỤC
        /// </summary>
        public const int CAPCUC = 0;
        /// <summary>
        /// CẤP SỞ
        /// </summary>
        public const int CAPSO = 1;
    }

    //public class TRANGTHAIODICH
    //{
    //    /// <summary>
    //    /// TẠO MỚI Ổ DỊCH
    //    /// </summary>
    //    public const int TAOMOIODICH = 0;

    //    /// <summary>
    //    /// Đang bùng phát
    //    /// </summary>
    //    public const short DANGBUNGPHAT = 0;
    //    /// <summary>
    //    /// Đã kết thúc
    //    /// </summary>
    //    public const int DAKETTHUC = 1;

    //    /// <summary>
    //    /// Xác định trường hợp bệnh đầu tiên
    //    /// </summary>
    //    public const int XACDINHTHBDAUTIEN = 0;


    //}
    public class MENU_POSITON
    {
        public const int LEFT = 0;

        public const int RIGHT = 1;
        public const int TOP = 2;
        public const int BOTTOM = 3;
    }
    public class BANNER_POSITON
    {
        public const int LEFT = 0;
        public const int RIGHT = 1;
        public const int TOP = 2;
        public const int BOTTOM = 3;
        public const int MIDDLE = 4;
    }



    public class LOAIHOSO
    {
        /// <summary>
        /// Văn bản đi
        /// </summary>
        public const int VANBANDI = 1;
        /// <summary>
        /// Văn bản đến
        /// </summary>
        public const int VANBANDEN = 2;

    }
}