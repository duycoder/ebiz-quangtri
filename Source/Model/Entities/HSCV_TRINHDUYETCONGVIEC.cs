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
    
    public partial class HSCV_TRINHDUYETCONGVIEC
    {
        public long ID { get; set; }
        public Nullable<long> CONGVIEC_ID { get; set; }
        public string NOIDUNG_TRINHDUYET { get; set; }
        public Nullable<System.DateTime> CREATED_AT { get; set; }
        public Nullable<long> CREATED_BY { get; set; }
        public Nullable<System.DateTime> NGAYPHANHOI { get; set; }
        public Nullable<int> SONGAYCHOPHANHOI { get; set; }
        public string KETQUATRINHDUYET { get; set; }
        public Nullable<bool> PHEDUYETKETQUA { get; set; }
    }
}