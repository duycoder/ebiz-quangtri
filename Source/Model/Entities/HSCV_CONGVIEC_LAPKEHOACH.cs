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
    
    public partial class HSCV_CONGVIEC_LAPKEHOACH
    {
        public long ID { get; set; }
        public long CONGVIEC_ID { get; set; }
        public System.DateTime NGAYBATDAUTHEOKEHOACH { get; set; }
        public System.DateTime NGAYHOANTHANHTHEOKEHOACH { get; set; }
        public int SONGAYTRIENKHAITHEOKEHOACH { get; set; }
        public string CACBUOCTHUCHIEN { get; set; }
        public string MUCTIEUCONGVIEC { get; set; }
        public Nullable<System.DateTime> NGAYTRINHKEHOACH { get; set; }
        public Nullable<System.DateTime> NGAYCAPTRENPHANHOI { get; set; }
        public Nullable<int> SONGAYCHOPHANHOI { get; set; }
        public string KETQUATRINHDUYET { get; set; }
        public Nullable<bool> ISAPPROVE { get; set; }
        public Nullable<System.DateTime> CREATED_AT { get; set; }
        public Nullable<long> CREATED_BY { get; set; }
    }
}
