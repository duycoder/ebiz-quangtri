using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.QuanLyCongViec.Models
{
    public class ImportCongViecViewModel
    {
        public long taskId { set; get; }
        public string controllerName { set; get; }
        public string importTemplatePath { set; get; }
    }
}