using Business.CommonModel.WFSTEP;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CommonModel.WFLOG
{
    public class WF_LOG_BO:WF_LOG
    {
        public string TenNguoiXuLy { get; set; }
        public string TenNguoiNhan { get; set; }
        public WF_STEP_BO step { get; set; }
        public List<string> LstThamGia { get; set; }
        public List<Model.Entities.TAILIEUDINHKEM> LstTaiLieuDinhKem { get; set; }
    }
}
