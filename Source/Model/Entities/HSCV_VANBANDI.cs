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
    
    public partial class HSCV_VANBANDI
    {
        public long ID { get; set; }
        public Nullable<int> DOKHAN_ID { get; set; }
        public Nullable<int> DOUUTIEN_ID { get; set; }
        public Nullable<int> LINHVUCVANBAN_ID { get; set; }
        public Nullable<int> LOAIVANBAN_ID { get; set; }
        public Nullable<int> DONVISOANTHAO_ID { get; set; }
        public Nullable<long> NGUOIKY_ID { get; set; }
        public string CHUCVU { get; set; }
        public string TRICHYEU { get; set; }
        public string NOIDUNG { get; set; }
        public Nullable<System.DateTime> NGAYVANBAN { get; set; }
        public Nullable<System.DateTime> NGAYCOHIEULUC { get; set; }
        public Nullable<System.DateTime> NGAYHETHIEULUC { get; set; }
        public string SOHIEU { get; set; }
        public Nullable<int> SOVANBAN_ID { get; set; }
        public string SOTHEOSO { get; set; }
        public Nullable<int> SOTO { get; set; }
        public Nullable<int> SOBANSAO { get; set; }
        public string YKIENCHIDAO { get; set; }
        public Nullable<System.DateTime> THOIHANXULY { get; set; }
        public Nullable<System.DateTime> THOIHANHOIBAO { get; set; }
        public string DONVINHAN_INTERNAL_ID { get; set; }
        public string DONVINHAN_EXTERNAL_ID { get; set; }
        public string DONVINHAN_NGOAIHETHONG { get; set; }
        public Nullable<int> SOTRANG { get; set; }
        public Nullable<int> LANBANHANH { get; set; }
        public Nullable<System.DateTime> NGAYBANHANH { get; set; }
        public Nullable<System.DateTime> CREATED_AT { get; set; }
        public Nullable<long> CREATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_AT { get; set; }
        public Nullable<long> UPDATED_BY { get; set; }
        public Nullable<bool> HAS_SIGNED { get; set; }
        public string VANBANDENIDS { get; set; }
        public string VANBANDIIDS { get; set; }
        public Nullable<int> DEPTID { get; set; }
        public Nullable<long> VANBANDEN_ID { get; set; }
        public string NOI_NHAN { get; set; }
        public string USER_RECEIVE_DIRECTLY { get; set; }
        public Nullable<bool> IS_INTERNAL { get; set; }
        public string MA_DANGKY { get; set; }
        public Nullable<int> LOAI_COQUAN_ID { get; set; }
        public Nullable<int> THONGTIN_LOAI_ID { get; set; }
        public Nullable<long> TACGIA_ID { get; set; }
        public Nullable<bool> CAN_SEND_SMS { get; set; }
    }
}
