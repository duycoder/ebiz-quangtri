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
    
    public partial class LOGSMS
    {
        public long ID { get; set; }
        public string SODIENTHOAINHAN { get; set; }
        public string NOIDUNG { get; set; }
        public string KETQUA { get; set; }
        public Nullable<System.DateTime> CREATED_AT { get; set; }
        public Nullable<long> CREATED_BY { get; set; }
        public Nullable<int> SOKYTU { get; set; }
        public string HOTENNGUOINHAN { get; set; }
        public string HOTENNGUOIGUI { get; set; }
        public string ITEMTYPE { get; set; }
        public Nullable<long> ITEMID { get; set; }
        public Nullable<int> DONVI_GUI { get; set; }
        public Nullable<int> DONVI_NHAN { get; set; }
    }
}
