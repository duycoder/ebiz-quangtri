using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.BaseBusiness;

namespace Business.CommonModel.WFSTEP
{
    public class WF_STEP_SEARCHBO : SearchBaseBO
    {
       
        public int? QR_WF_ID { get; set; }
        public int? QR_STATE_BEGIN { get; set; }
        public int? QR_STATE_END { get; set; }
      
        public bool QR_IS_RETURN { get; set; }
       
        public string QR_NAME { get; set; }
        public string QR_GHICHU { get; set; }
     
    }
}

