using Business.CommonModel.WFSTEP;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.HSCVVANBANDI
{
    public class HSCV_VANBANDI_BO : HSCV_VANBANDI
    {
        public string TENSOVANBANDI { get; set; }
        public string TEN_LOAIVANBAN { get; set; }
        public string TEN_LINHVUC { get; set; }
        public string TEN_DOKHAN { get; set; }
        public string TEN_DOUUTIEN { get; set; }
        public string TEN_NGUOIKY { get; set; }
        public string TENSOVANBAN { get; set; }
        public string COLOR { get; set; }
        public List<StepBackBO> LstStepBack { get; set; }
        public List<WF_STEP> LstStep { get; set; }
        public bool REQUIRED_REVIEW { get; set; }
        public WF_FUNCTION Function { get; set; }
        public string ICON_LOAIVANBAN { get; set; }
        public string ICON_LINHVUC { get; set; }
        public string ICON_DOKHAN { get; set; }
        public string ICON_DOUUTIEN { get; set; }
        public long PROCESS_USER_ID { get; set; }
        public int? CURRENT_STATE { get; set; }
        public DateTime? Update_At { get; set; }
    }
}
