using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.BaseBusiness;

namespace Business.CommonModel.DMVAITRO
{
    public class DM_VAITRO_SEARCHBO : SearchBaseBO
    {
        public string QR_MA { get; set; }
        public string QR_VAITRO { get; set; }
    }
}

