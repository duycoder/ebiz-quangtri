using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.BaseBusiness;

namespace Business.CommonModel.DMNHOMDANHMUC
{
    public class DM_NHOMDANHMUC_SEARCHBO : SearchBaseBO
    {
        public string QR_CODE { get; set; }
        public string QR_NAME { get; set; }
    }
}

