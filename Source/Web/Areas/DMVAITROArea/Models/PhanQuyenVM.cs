using Business.CommonModel.DMCHUCNANG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Entities;
using Business.CommonModel.DMVAITRO;
using System.Web.Mvc;
namespace Web.Areas.DMVAITROArea.Models
{
    public class PhanQuyenVM
    {
        public List<DM_VAITRO_BO> ListUserInRole { get; set; }
        public List<DM_CHUCNANG_BO> ListAllChucNang { get; set; }
        public List<DM_CHUCNANG_BO> ListChucNangVaiTro { get; set; }
        public DM_VAITRO VaiTro { get; set; }
        public List<SelectListItem> ListUserNotInRole { get; set; }
        public int VaiTroID { get; set; }
    }
}