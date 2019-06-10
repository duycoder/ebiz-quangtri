using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;

namespace Business.CommonModel.WFSTEP
{
    public class WF_STEP_BO : WF_STEP
    {
        public string TrangThaiBatDau { get; set; }
        public string TrangThaiKetThuc { get; set; }
        public WF_STEP_CONFIG ConfigStep { get; set; }
        public WF_STEP_USER_PROCESS NguoiXuLyChinh { get; set; }
        public WF_STEP_USER_PROCESS NguoiThamGiaXuLy { get; set; }
    }
}
