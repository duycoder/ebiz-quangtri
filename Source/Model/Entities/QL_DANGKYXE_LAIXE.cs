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
    
    public partial class QL_DANGKYXE_LAIXE
    {
        public long ID { get; set; }
        public string TEN_CHUYEN { get; set; }
        public Nullable<long> QL_DANGKY_XE_ID { get; set; }
        public Nullable<int> LAIXE_ID { get; set; }
        public Nullable<int> XE_ID { get; set; }
        public Nullable<int> TRANGTHAI { get; set; }
        public Nullable<int> LOAICHUYEN_ID { get; set; }
        public Nullable<bool> IS_TUVONG_TRENDUONG { get; set; }
        public Nullable<bool> IS_BHYT { get; set; }
        public string GHICHU { get; set; }
        public Nullable<System.DateTime> NGAYTAO { get; set; }
        public Nullable<System.DateTime> NGAYSUA { get; set; }
        public Nullable<long> NGUOITAO { get; set; }
        public Nullable<long> NGUOISUA { get; set; }
        public Nullable<int> CCTC_THANHPHAN_ID { get; set; }
        public Nullable<double> QUANGDUONG_DICHUYEN { get; set; }
        public Nullable<decimal> TONG_CHIPHI { get; set; }
    }
}