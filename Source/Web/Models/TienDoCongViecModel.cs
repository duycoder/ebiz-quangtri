using Business.CommonBusiness;

namespace Web.Models
{
    public class TienDoCongViecModel
    {
        public double TOT { get; set; }
        public double KHA { get; set; }
        public double DAT { get; set; }
        public double KHONGDAT { get; set; }
        public double DUNGHAN { get; set; }
        public double TREHAN { get; set; }
    }
    public class DanhSachCongViec
    {
        public int Type { get; set; }
        public PageListResultBO<CongViecBO> ListResult { get; set; }
    }
}