using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.QLCHUYEN
{
    public class ChuyenBO : QL_DANGKYXE_LAIXE
    {
        public long CANBO_ID { set; get; }
        public DateTime? NGAY_XUATPHAT { set; get; }
        public int? GIO_XUATPHAT { set; get; }
        public int? PHUT_XUATPHAT { set; get; }
        public string THOIGIAN_XUATPHAT { set; get; }
        public string DIEM_XUATPHAT { set; get; }
        public string DIEM_KETTHUC { set; get; }

        public string TENXE { set; get; }
        public string TEN_LOAIXE { set; get; }

        public string MUCDICH { set; get; }
        public string NOIDUNG { set; get; }
        
        public string TEN_LAIXE { set; get; }
        public string DIENTHOAI_LAIXE { set; get; }

        public string TEN_LOAICHUYEN { set; get; }

        public string TEN_TRANGTHAI { set; get; }
        public string MAU_TRANGTHAI { set; get; }

        public string TEN_BENHNHAN { set; get; }
        public string TEN_CANBO { set; get; }
        public long? LICHCONGTAC_ID { set; get; }
        public string TEN_LICHCONGTAC { set; get; }
    }
}
