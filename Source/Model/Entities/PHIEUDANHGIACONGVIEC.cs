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
    
    public partial class PHIEUDANHGIACONGVIEC
    {
        public long ID { get; set; }
        public long CONGVIEC_ID { get; set; }
        public Nullable<int> TDG_TUCHUCAO { get; set; }
        public Nullable<int> TDG_TRACHNHIEMLON { get; set; }
        public Nullable<int> TDG_TUONGTACTOT { get; set; }
        public Nullable<int> TDG_TOCDONHANH { get; set; }
        public Nullable<int> TDG_TIENBONHIEU { get; set; }
        public Nullable<int> TDG_THANHTICHVUOT { get; set; }
        public Nullable<int> DD_TUCHUCAO { get; set; }
        public Nullable<int> DD_TRACHNHIEMLON { get; set; }
        public Nullable<int> DD_TUONGTACTOT { get; set; }
        public Nullable<int> DD_TOCDONHANH { get; set; }
        public Nullable<int> DD_TIENBONHIEU { get; set; }
        public Nullable<int> DD_THANHTICHVUOT { get; set; }
        public Nullable<long> NGUOITUDANHGIA { get; set; }
        public Nullable<long> NGUOIDUYET { get; set; }
        public Nullable<System.DateTime> NGAYDUYET { get; set; }
        public Nullable<System.DateTime> NGAYDANHGIA { get; set; }
        public Nullable<int> TONGDIEM { get; set; }
        public string XEPLOAI { get; set; }
        public string KETLUAN { get; set; }
        public string XEPLOAIDUYET { get; set; }
        public Nullable<int> TDG_XEPLOAI_ID { get; set; }
        public Nullable<int> DD_XEPLOAI_ID { get; set; }
        public Nullable<int> TDG_TONGDIEM { get; set; }
        public string TDG_XEPLOAI { get; set; }
    }
}
