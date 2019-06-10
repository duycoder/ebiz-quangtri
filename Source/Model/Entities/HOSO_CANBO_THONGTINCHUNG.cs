//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class HOSO_CANBO_THONGTINCHUNG
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HOSO_CANBO_THONGTINCHUNG()
        {
            this.HOSO_CANBO_CONGTAC = new HashSet<HOSO_CANBO_CONGTAC>();
            this.HOSO_CANBO_DAOTAO = new HashSet<HOSO_CANBO_DAOTAO>();
            this.HOSO_CANBO_QUANHEBANTHAN = new HashSet<HOSO_CANBO_QUANHEBANTHAN>();
            this.HOSO_CANBO_QUANHEKETHON = new HashSet<HOSO_CANBO_QUANHEKETHON>();
            this.HOSO_CANBO_QUATRINH_LUONG = new HashSet<HOSO_CANBO_QUATRINH_LUONG>();
        }
    
        public long ID { get; set; }
        public string HOTEN { get; set; }
        public string TENGOIKHAC { get; set; }
        public Nullable<System.DateTime> NGAYSINH { get; set; }
        public Nullable<int> GIOITINH { get; set; }
        public Nullable<int> NOISINH_XA { get; set; }
        public Nullable<int> NOISINH_HUYEN { get; set; }
        public Nullable<int> NOISINH_TINH { get; set; }
        public Nullable<int> QUEQUAN_XA { get; set; }
        public Nullable<int> QUEQUAN_HUYEN { get; set; }
        public Nullable<int> QUEQUAN_TINH { get; set; }
        public Nullable<int> DANTOC { get; set; }
        public Nullable<int> TONGIAO { get; set; }
        public string NOIDANGKYHOKHAUTHUONGTRU { get; set; }
        public string NOIOHIENNAY { get; set; }
        public string NGHENGHIEPKHIDUOCTUYENDUNG { get; set; }
        public Nullable<System.DateTime> NGAYTRUNGTUYEN { get; set; }
        public string COQUANTUYENDUNG { get; set; }
        public Nullable<int> CHUCVUHIENTAI { get; set; }
        public string CONGVIECCHINHDUOCGIAO { get; set; }
        public Nullable<int> NGACHCONGCHUCVIENCHUC { get; set; }
        public string MANGACH { get; set; }
        public string BACLUONG { get; set; }
        public string HESO { get; set; }
        public Nullable<System.DateTime> NGAYHUONG { get; set; }
        public string PHUCAPCHUCVU { get; set; }
        public string PHUCAPKHAC { get; set; }
        public Nullable<int> TRINHDOGIAODUC { get; set; }
        public Nullable<int> TRINHDOCHUYENMONCAONHAT { get; set; }
        public Nullable<int> LYLUANCHINHTRI { get; set; }
        public Nullable<int> QUANLYNHANUOC { get; set; }
        public Nullable<int> NGOAINGU { get; set; }
        public Nullable<int> TINHOC { get; set; }
        public Nullable<System.DateTime> NGAYVAODANG { get; set; }
        public Nullable<System.DateTime> NGAYCHINHTHUC { get; set; }
        public Nullable<System.DateTime> NGAYTHAMGIATCCTXH { get; set; }
        public Nullable<System.DateTime> NGAYNHAPNGU { get; set; }
        public Nullable<System.DateTime> NGAYXUATNGU { get; set; }
        public Nullable<int> QUANHAMCAONHAT { get; set; }
        public string DANHHIEUDUOCPHONG { get; set; }
        public string SOTRUONGCONGTAC { get; set; }
        public string KHENTHUONG { get; set; }
        public string KYLUAT { get; set; }
        public Nullable<int> SUCKHOE { get; set; }
        public string CHIEUCAO { get; set; }
        public string CANNANG { get; set; }
        public Nullable<int> NHOMMAU { get; set; }
        public string THUONGBINHHANG { get; set; }
        public Nullable<int> GIADINHCHINHSACH { get; set; }
        public string CMTND { get; set; }
        public Nullable<System.DateTime> NGAYCAP { get; set; }
        public string NOICAP { get; set; }
        public string SOSOBHXH { get; set; }
        public string DACDIEMLICHSU_BIBAT { get; set; }
        public string DACDIEMLICHSU_THAMGIA { get; set; }
        public string DACDIEMLICHSU_THANNHAN { get; set; }
        public string NHANXET { get; set; }
        public Nullable<long> USER_ID { get; set; }
        public Nullable<int> DONVI_ID { get; set; }
        public string AVATAR { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HOSO_CANBO_CONGTAC> HOSO_CANBO_CONGTAC { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HOSO_CANBO_DAOTAO> HOSO_CANBO_DAOTAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HOSO_CANBO_QUANHEBANTHAN> HOSO_CANBO_QUANHEBANTHAN { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HOSO_CANBO_QUANHEKETHON> HOSO_CANBO_QUANHEKETHON { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HOSO_CANBO_QUATRINH_LUONG> HOSO_CANBO_QUATRINH_LUONG { get; set; }
    }
}