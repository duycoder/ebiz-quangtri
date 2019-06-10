using Business.CommonBusiness;
using Business.CommonModel.EFILECHIASE;
using Business.CommonModel.THUMUCLUUTRU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.THUMUCLUUTRUArea.Models
{
    public class FollowingModel
    {
        public PageListResultBO<THUMUC_LUUTRU_BO> ListChiaSe { get; set; }
    }
}