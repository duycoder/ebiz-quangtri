using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;

namespace Business.CommonModel.WFSTATE
{
    public class WF_STATE_BO : WF_STATE
    {
        public int? FunctionID { get; set; }
        public string FunctionName { get; set; }
        public string ChucVu { get; set; }
        public string VaiTro { get; set; }
    }
}
