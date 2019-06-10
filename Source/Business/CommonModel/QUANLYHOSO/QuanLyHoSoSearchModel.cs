using Business.BaseBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.QUANLYHOSO
{
    public class QuanLyHoSoSearchModel:SearchBaseBO
    {
        public int? HoSoNam { get; set; }
        public int? NamChinhLy { get; set; }
        public int? KhoId { get; set; }
        public int? PhongId { get; set; }
        public string FTS { get; set; }
    }
}
