using Business.CommonModel.DMNguoiDung;
using Business.CommonModel.WFLOG;
using Business.CommonModel.WFSTEP;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.WFSTREAMArea.Models
{
    public class FlowBO
    {
        public WF_PROCESS Process { get; set; }
        public WF_STEP_BO Step { get; set; }
        public List<NguoiDungPhongBanBO> dsNgNhanChinh { get; set; }
        public List<NguoiDungPhongBanBO> dsNgThamGia { get; set; }

        public bool IsBack { get; set; }
        public WF_LOG_BO Log { get; set; }
    }
}