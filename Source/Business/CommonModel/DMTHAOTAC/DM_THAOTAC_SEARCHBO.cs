using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.BaseBusiness;

namespace Business.CommonModel.DMTHAOTAC
{
    public class DM_THAOTAC_SEARCHBO : SearchBaseBO
    {
        public string QR_MA { get; set; }
        public string QR_THAOTAC { get; set; }
        public int QR_CHUCNANGID { get; set; }
    }
}

