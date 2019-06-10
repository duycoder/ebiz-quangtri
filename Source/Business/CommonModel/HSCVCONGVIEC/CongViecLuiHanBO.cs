using Model.Entities;
using System;
using System.Collections.Generic;

namespace Business.CommonBusiness
{
    public class CongViecLuiHanBO
    {
        public long? CONGVIEC_ID { get; set; }
        public string TENCONGVIEC { get; set; }
        public DateTime? HANKETHUC { get; set; }
        public DateTime? HANKETHUCTRUOC { get; set; }
        public DateTime? NGAYPHEDUYET { get; set; }
        public DateTime? HANKETTHUC_LANHDAODUYET { get; set; }
        public DateTime? NGAYGUI { get; set; }
        public bool? HAS_FILE { get; set; }
        public long ID { get; set; }
        public bool? IS_APPROVED { get; set; }
        public bool? IS_SENDREQUEST { get; set; }
        public DateTime? NGAYTAO { get; set; }
        public string NOIDUNG { get; set; }
        public string TIEUDE { get; set; }
        public long? USER_ID { get; set; }
        public string FullName { get; set; }
        public long? NGUOIGIAOVIEC_ID { get; set; }
        public int? COSO_ID { get; set; }
        public List<TAILIEUDINHKEM> TaiLieuDinhKem { get; set; }
        public string BUTPHELANHDAO { get; set; }
    }
}
