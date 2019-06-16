using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.BaseBusiness;

namespace Business.CommonModel.CCTCTHANHPHAN
{
    public class CCTC_THANHPHAN_SEARCHBO : SearchBaseBO
    {
        public string QR_MAPHONGBAN { get; set; }
        public string QR_TENPHONGBAN { get; set; }
        public int? QR_LOAIPHONGBAN { get; set; }
        public int? QR_CAPPHONGBAN { get; set; }
    }
}

