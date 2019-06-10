﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Model.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DBEntities : DbContext
    {
        public DBEntities()
            : base("name=DBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ACTION_AUDIT> ACTION_AUDIT { get; set; }
        public virtual DbSet<CCTC_THANHPHAN> CCTC_THANHPHAN { get; set; }
        public virtual DbSet<CHAT_GROUP> CHAT_GROUP { get; set; }
        public virtual DbSet<CHAT_GROUP_USER> CHAT_GROUP_USER { get; set; }
        public virtual DbSet<CHAT_NOIDUNG> CHAT_NOIDUNG { get; set; }
        public virtual DbSet<CHIASE_TAILIEU> CHIASE_TAILIEU { get; set; }
        public virtual DbSet<DM_CHUCNANG> DM_CHUCNANG { get; set; }
        public virtual DbSet<DM_DANHMUC_DATA> DM_DANHMUC_DATA { get; set; }
        public virtual DbSet<DM_DANHMUC_DATA_VAITRO> DM_DANHMUC_DATA_VAITRO { get; set; }
        public virtual DbSet<DM_LOAI_DONVI> DM_LOAI_DONVI { get; set; }
        public virtual DbSet<DM_NGUOIDUNG> DM_NGUOIDUNG { get; set; }
        public virtual DbSet<DM_NGUOIDUNG_THAOTAC> DM_NGUOIDUNG_THAOTAC { get; set; }
        public virtual DbSet<DM_NHOMDANHMUC> DM_NHOMDANHMUC { get; set; }
        public virtual DbSet<DM_THAOTAC> DM_THAOTAC { get; set; }
        public virtual DbSet<DM_VAITRO> DM_VAITRO { get; set; }
        public virtual DbSet<DUNGLUONG_LUUTRU> DUNGLUONG_LUUTRU { get; set; }
        public virtual DbSet<EFILE_CHIASE> EFILE_CHIASE { get; set; }
        public virtual DbSet<EFILE_CHIASE_NGUOIDUNG> EFILE_CHIASE_NGUOIDUNG { get; set; }
        public virtual DbSet<FCM_NOTIFICATION> FCM_NOTIFICATION { get; set; }
        public virtual DbSet<HOSO_CANBO_CONGTAC> HOSO_CANBO_CONGTAC { get; set; }
        public virtual DbSet<HOSO_CANBO_DAOTAO> HOSO_CANBO_DAOTAO { get; set; }
        public virtual DbSet<HOSO_CANBO_QUANHEBANTHAN> HOSO_CANBO_QUANHEBANTHAN { get; set; }
        public virtual DbSet<HOSO_CANBO_QUANHEKETHON> HOSO_CANBO_QUANHEKETHON { get; set; }
        public virtual DbSet<HOSO_CANBO_QUATRINH_LUONG> HOSO_CANBO_QUATRINH_LUONG { get; set; }
        public virtual DbSet<HOSO_CANBO_THONGTINCHUNG> HOSO_CANBO_THONGTINCHUNG { get; set; }
        public virtual DbSet<HSCV_CAPNHATTIENDO_CV> HSCV_CAPNHATTIENDO_CV { get; set; }
        public virtual DbSet<HSCV_CONGVIEC> HSCV_CONGVIEC { get; set; }
        public virtual DbSet<HSCV_CONGVIEC_LAPKEHOACH> HSCV_CONGVIEC_LAPKEHOACH { get; set; }
        public virtual DbSet<HSCV_CONGVIEC_NGUOITHAMGIAXULY> HSCV_CONGVIEC_NGUOITHAMGIAXULY { get; set; }
        public virtual DbSet<HSCV_CONGVIEC_NOIDUNGTRAODOI> HSCV_CONGVIEC_NOIDUNGTRAODOI { get; set; }
        public virtual DbSet<HSCV_CONGVIEC_XINLUIHAN> HSCV_CONGVIEC_XINLUIHAN { get; set; }
        public virtual DbSet<HSCV_READVANBAN> HSCV_READVANBAN { get; set; }
        public virtual DbSet<HSCV_SUBTASK> HSCV_SUBTASK { get; set; }
        public virtual DbSet<HSCV_TRINHDUYETCONGVIEC> HSCV_TRINHDUYETCONGVIEC { get; set; }
        public virtual DbSet<HSCV_TRINHDUYETKEHOACH> HSCV_TRINHDUYETKEHOACH { get; set; }
        public virtual DbSet<HSCV_VANBANDEN> HSCV_VANBANDEN { get; set; }
        public virtual DbSet<HSCV_VANBANDEN_DONVINHAN> HSCV_VANBANDEN_DONVINHAN { get; set; }
        public virtual DbSet<HSCV_VANBANDI> HSCV_VANBANDI { get; set; }
        public virtual DbSet<HSCV_VANBANDI_DONVINHAN> HSCV_VANBANDI_DONVINHAN { get; set; }
        public virtual DbSet<HSCV_VANBANDI_TRAODOI> HSCV_VANBANDI_TRAODOI { get; set; }
        public virtual DbSet<HUYEN> HUYEN { get; set; }
        public virtual DbSet<LICHCONGTAC> LICHCONGTAC { get; set; }
        public virtual DbSet<LOAITAILIEU_THUOCTINH> LOAITAILIEU_THUOCTINH { get; set; }
        public virtual DbSet<LOGSMS> LOGSMS { get; set; }
        public virtual DbSet<NGUOIDUNG_VAITRO> NGUOIDUNG_VAITRO { get; set; }
        public virtual DbSet<PHIEUDANHGIACONGVIEC> PHIEUDANHGIACONGVIEC { get; set; }
        public virtual DbSet<QL_DANGKY_XE> QL_DANGKY_XE { get; set; }
        public virtual DbSet<QL_DANGKYXE_LAIXE> QL_DANGKYXE_LAIXE { get; set; }
        public virtual DbSet<QL_LAIXE> QL_LAIXE { get; set; }
        public virtual DbSet<QL_NGUOINHAN_VANBAN> QL_NGUOINHAN_VANBAN { get; set; }
        public virtual DbSet<QL_PHONGHOP> QL_PHONGHOP { get; set; }
        public virtual DbSet<QL_TOKEN> QL_TOKEN { get; set; }
        public virtual DbSet<QL_XE> QL_XE { get; set; }
        public virtual DbSet<QUANLY_HOSO> QUANLY_HOSO { get; set; }
        public virtual DbSet<QUANLY_HOSO_NGUOINHAP> QUANLY_HOSO_NGUOINHAP { get; set; }
        public virtual DbSet<QUANLY_PHONGHOP> QUANLY_PHONGHOP { get; set; }
        public virtual DbSet<QUANLY_VANBAN> QUANLY_VANBAN { get; set; }
        public virtual DbSet<SYS_TINNHAN> SYS_TINNHAN { get; set; }
        public virtual DbSet<TAILIEU_THUOCTINH> TAILIEU_THUOCTINH { get; set; }
        public virtual DbSet<TAILIEUDINHKEM> TAILIEUDINHKEM { get; set; }
        public virtual DbSet<TAILIEUDINHKEM_VERSION> TAILIEUDINHKEM_VERSION { get; set; }
        public virtual DbSet<THONGBAO> THONGBAO { get; set; }
        public virtual DbSet<THUMUC_LUUTRU> THUMUC_LUUTRU { get; set; }
        public virtual DbSet<TINH> TINH { get; set; }
        public virtual DbSet<VAITRO_THAOTAC> VAITRO_THAOTAC { get; set; }
        public virtual DbSet<WF_FUNCTION> WF_FUNCTION { get; set; }
        public virtual DbSet<WF_FUNCTION_DONE> WF_FUNCTION_DONE { get; set; }
        public virtual DbSet<WF_ITEM_USER_PROCESS> WF_ITEM_USER_PROCESS { get; set; }
        public virtual DbSet<WF_LOG> WF_LOG { get; set; }
        public virtual DbSet<WF_MODULE> WF_MODULE { get; set; }
        public virtual DbSet<WF_PROCESS> WF_PROCESS { get; set; }
        public virtual DbSet<WF_REVIEW> WF_REVIEW { get; set; }
        public virtual DbSet<WF_STATE> WF_STATE { get; set; }
        public virtual DbSet<WF_STATE_FUNCTION> WF_STATE_FUNCTION { get; set; }
        public virtual DbSet<WF_STEP> WF_STEP { get; set; }
        public virtual DbSet<WF_STEP_CONFIG> WF_STEP_CONFIG { get; set; }
        public virtual DbSet<WF_STEP_USER_PROCESS> WF_STEP_USER_PROCESS { get; set; }
        public virtual DbSet<WF_STREAM> WF_STREAM { get; set; }
        public virtual DbSet<WF_USER_REVIEW> WF_USER_REVIEW { get; set; }
        public virtual DbSet<XA> XA { get; set; }
    }
}
