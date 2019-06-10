using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.QLXE
{
    public class XeBO : QL_XE
    {
        public string TEN_LOAIXE { set; get; }
        public int? TRANGTHAI { set; get; }
    }
}
