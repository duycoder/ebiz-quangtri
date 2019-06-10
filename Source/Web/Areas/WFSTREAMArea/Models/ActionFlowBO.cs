using Business.CommonModel.WFSTEP;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.WFSTREAMArea.Models
{
    public class ActionFlowBO
    {
        public List<WF_STEP> LstStep { get; set; }
        public List<StepBackBO> LstStepBack { get; set; }
        public WF_PROCESS Process { get; set; }
        public WF_STATE StartState { get; set; }
        public WF_STATE EndState { get; set; }
        public WF_FUNCTION Function { get; set; }
        public bool REQUIRED_REVIEW { get; set; }
        public WF_REVIEW ReviewObj { get; set; }
    }
}