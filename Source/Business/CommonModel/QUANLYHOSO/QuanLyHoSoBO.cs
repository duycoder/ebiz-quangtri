using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;

namespace Business.CommonModel.QUANLYHOSO
{
    public class QuanLyHoSoBO:QUANLY_HOSO
    {
        public string TEN_NGUOITAO { get; set; }
        public string TEN_PHONG { get; set; }
        public string TEN_KHO { get; set; }
        public int CountVanBan { get; set; }
    }
}
