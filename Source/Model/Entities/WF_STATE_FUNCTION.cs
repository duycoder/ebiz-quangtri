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
    
    public partial class WF_STATE_FUNCTION
    {
        public int ID { get; set; }
        public Nullable<int> WF_STATE_ID { get; set; }
        public Nullable<int> ACTION { get; set; }
        public Nullable<int> PRE_FUNCTION { get; set; }
        public Nullable<bool> IS_BREAK { get; set; }
        public Nullable<System.DateTime> create_at { get; set; }
        public Nullable<long> create_by { get; set; }
        public Nullable<System.DateTime> edit_at { get; set; }
        public Nullable<long> edit_by { get; set; }
    }
}
