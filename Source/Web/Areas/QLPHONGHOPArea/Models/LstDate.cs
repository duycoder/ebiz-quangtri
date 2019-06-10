using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Entities;

namespace Web.Areas.QLPHONGHOPArea.Models
{
    public class LstDate
    {
        public List<QUANLY_PHONGHOP> LstMonday { get; set; }
        public List<QUANLY_PHONGHOP> LstTuesday { get; set; }
        public List<QUANLY_PHONGHOP> LstWednesday { get; set; }
        public List<QUANLY_PHONGHOP> LstThursday { get; set; }
        public List<QUANLY_PHONGHOP> LstFriday { get; set; }
        public List<QUANLY_PHONGHOP> LstSaturday { get; set; }
        public List<QUANLY_PHONGHOP> LstSunday { get; set; }
        public DateTime Monday { get; set; }
        public DateTime Tuesday { get; set; }
        public DateTime Wednesday { get; set; }
        public DateTime Thursday { get; set; }
        public DateTime Friday { get; set; }
        public DateTime Saturday { get; set; }
        public DateTime Sunday { get; set; }
        public List<DM_DANHMUC_DATA> LstPhongHop { get; set; }
    }
}