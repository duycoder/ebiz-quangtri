using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.BaseBusiness;
namespace Business.CommonModel.HOSOCANBO
{
    public class HoSoCanBoSearch :SearchBaseBO
    {
        public string HOTEN { get; set; }
        public string MANGACH_BAC { get; set; }
        public string STRCHUCVU { get; set; }
        public string TENDANGNHAP { get; set; }

    }
}
