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
    
    public partial class WF_STEP
    {
        public int ID { get; set; }
        public Nullable<int> WF_ID { get; set; }
        public string NAME { get; set; }
        public string GHICHU { get; set; }
        public Nullable<int> STATE_BEGIN { get; set; }
        public Nullable<int> STATE_END { get; set; }
        public string ICON { get; set; }
        public Nullable<bool> IS_RETURN { get; set; }
        public Nullable<System.DateTime> create_at { get; set; }
        public Nullable<long> create_by { get; set; }
        public Nullable<System.DateTime> edit_at { get; set; }
        public Nullable<long> edit_by { get; set; }
        public Nullable<bool> REQUIRED_REVIEW { get; set; }
    }
}
