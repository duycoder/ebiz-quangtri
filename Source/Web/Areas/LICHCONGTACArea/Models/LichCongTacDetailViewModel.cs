using Business.CommonModel.LICH_CONGTAC;
using Business.CommonModel.QLDANGKYXE;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.LICHCONGTACArea.Models
{
    public class LichCongTacDetailViewModel
    {
        public LICHCONGTAC_BO calendarEntity { set; get; }
        public QL_DANGKY_XE carRegistrationEntity { set; get; }
        public bool isShowCarRegistration { set; get; }
    }
}