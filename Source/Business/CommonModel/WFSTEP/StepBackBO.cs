using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;

namespace Business.CommonModel.WFSTEP
{
    public class StepBackBO:WF_STEP
    {
        public WF_LOG Log { get; set; }
    }
}
