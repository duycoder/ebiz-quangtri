using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model.Entities;
using Business.CommonModel.QLPHONGHOP;
using Business.CommonModel.DMCHUCNANG;
using Business.CommonModel.CONSTANT;

namespace Web.Areas.QLPHONGHOPArea.Models
{
    public class PhongHopViewModel
    {
        public string title { set; get; }
        public List<SelectListItem> groupOfLeaders { set; get; }
        public List<SelectListItem> groupOfYears { set; get; }
        public List<SelectListItem> groupOfWeeks { set; get; }
        public List<DateTime> groupOfDates { set; get; } //hiển thị thông tin các ngày trong tuần
        public List<PhongHopEntity> groupOfRooms { set; get; }
        public List<LichCongTacEntityViewModel> groupOfCalendars { set; get; }
    }

    public class PhongHopEntity
    {
        public string name { set; get; } //tên phòng họp
        public List<LichCongTacEntityViewModel> groupOfCalendars { set; get; }
    }


    public class LichHopViewModel
    {
        /// <summary>
        /// @author: duynn
        /// @since: 15/09/2018
        /// </summary>
        public int calendarType { set; get; } //loại hiển thị lịch họp
        public List<SelectListItem> groupOfLeaders { set; get; } //danh sách lãnh đạo
        public List<SelectListItem> groupOfWeeks { set; get; } //danh sách các tuần trong năm
        public LichHopViewModel()
        {
            this.calendarType = LICH_CONSTANT.THANG;
        }
    }

    public class LichHopByMonthViewModel
    {
        public long currentUserId { set; get; }
        public int month { set; get; }
        public int year { set; get; }
        public int day { set; get; }
        public string title { set; get; }
        public List<SelectListItem> groupOfYears { set; get; }
        public List<SelectListItem> groupOfMonths { set; get; }
        public List<LichHopByWeekViewModel> groupOfWeeks { set; get; }
    }

    public class LichHopByWeekViewModel
    {
        public long currentUserId { set; get; }
        public string title { set; get; }
        public List<SelectListItem> groupOfYears { set; get; }
        public List<SelectListItem> groupOfWeeks { set; get; }
        public List<DateTime> groupOfDates { set; get; } //hiển thị thông tin các ngày trong tuần
        public List<LichHopByHourViewModel> groupOfHours { set; get; }
        public List<LichCongTacEntityViewModel> groupOfDays { set; get; }
    }

    public class LichHopByHourViewModel
    {
        public string title { set; get; }
        public List<LichCongTacEntityViewModel> groupOfEntities { set; get; }
    }

    public class LichHopByDayViewModel
    {
        public long currentUserId { set; get; }
        public string title { set; get; }
        public DateTime? startDate { set; get; }
        public DateTime? endDate { set; get; }
        public List<LichCongTacEntityViewModel> groupOfEntities { set; get; }
    }
    public class LichCongTacEntityViewModel
    {
        public DateTime entityDay { set; get; } // để tạo lịch nhanh từ danh sách lịch
        public bool isToday { set; get; }
        public string title { set; get; }
        public List<QLPHONGHOP_BO> groupOfCalendars { set; get; }

        /*==================*/
        public List<QLPHONGHOP_BO> groupMorningItems { set; get; } //danh sách lịch họp buổi sáng
        public List<QLPHONGHOP_BO> groupAfternoonItems { set; get; } //danh sách lịch họp buổi chiều
    }
}