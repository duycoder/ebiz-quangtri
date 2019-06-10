using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.QLCHUYEN
{
    public class ChuyenReportBO
    {
        public long CANBO_ID { set; get; }
        public string TEN_CANBO { set; get; }
        public DateTime? NGAY_XUATPHAT { set; get; }
        public int? GIO_XUATPHAT { set; get; }
        public int? PHUT_XUATPHAT { set; get; }
        public string TEN_CHUYEN { set; get; }
        public decimal? TONG_CHIPHI { set; get; }
        public double? QUANGDUONG_DICHUYEN { set; get; }
        public string DIEM_XUATPHAT { set; get; }
        public string DIEM_KETTHUC { set; get; }

        public List<ChuyenBO> groupOfTrips { set; get; }
    }
}
