using Model.Entities;
using System;
using System.Collections.Generic;

namespace Business.CommonBusiness
{
    public class CongViecNoiDungTraoDoiBO
    {
        public long? CONGVIEC_ID { get; set; }
        public bool? HAS_FILE { get; set; }
        public long ID { get; set; }
        public DateTime? NGAYTAO { get; set; }
        public string NOIDUNG { get; set; }
        public long? REPLY_ID { get; set; }
        public string TIEUDE { get; set; }
        public long? USER_ID { get; set; }
        public string NguoiDangNoiDung { get; set; }
        public List<TAILIEUDINHKEM> TaiLieuDinhKem { get; set; }
    }
}
