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
    
    public partial class CHAT_NOIDUNG
    {
        public long ID { get; set; }
        public Nullable<long> NGUOIGUI_ID { get; set; }
        public Nullable<long> NGUOINHAN_ID { get; set; }
        public Nullable<long> GROUPCHAT_ID { get; set; }
        public string FROMUSER { get; set; }
        public string TOUSER { get; set; }
        public string FROMFULLNAME { get; set; }
        public string TOFULLNAME { get; set; }
        public Nullable<int> COSO_ID { get; set; }
        public string NOIDUNG { get; set; }
        public Nullable<System.DateTime> NGAYGUI { get; set; }
        public Nullable<System.DateTime> NGAYSUA { get; set; }
        public Nullable<bool> IS_DELETE { get; set; }
        public Nullable<bool> IS_NGUOINHAN_READ { get; set; }
    }
}
