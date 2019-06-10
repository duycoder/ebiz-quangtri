using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonBusiness
{
    public class SubTaskBO
    {
        public long? CONGVIEC_ID { get; set; }
        public bool? DAGIAOVIEC { get; set; }
        public DateTime? HANHOANTHANH { get; set; }
        public long ID { get; set; }
        public int? MUCDOUUTIEN { get; set; }
        public DateTime? NGAYHOANTHANH { get; set; }
        public DateTime? NGAYTAO { get; set; }
        public string NOIDUNG { get; set; }
        public int? TRANGTHAI_ID { get; set; }
        public string NGUOITHUCHIEN { get; set; }
        public int? PHANTRAMHOANTHANH { get; set; }
        public long CONGVIEC_FOR_TASK { get; set; }
        public string DOKHAN_TEXT { get; set; }
        public string DOUUTIEN_TEXT { get; set; }
        public string TIEUDE_CONGVIEC { set; get; }
    }
}

