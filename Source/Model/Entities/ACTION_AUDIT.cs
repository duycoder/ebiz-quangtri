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
    
    public partial class ACTION_AUDIT
    {
        public long ACTION_AUDIT_ID { get; set; }
        public string USER_NAME { get; set; }
        public string CONTROLLER { get; set; }
        public string ACTION { get; set; }
        public string PARAMETER { get; set; }
        public Nullable<System.DateTime> BEGIN_AUDIT_TIME { get; set; }
        public Nullable<System.DateTime> END_AUDIT_TIME { get; set; }
        public string IP { get; set; }
        public string REFERER { get; set; }
        public string USER_AGENT { get; set; }
        public Nullable<int> OBJECT_ID { get; set; }
        public string OLD_OBJECT_VALUE { get; set; }
        public string NEW_OBJECT_VALUE { get; set; }
        public string DESCRIPTION { get; set; }
    }
}
