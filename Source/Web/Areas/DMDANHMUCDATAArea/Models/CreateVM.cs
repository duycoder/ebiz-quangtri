using System.Collections.Generic;
using System.Web.Mvc;
using Model.Entities;
namespace Web.Areas.DMDANHMUCDATAArea.Models
{
    public class CreateVM
    {
        public DM_DANHMUC_DATA objModel { get; set; }
        public DM_NHOMDANHMUC DanhMuc { get; set; }
        public List<SelectListItem> LstDept { get; set; }
    }
}
