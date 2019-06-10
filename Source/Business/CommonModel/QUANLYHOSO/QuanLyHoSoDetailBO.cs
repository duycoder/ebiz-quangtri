using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;

namespace Business.CommonModel.QUANLYHOSO
{
    public class QuanLyHoSoDetailBO:QUANLY_HOSO
    {
        public string TEN_KHO { get; set; }
        public string TEN_PHONG { get; set; }
        public string TEN_MUCDO_TRUYCAP { get; set; }
        public string THOIHAN { get; set; }
    }

    public class QuanLyHoSoShortBO
    {
        public string HOSO_SO { get; set; }
        public string TIEUDE { get; set; }
    }
}
