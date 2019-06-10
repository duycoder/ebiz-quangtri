using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.BaseBusiness;

namespace Business.CommonModel.WFSTREAM
{
    public class WF_STREAM_SEARCHBO : SearchBaseBO
    {
        public string QR_WF_NAME { get; set; }
        public string QR_WF_DESCRIPTION { get; set; }
    }
}

