using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.CommonBusiness;
using Business.CommonModel.HOSOCANBO;
using Model.Entities;

namespace Web.Areas.HoSoCanBoArea.Models
{
    public class DanhSachHoSoVM
    {
        public PageListResultBO<HOSOCANBOCUSTOMBO> ListResult { get; set; }
    }
}