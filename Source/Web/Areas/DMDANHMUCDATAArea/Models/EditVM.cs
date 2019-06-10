using System.Collections.Generic;
using System.Web.Mvc;
using Business.CommonModel.DMDANHMUCDATA;
using Model.Entities;
namespace Web.Areas.DMDANHMUCDATAArea.Models
{
    public class EditVM
    {
        public DM_DANHMUC_DATA objModel { get; set; }
        public DM_DANHMUC_DATA_BO objBOModel { get; set; }
        public string UrlNavigation { get; set; }
        public List<SelectListItem> LstDept { get; set; }
    }
}
