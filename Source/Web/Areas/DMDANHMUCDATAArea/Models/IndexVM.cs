using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.CommonBusiness;
using Business.CommonModel.DMDANHMUCDATA;
using Model.Entities;
namespace Web.Areas.DMDANHMUCDATAArea.Models
{
    public class IndexVM
    {
        public PageListResultBO<DM_DANHMUC_DATA_BO> lstData { get; set; }
        public DM_NHOMDANHMUC DanhMuc { get; set; }

    }
}