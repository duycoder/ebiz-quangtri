using Business.BaseBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.HSCVCONGVIEC
{
    public class HSCV_CONGVIEC_SEARCH : SearchBaseBO
    {
        public long USER_ID { get; set; }
        public string TENCONGVIEC { get; set; }
        public DateTime? NGAYBATDAU_FROM { get; set; }
        public DateTime? NGAYBATDAU_TO { get; set; }
        public DateTime? NGAYKETTHUC_FROM { get; set; }
        public DateTime? NGAYKETTHUC_TO { get; set; }
        public long? DO_UUTIEN { get; set; }
        public long? DOKHAN { get; set; }
        public int? NOTIF_TYPE { get; set; }
        public bool? IS_HASPLAN { get; set; }
        public int LOAI_CONGVIEC { get; set; }
    }
}
