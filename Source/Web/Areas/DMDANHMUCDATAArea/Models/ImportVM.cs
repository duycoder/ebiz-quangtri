using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Entities;

namespace Web.Areas.DMDANHMUCDATAArea.Models
{
    public class ImportVM
    {
        public DM_NHOMDANHMUC DanhMuc { get; set; }
        public string PathTemplate { get; set; }
    }
}