using Business.CommonModel.DMNguoiDung;
using Model.Entities;
using System;
using System.Collections.Generic;

namespace Business.CommonBusiness
{
    public class CongViecBO : HSCV_CONGVIEC
    {
        public List<DM_NGUOIDUNG_BO> ListNguoiXuLyChinh { get; set; }
        public List<DM_NGUOIDUNG_BO> ListNguoiThamGiaXuLy { get; set; }
        public List<DM_NGUOIDUNG_BO> ListNguoiTheoDoi { get; set; }
        public List<DateTime> ThoiGianXuLyRange { set; get; }
        public int? DayDiff { get; set; }
        public string TEN_DOKHAN { get; set; }
        public int TRONGSOCONGVIEC { get; set; }
        public string TEN_DOUUTIEN { get; set; }
        public string ICON_DOKHAN { get; set; }
        public string ICON_DOUUTIEN { get; set; }
        public string TEN_NGUOIXULYCHINH { get; set; }
        public string TEN_NGUOIGIAOVIEC { get; set; }
        public int DeptId { get; set; }
        public string COLOR { get; set; }
        public long ParentId { get; set; }
        /// <summary>
        /// Thuộc tính check công việc con
        /// </summary>
        public bool HasChild { get; set; }
        public int TONGDIEM { get; set; }
        public string XEPLOAI { get; set; }
        public bool isExpand { set; get; }
        public List<long> parentIds { set; get; }
        public int? SONGAYCONLAI { get; set; }
        public string NGUOI_THAMGIA_XULY { set; get; }
        public List<long> IDS_THAMGIA_XULY { set; get; }
        public string NGAY_NHANVIEC_TEXT { set; get; }
        public string NGAYHOANTHANH_THEOMONGMUON_TEXT { set; get; }
    }
}
