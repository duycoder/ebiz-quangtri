using Business.CommonModel.LICH_CONGTAC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.LICHCONGTACArea.Models
{
    public class LichCongTacViewModel
    {
        public bool isLeader { set; get; } //là lãnh đạo hay không?
        public int calendarType { set; get; }
        public List<string> groupOfDestinations { set; get; }
        public List<SelectListItem> groupOfLanhDaos { set; get; }
        public bool canCreate { set; get; }
    }

    public class LichCongTacByDayViewModel
    {
        public bool canCreate { set; get; }
        public long currentUserId { set; get; }
        public string title { set; get; }
        public DateTime? startDate { set; get; }
        public DateTime? endDate { set; get; }
        public List<SelectListItem> groupOfYears { set; get; }
        public List<LichCongTacDateEntity> groupDateEntities { set; get; }

    }

    public class LichCongTacByWeekViewModel
    {
        public bool canCreate { set; get; }
        public long currentUserId { set; get; }
        public string title { set; get; }
        public List<DateTime> groupOfDates { set; get; }
        public List<SelectListItem> groupOfYears { set; get; }
        public List<SelectListItem> groupOfWeeks { set; get; }
        public List<LichCongTacByWeekHourEntity> groupWeekHourEntities { set; get; }
    }

    public class LichCongTacByMonthViewModel
    {
        public bool canCreate { set; get; }
        public long currentUserId { set; get; }
        public int year { set; get; }
        public int month { set; get; }
        public string title { set; get; }
        public List<SelectListItem> groupOfYears { set; get; }
        public List<SelectListItem> groupOfMonths { set; get; }
        public List<LichCongTacByMonthWeekEntity> groupWeekEntities { set; get; }
    }

    public class LichCongTacByWeekHourEntity
    {
        public string title { set; get; }
        public int hour { set; get; }
        public List<LichCongTacDateEntity> groupDateEntities { set; get; }
    }

    public class LichCongTacByMonthWeekEntity
    {
        public List<LichCongTacDateEntity> groupDateEntities { set; get; }
    }

    public class LichCongTacDateEntity
    {
        public bool isToday { set; get; } //là ngày hôm nay
        public DateTime date { set; get; }
        public long currentUserId { set; get; }
        public string title { set; get; }
        public List<LICHCONGTAC_BO> groupOfCalendars { set; get; }
    }
}