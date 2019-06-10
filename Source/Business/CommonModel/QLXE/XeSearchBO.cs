using Business.BaseBusiness;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.QLXE
{
    public class XeSearchBO : SearchBaseBO
    {
        public int? CCTC_THANHPHAN_ID { set; get; }
        public string TENXE { set; get; }
        public string BIENSO { set; get; }
        public int? querySoChoStart { set; get; }
        public int? querySoChoEnd { set; get; }
    }
}
