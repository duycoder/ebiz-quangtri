using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.BaseBusiness;

namespace Business.CommonModel.DMCHUCNANG
{
    public class DM_CHUCNANG_SEARCHBO : SearchBaseBO
    {
        public string QR_MA { get; set; }
        public string QR_CHUCNANG { get; set; }
    }
}

