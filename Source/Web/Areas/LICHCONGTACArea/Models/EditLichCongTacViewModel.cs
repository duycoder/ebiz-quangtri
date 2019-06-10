using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonHelper.DateExtend;
namespace Web.Areas.LICHCONGTACArea.Models
{
    public class EditLichCongTacViewModel
    {
        public List<SelectListItem> groupOfNguoiChuTris { set; get; }
        public List<SelectListItem> groupOfLanhDaos { set; get; }
        public List<SelectListItem> groupHours { set; get; }
        public List<SelectListItem> groupMinutes { set; get; }
        public List<string> groupOfDestinations { set; get; }
        public LICHCONGTAC entityLichCongTac { set; get; }
        public bool isPopUp { set; get; }

        public EditLichCongTacViewModel()
        {
            entityLichCongTac = new LICHCONGTAC();
            entityLichCongTac.NGAY_CONGTAC = DateTime.Now;

            this.groupHours = Utility.GetHours();
            this.groupMinutes = Utility.GetMinutes(0, 5);
        }

        public EditLichCongTacViewModel(LICHCONGTAC model)
        {
            this.entityLichCongTac = model;
            this.groupHours = Utility.GetHours(model.GIO_CONGTAC);
            this.groupMinutes = Utility.GetMinutes(model.PHUT_CONGTAC, 5);
        }
    }
}