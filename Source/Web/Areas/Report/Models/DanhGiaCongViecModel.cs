using Business.CommonBusiness;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.Report.Models
{
    public class DanhGiaCongViecModel
    {
        public PHIEUDANHGIACONGVIEC PhieuDanhGia { get; set; }
        public CongViecBO CongViec { get; set; }            
    }
}