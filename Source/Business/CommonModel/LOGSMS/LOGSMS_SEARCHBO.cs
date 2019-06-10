using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
using Business.BaseBusiness;

namespace Business.CommonModel.LOGSMS
{
    public class LOGSMS_SEARCHBO : SearchBaseBO
    {
        public string NguoiNhan { get; set; }
        public string NguoiGui { get; set; }
        public string SoDienThoai { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }

        public string DonViGui { set; get; }
        public string DonViNhan { set; get; }
    }
}
