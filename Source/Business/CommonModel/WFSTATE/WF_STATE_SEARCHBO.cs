using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.BaseBusiness;

namespace Business.CommonModel.WFSTATE
{
    public class WF_STATE_SEARCHBO : SearchBaseBO
    {
        public string QR_STATE_NAME { get; set; }
        public string QR_GHICHU { get; set; }
    }
}

