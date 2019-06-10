using System.Collections.Generic;
using Business.CommonBusiness;

namespace Web.Areas.QuanLyCongViec.Models
{
    public class ReportModel
    {
        
        public List<CongViecBO> ListViecChuaHoanThanhByDonVi { get; set; }
        public List<CongViecBO> ListViecQuaHanByDonVi { get; set; }

        public List<CongViecBO> LstCongViec { get; set; }
        public long CongViecId { get; set; }
    }
}