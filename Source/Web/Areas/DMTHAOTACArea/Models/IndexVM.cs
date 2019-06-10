using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Entities;
using Business.CommonBusiness;
using Business.CommonModel.DMTHAOTAC;



namespace Web.Areas.DMTHAOTACArea.Models
{
    public class IndexVM
    {
        public DM_CHUCNANG ChucNang { get; set; }
        public PageListResultBO<DM_THAOTAC_BO> ListThaoTac { get; set; }
    }
}