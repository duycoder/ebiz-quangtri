using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.LOGSMS
{
    public class LOGSMS_BO: Model.Entities.LOGSMS
    {
        public string TEN_DONVI_NHAN { set; get; }
        public string TEN_DONVI_GUI { set; get; }
    }
}
