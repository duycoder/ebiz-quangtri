using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.CommonModel.WFSTREAM;

namespace Web.Models
{
    public class VanBanChartVM
    {
        public List<STATISTICVANBANBO> LstData { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}