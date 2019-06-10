using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;

namespace Business.CommonModel.DMNguoiDung
{
    public class DM_NGUOIDUNG_BO : DM_NGUOIDUNG
    {
        public int DM_VAITRO_ID { set; get; }
        public string ChucVu { get; set; }
        public string TenPhongBan { get; set; }
        public int DeptType { set; get; } //loại phòng ban
        public int DeptParentId { set; get; } //phòng ban cha
        public int DeptCategory { set; get; } //mã loại phòng ban
        public string TEN_DONVI { get; set; }
        public long? DUNGLUONG { get; set; }
        public bool? TRANGTHAI_2 { get; set; }
        public int? TYPE { get; set; }
        public int? ChucVu_Id { get; set; }
        public int? PhongBan_Id { get; set; }
        public string ERROR { get; set; }
        public string NGAYSINH_TEXT { get; set; }
        public string TEN_VAITRO { get; set; }
        public string MA_VAITRO { set; get; }
        public List<string> LstVaiTro { get; set; }
        public string txtVaiTro
        {
            get
            {
                if (LstVaiTro != null && LstVaiTro.Any())
                {
                    return string.Join(", ", LstVaiTro);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public DM_NGUOIDUNG ToModel()
        {
            DM_NGUOIDUNG model = new DM_NGUOIDUNG();
            model.ANH_DAIDIEN = this.ANH_DAIDIEN;
            model.CHUCVU_ID = CHUCVU_ID;
            model.CODERESETPASS = CODERESETPASS;
            model.DIACHI = DIACHI;
            model.DIENTHOAI = DIENTHOAI;
            model.DM_PHONGBAN_ID = DM_PHONGBAN_ID;
            model.EMAIL = EMAIL;
            model.EMAILPASS = EMAILPASS;
            model.HOTEN = HOTEN;
            model.ID = ID;
            model.MAHOA_MK = MAHOA_MK;
            model.MATKHAU = MATKHAU;
            model.NGAYSINH = NGAYSINH;
            model.NGAYSUA = NGAYSUA;
            model.NGAYTAO = NGAYTAO;
            model.NGUOISUA = NGUOISUA;
            model.NGUOITAO = NGUOITAO;
            model.TENDANGNHAP = TENDANGNHAP;
            model.TOKEN = TOKEN;
            model.TRANGTHAI = TRANGTHAI;
            model.IS_ACTIVE = IS_ACTIVE;
            return model;
        }
    }
}
