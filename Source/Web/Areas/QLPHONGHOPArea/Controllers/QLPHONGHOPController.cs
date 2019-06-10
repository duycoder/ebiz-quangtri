using Business.Business;
using Business.CommonBusiness;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Web.Areas.QLPHONGHOPArea.Models;
using Web.Custom;
using CommonHelper;
using Business.BaseBusiness;
using CommonHelper.DateExtend;
using Business.CommonModel.QLPHONGHOP;
using Web.Filter;
using Business.CommonModel.DMTHAOTAC;
using Business.CommonModel.CONSTANT;

namespace Web.Areas.QLPHONGHOPArea.Controllers
{
    public class QLPHONGHOPController : BaseController
    {
        // GET: QLPHONGHOPArea/QLPHONGHOP
        DM_NGUOIDUNGBusiness DM_NGUOIDUNGBusiness;
        QUANLY_PHONGHOPBusiness QUANLY_PHONGHOPBusiness;
        DM_THAOTACBusiness DM_THAOTACBusiness;
        VAITRO_THAOTACBusiness VAITRO_THAOTACBusiness;
        private SYS_TINNHANBusiness SYS_TINNHANBusiness;
        private DM_DANHMUC_DATABusiness DM_DANHMUC_DATABusiness;
        private QL_PHONGHOPBusiness QL_PHONGHOPBusiness;
        private string CODE_PHONGHOP = WebConfigurationManager.AppSettings["CODE_PHONGHOP"];

        [ActionAudit]
        [CodeAllowAccess(Code = "QUANLY_LICHHOP")]
        public ActionResult Index()
        {
            AssignUserInfo();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            LichHopViewModel viewModel = new LichHopViewModel();
            viewModel.groupOfLeaders = DM_NGUOIDUNGBusiness.GetDropDownByDeptParentId(currentUser.DeptParentID.GetValueOrDefault(), 0);
            return View(viewModel);
        }

        /// <summary>
        /// @author: duynn
        /// @description: tìm kiếm lịch đặt phòng
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [ActionAudit]
        public PartialViewResult SearchLichDatPhong(QLPHONGHOP_SEARCH searchModel)
        {
            AssignUserInfo();
            QUANLY_PHONGHOPBusiness = Get<QUANLY_PHONGHOPBusiness>();

            if (searchModel.calendarDay == null)
            {
                searchModel.calendarDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            }
            else
            {
                searchModel.calendarDay = new DateTime(searchModel.calendarDay.Value.Year, searchModel.calendarDay.Value.Month, searchModel.calendarDay.Value.Day);
            }

            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime currentDay = searchModel.calendarDay.Value;
            DateTime inValidDay = new DateTime(1, 1, 1);

            List<SelectListItem> groupOfYears = QUANLY_PHONGHOPBusiness.GetListYears(currentDay);
            List<SelectListItem> groupOfWeeks = Enumerable.Range(1, 52).Select(w => new SelectListItem()
            {
                Value = w.ToString(),
                Text = "Tuần " + w,
                Selected = (currentDay.GetWeekNumber() == w)
            }).ToList();

            var groupOfMonths = Enumerable.Range(1, 12).Select(m => new SelectListItem()
            {
                Value = m.ToString(),
                Text = "Tháng " + m,
                Selected = (currentDay.Month == m)
            }).ToList();

            searchModel.queryDeptParentID = currentUser.DeptParentID.GetValueOrDefault();
            if (searchModel.calendarType == LICH_CONSTANT.NGAY)
            {
                //nếu người dùng không nhập thông tin ngày bắt đầu và kết thúc mặc định lấy ngày đầu và kêt thúc của tuần hiện tại
                if (searchModel.queryStartDate == null && searchModel.queryEndDate == null)
                {
                    searchModel.queryStartDate = currentDay.GetStartDayOfWeek();
                    searchModel.queryEndDate = currentDay.GetEndDayOfWeek();
                }

                LichHopByDayViewModel viewModel = new LichHopByDayViewModel();
                viewModel.currentUserId = currentUser.ID;
                viewModel.startDate = searchModel.queryStartDate;
                viewModel.endDate = searchModel.queryEndDate;
                viewModel.title = string.Format("Từ {0} đến {1}", string.Format("{0:dd/MM/yyyy}", searchModel.queryStartDate), string.Format("{0:dd/MM/yyyy}", searchModel.queryEndDate));
                viewModel.groupOfEntities = QUANLY_PHONGHOPBusiness.GetData(searchModel)
                    .GroupBy(x => x.NGAYDAT.Value)
                    .OrderBy(x => x.Key)
                    .Select(x => new LichCongTacEntityViewModel()
                    {
                        isToday = x.Key.Equals(today),
                        title = string.Format("{0} ({1})", x.Key.GetNameOfDay(), x.Key.ToVietnameseDateFormat()),
                        groupOfCalendars = x.OrderBy(y => y.GIOBATDAU).ThenBy(y => y.PHUTBATDAU).ToList()
                    }).ToList();
                return PartialView("_LichDatPhongByDay", viewModel);
            }
            else if (searchModel.calendarType == LICH_CONSTANT.TUAN)
            {
                LichHopByWeekViewModel viewModel = new LichHopByWeekViewModel();
                viewModel.currentUserId = currentUser.ID;
                var startOfWeek = currentDay.GetStartDayOfWeek();
                var endOfWeek = currentDay.GetEndDayOfWeek();

                //gán vào search model
                searchModel.queryStartDate = startOfWeek;
                searchModel.queryEndDate = endOfWeek;

                //lấy các ngày trong tuần
                viewModel.groupOfDates = new List<DateTime>();
                viewModel.title = currentDay.GetNameOfWeek() + " (" + startOfWeek.GetNameOfDay() + ", " + startOfWeek.ToVietnameseDateFormat() + " - " + endOfWeek.GetNameOfDay() + ", " + endOfWeek.ToVietnameseDateFormat() + ")";
                viewModel.groupOfWeeks = groupOfWeeks;
                viewModel.groupOfYears = groupOfYears;
                for (DateTime d = startOfWeek; d <= endOfWeek; d = d.AddDays(1))
                {
                    viewModel.groupOfDates.Add(d);
                }

                //lấy từng khung giờ trong mỗi ngày
                viewModel.groupOfHours = new List<LichHopByHourViewModel>();
                for (int hour = 8; hour <= 23; hour++)
                {
                    LichHopByHourViewModel hourEntity = new LichHopByHourViewModel();
                    hourEntity.title = hour.ToString("D2");
                    hourEntity.groupOfEntities = new List<LichCongTacEntityViewModel>();
                    for (DateTime d = startOfWeek; d <= endOfWeek; d = d.AddDays(1))
                    {
                        LichCongTacEntityViewModel entity = new LichCongTacEntityViewModel();
                        entity.entityDay = d;
                        entity.isToday = d.Equals(today);
                        entity.groupOfCalendars = QUANLY_PHONGHOPBusiness.GetData(searchModel)
                            .Where(x => x.NGAYDAT == d)
                            .Where(x => x.GIOBATDAU == hour)
                            .OrderBy(x => x.PHUTBATDAU)
                            .ToList();
                        hourEntity.groupOfEntities.Add(entity);
                    }
                    viewModel.groupOfHours.Add(hourEntity);
                }

                return PartialView("_LichDatPhongByWeek", viewModel);
            }
            else
            {
                LichHopByMonthViewModel viewModel = new LichHopByMonthViewModel();
                viewModel.currentUserId = currentUser.ID;
                viewModel.day = currentDay.Day;
                viewModel.month = currentDay.Month;
                viewModel.year = currentDay.Year;
                viewModel.groupOfMonths = groupOfMonths;
                viewModel.groupOfYears = groupOfYears;
                viewModel.title = string.Format("Tháng {0}, năm {1}", viewModel.month, viewModel.year);
                //int weeksOfMonth = Utility.GetWeeksOfMonth(viewModel.month, viewModel.year);
                int lastDayOfMonth = DateTime.DaysInMonth(viewModel.year, viewModel.month);
                List<DateTime> daysInMonth = Enumerable.Range(1, lastDayOfMonth)
                    .Select(x => new DateTime(viewModel.year, viewModel.month, x))
                    .ToList();

                //gán vào search model
                searchModel.queryStartDate = new DateTime(viewModel.year, viewModel.month, 1);
                searchModel.queryEndDate = new DateTime(viewModel.year, viewModel.month, lastDayOfMonth);

                //lấy các tuần trong tháng
                viewModel.groupOfWeeks = new List<LichHopByWeekViewModel>();
                for (int week = 1; week <= 6; week++)
                {
                    LichHopByWeekViewModel weekEntity = new LichHopByWeekViewModel();
                    weekEntity.groupOfDays = new List<LichCongTacEntityViewModel>();

                    List<DateTime> daysOfWeek = daysInMonth.Where(x => x.GetWeekOfMonth() == week).ToList();
                    //lấy các ngày trong tuần
                    for (int day = (int)DayOfWeek.Sunday; day <= (int)DayOfWeek.Saturday; day++)
                    {
                        LichCongTacEntityViewModel dayEntity = new LichCongTacEntityViewModel();
                        DateTime dayOfWeek = daysOfWeek.Where(x => (int)x.DayOfWeek == day).FirstOrDefault();
                        if (dayOfWeek.Equals(inValidDay) == false)
                        {
                            dayEntity.title = dayOfWeek.Day.ToString();
                            dayEntity.entityDay = dayOfWeek;
                            dayEntity.isToday = dayOfWeek.Equals(today);
                            dayEntity.groupOfCalendars = QUANLY_PHONGHOPBusiness.GetData(searchModel)
                                .Where(x => x.NGAYDAT.Value.Day == dayOfWeek.Day)
                                .OrderBy(x => x.GIOBATDAU)
                                .ThenBy(x => x.PHUTBATDAU)
                                .ToList();
                        }
                        weekEntity.groupOfDays.Add(dayEntity);
                    }
                    viewModel.groupOfWeeks.Add(weekEntity);
                }
                return PartialView("_LichDatPhongByMonth", viewModel);
            }
        }

        /// <summary>
        /// @author: duynn
        /// @description: hiển thị cập nhật yêu cầu phòng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionAudit]
        public PartialViewResult EditRoomRequest(long id)
        {
            AssignUserInfo();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            QUANLY_PHONGHOPBusiness = Get<QUANLY_PHONGHOPBusiness>();
            QUANLY_PHONGHOP roomEntity = QUANLY_PHONGHOPBusiness.Find(id) ?? new QUANLY_PHONGHOP();
            EditVM viewModel = new EditVM(roomEntity);
            viewModel.groupLeaders = DM_NGUOIDUNGBusiness.GetDropDownByDeptParentId(currentUser.DeptParentID.GetValueOrDefault(), roomEntity.USER_ID);
            return PartialView("EditPhong", viewModel);
        }

        /// <summary>
        /// @author: duynn
        /// @description: tạo nhanh yêu cầu lịch hộp trên màn hình danh sách
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionAudit]
        public PartialViewResult EditFastRoomRequest(QUANLY_PHONGHOP item)
        {
            AssignUserInfo();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            QUANLY_PHONGHOPBusiness = Get<QUANLY_PHONGHOPBusiness>();
            QUANLY_PHONGHOP roomEntity;
            if (item.ID > 0)
            {
                roomEntity = QUANLY_PHONGHOPBusiness.Find(item.ID) ?? new QUANLY_PHONGHOP();
            }
            else
            {
                if (item.GIOBATDAU >= 8 && item.GIOBATDAU < 23)
                {
                    item.GIOKETTHUC = item.GIOBATDAU + 1;
                }
                else if (item.GIOBATDAU < 8)
                {
                    item.GIOBATDAU = 8;
                    item.GIOKETTHUC = item.GIOBATDAU + 1;
                }
                else
                {
                    item.GIOKETTHUC = item.GIOBATDAU;
                }
                roomEntity = item;
            }
            EditVM viewModel = new EditVM(roomEntity);
            viewModel.groupLeaders = DM_NGUOIDUNGBusiness.GetDropDownByDeptParentId(currentUser.DeptParentID.GetValueOrDefault(), roomEntity.USER_ID);
            return PartialView("EditPhong", viewModel);
        }

        /// <summary>
        /// @author: duynn
        /// @description: cập nhật yêu cầu lịch đặt phòng
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionAudit]
        public JsonResult SaveRoomRequest(FormCollection fc)
        {
            AssignUserInfo();
            SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
            QUANLY_PHONGHOPBusiness = Get<QUANLY_PHONGHOPBusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            JsonResultBO result = new JsonResultBO(true);

            QUANLY_PHONGHOP entity = new QUANLY_PHONGHOP();
            entity.ID = fc["ID"].ToIntOrZero();
            entity.USER_ID = fc["USER_ID"].ToLongOrZero();
            entity.MUCDICH = fc["MUCDICH"].Trim();
            entity.THANHPHANTHAMDU = fc["THANHPHANTHAMDU"].Trim();
            entity.GIOBATDAU = fc["GIOBATDAU"].ToIntOrZero();
            entity.PHUTBATDAU = fc["PHUTBATDAU"].ToIntOrZero();

            entity.GIOKETTHUC = fc["GIOKETTHUC"].ToIntOrZero();
            entity.PHUTKETTHUC = fc["PHUTKETTHUC"].ToIntOrZero();

            entity.CREATED_AT = DateTime.Now;
            entity.CREATED_BY = currentUser.ID;
            entity.NGAYDAT = fc["NGAYDAT"].ToDateTime();
            entity.IS_DELETE = false;
            entity.CCTC_THANHPHAN_ID = currentUser.DeptParentID.GetValueOrDefault();

            //danh sách người nhận thông báo đặt lịch họp
            List<long> notifyUsers = DM_NGUOIDUNGBusiness.GetListUsersByFunctionCodeAndDeptId("QUANLY_DAT_PHONGHOP", currentUser.DeptParentID.GetValueOrDefault());
            string title = string.Empty;
            string content = string.Empty;
            string url = string.Empty;
            string bookDate = entity.NGAYDAT.Value.ToVietnameseDateFormat();
            string startTime = string.Format("{0}h{1}", entity.GIOBATDAU.Value.ToString("D2"), entity.PHUTBATDAU.Value.ToString("D2"));
            string endTime = string.Format("{0}h{1}", entity.GIOKETTHUC.Value.ToString("D2"), entity.PHUTKETTHUC.Value.ToString("D2"));
            //kiểm tra trùng lịch lãnh đạo
            if (entity.USER_ID > 0)
            {
                List<QUANLY_PHONGHOP> bookingsOfUserInDay = QUANLY_PHONGHOPBusiness.GetBookingsOfUserInDay(entity.NGAYDAT.Value, entity.USER_ID);
                if(entity.ID > 0)
                {
                    bookingsOfUserInDay = bookingsOfUserInDay.Where(x => x.ID != entity.ID).ToList();
                }

                if (bookingsOfUserInDay.Any())
                {
                    int totalBookDuplicate = 0; //tổng số lịch bị trùng

                    int totalStartMinutes = entity.GIOBATDAU.GetValueOrDefault() * 60 + entity.PHUTBATDAU.GetValueOrDefault();
                    int totalEndMinutes = entity.GIOKETTHUC.GetValueOrDefault() * 60 + entity.PHUTKETTHUC.GetValueOrDefault();

                    foreach (var booking in bookingsOfUserInDay)
                    {
                        int totalStartMinutesDb = booking.GIOBATDAU.GetValueOrDefault() * 60 + booking.PHUTBATDAU.GetValueOrDefault();
                        int totalEndMinutesDb = booking.GIOKETTHUC.GetValueOrDefault() * 60 + booking.PHUTKETTHUC.GetValueOrDefault();

                        //kiểm tra khoảng thời gian trùng lặp
                        if (totalStartMinutes >= totalStartMinutesDb && totalStartMinutes <= totalStartMinutesDb) //giờ bắt đầu nằm trong khoảng họp
                        {
                            totalBookDuplicate++;
                        }
                        else if (totalStartMinutes <= totalStartMinutesDb && totalEndMinutes >= totalStartMinutesDb
                            && totalEndMinutes <= totalEndMinutesDb) //giờ kết thúc nằm trong khoảng họp
                        {
                            totalBookDuplicate++;
                        }

                        else if (totalStartMinutes >= totalStartMinutesDb
                            && totalEndMinutes <= totalStartMinutesDb) //giờ bắt đầu và giờ kết thúc đều nằm trong khoảng họp
                        {
                            totalBookDuplicate++;
                        }
                    }

                    if (totalBookDuplicate > 0)
                    {
                        result.Message = string.Format("Trong ngày {0} lãnh đạo có {1} lịch diễn ra trong khoảng thời gian trên", bookDate, totalBookDuplicate);
                        result.Status = false;
                        return Json(result);
                    }
                }
            }

            if (entity.ID > 0)
            {
                QUANLY_PHONGHOP dbEntity = QUANLY_PHONGHOPBusiness.Find(entity.ID);
                dbEntity.USER_ID = entity.USER_ID;
                dbEntity.MUCDICH = entity.MUCDICH;
                dbEntity.THANHPHANTHAMDU = entity.THANHPHANTHAMDU;
                dbEntity.GIOBATDAU = entity.GIOBATDAU;
                dbEntity.GIOKETTHUC = entity.GIOKETTHUC;

                dbEntity.NGAYDAT = entity.NGAYDAT;
                dbEntity.PHUTBATDAU = entity.PHUTBATDAU;
                dbEntity.PHUTKETTHUC = entity.PHUTKETTHUC;
                dbEntity.CCTC_THANHPHAN_ID = entity.CCTC_THANHPHAN_ID;
                QUANLY_PHONGHOPBusiness.Save(dbEntity);
                result.Message = "Cập nhật lịch họp thành công";

                title = "THÔNG BÁO THAY ĐổI LỊCH HỌP";
                content = string.Format("{0} đã thay đổi thông tin lịch họp vào ngày {1} lúc {2} đến {3}", currentUser.HOTEN, bookDate, startTime, endTime);
            }
            else
            {
                QUANLY_PHONGHOPBusiness.Save(entity);
                result.Message = "Thêm mới lịch họp thành công";

                title = "THÔNG BÁO ĐẶT LỊCH HỌP";
                content = string.Format("{0} đã đặt một lịch họp vào ngày {1} lúc {2} đến {3}", currentUser.HOTEN, bookDate, startTime, endTime);
            }

            //gửi tin nhắn cho người có nhiệm vụ đặt phòng
            SYS_TINNHANBusiness.sendMessageMultipleUsers(notifyUsers, currentUser, title, content, url, string.Empty, false, entity.ID, 0);
            return Json(result);
        }

        /// <summary>
        /// @author: duynn
        /// @description: quản lý danh sách phòng họp
        /// </summary>
        /// <returns></returns>
        [ActionAudit]
        [CodeAllowAccess(Code = "QUANLY_DAT_PHONGHOP")]
        public ActionResult DanhSachPhongHop()
        {
            return View();
        }

        [ActionAudit]
        public PartialViewResult SearchPhongHop(QLPHONGHOP_SEARCH searchModel)
        {
            AssignUserInfo();
            QL_PHONGHOPBusiness = Get<QL_PHONGHOPBusiness>();
            QUANLY_PHONGHOPBusiness = Get<QUANLY_PHONGHOPBusiness>();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            if (searchModel.calendarDay == null)
            {
                searchModel.calendarDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            }
            else
            {
                searchModel.calendarDay = new DateTime(searchModel.calendarDay.Value.Year, searchModel.calendarDay.Value.Month, searchModel.calendarDay.Value.Day);
            }
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime currentDay = searchModel.calendarDay.Value;
            PhongHopViewModel viewModel = new PhongHopViewModel();
            var startOfWeek = currentDay.GetStartDayOfWeek();
            var endOfWeek = currentDay.GetEndDayOfWeek();

            //gán vào search model
            searchModel.queryStartDate = startOfWeek;
            searchModel.queryEndDate = endOfWeek;
            searchModel.queryDeptParentID = currentUser.DeptParentID.GetValueOrDefault();
            //lấy các ngày trong tuần
            viewModel.groupOfDates = new List<DateTime>();
            for (DateTime day = startOfWeek; day <= endOfWeek; day = day.AddDays(1))
            {
                viewModel.groupOfDates.Add(day);
            }

            viewModel.title = currentDay.GetNameOfWeek() + " (" + startOfWeek.GetNameOfDay() + ", " + startOfWeek.ToVietnameseDateFormat() + " - " + endOfWeek.GetNameOfDay() + ", " + endOfWeek.ToVietnameseDateFormat() + ")";
            viewModel.groupOfYears = QUANLY_PHONGHOPBusiness.GetListYears(currentDay);
            viewModel.groupOfWeeks = Enumerable.Range(1, 52).Select(w => new SelectListItem()
            {
                Value = w.ToString(),
                Text = "Tuần " + w,
                Selected = (currentDay.GetWeekNumber() == w)
            }).ToList();

            //lấy danh sách các lịch đặt phòng
            List<QLPHONGHOP_BO> calendars = QUANLY_PHONGHOPBusiness.GetData(searchModel);
            //lấy danh sách các phòng
            List<QL_PHONGHOP> rooms = QL_PHONGHOPBusiness.context.QL_PHONGHOP.Where(x => x.DEPID == currentUser.DeptParentID).ToList();
            viewModel.groupOfCalendars = new List<LichCongTacEntityViewModel>();
            for (DateTime day = startOfWeek; day <= endOfWeek; day = day.AddDays(1))
            {
                LichCongTacEntityViewModel dayEntity = new LichCongTacEntityViewModel();
                dayEntity.title = string.Format("{0} ({1})", day.GetNameOfDay(), day.ToVietnameseDateFormat());
                dayEntity.isToday = day.Equals(today);
                dayEntity.groupOfCalendars = calendars.Where(x => x.NGAYDAT == day && x.PHONG_ID == 0)
                    .OrderBy(x => x.GIOBATDAU).ThenBy(x => x.PHUTBATDAU)
                    .ThenBy(x => x.USER_ID)
                    .ToList();
                viewModel.groupOfCalendars.Add(dayEntity);
            }
            viewModel.groupOfRooms = new List<PhongHopEntity>();
            foreach (var room in rooms)
            {
                PhongHopEntity roomEntity = new PhongHopEntity();
                roomEntity.name = room.TENPHONG;
                roomEntity.groupOfCalendars = new List<LichCongTacEntityViewModel>();
                for (DateTime day = startOfWeek; day <= endOfWeek; day = day.AddDays(1))
                {
                    LichCongTacEntityViewModel dayEntity = new LichCongTacEntityViewModel();
                    dayEntity.groupMorningItems = calendars
                        .Where(x => x.NGAYDAT == day && x.PHONG_ID == room.ID)
                        .Where(x => x.GIOBATDAU <= 12)
                        .OrderBy(x => x.GIOBATDAU)
                        .ThenBy(x => x.PHUTBATDAU)
                        .ToList();
                    dayEntity.groupAfternoonItems = calendars
                        .Where(x => x.NGAYDAT == day && x.PHONG_ID == room.ID)
                        .Where(x => x.GIOBATDAU > 13)
                        .OrderBy(x => x.GIOBATDAU)
                        .ThenBy(x => x.PHUTBATDAU)
                        .ToList();

                    roomEntity.groupOfCalendars.Add(dayEntity);
                }

                viewModel.groupOfRooms.Add(roomEntity);
            }
            return PartialView("_ListPhongHop", viewModel);
        }


        /// <summary>
        /// @description: xóa lịch họp
        /// @author: duynn
        /// @since: 14/08/2018
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionAudit]
        public JsonResult DeleteLichHop(long id, bool isCancel = false)
        {
            AssignUserInfo();
            JsonResultBO result = new JsonResultBO(true);
            QUANLY_PHONGHOPBusiness = Get<QUANLY_PHONGHOPBusiness>();
            QUANLY_PHONGHOP entity = QUANLY_PHONGHOPBusiness.Find(id);
            if (entity != null && entity.IS_DELETE != true)
            {
                entity.IS_DELETE = true;
                QUANLY_PHONGHOPBusiness.Save(entity);

                if (isCancel)
                {
                    SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
                    string bookDate = entity.NGAYDAT != null ? entity.NGAYDAT.Value.ToVietnameseDateFormat() : string.Empty;
                    string title = "THÔNG BÁO HỦY LỊCH";
                    string url = string.Empty;
                    string startTime = string.Format("{0}h{1}", entity.GIOBATDAU.Value.ToString("D2"), entity.PHUTBATDAU.Value.ToString("D2"));
                    string endTime = string.Format("{0}h{1}", entity.GIOKETTHUC.Value.ToString("D2"), entity.PHUTKETTHUC.Value.ToString("D2"));

                    string content = string.Format("Cuộc họp về \"{0}\" trong ngày {1} ({2} - {3}) đã bị hủy", entity.MUCDICH, bookDate, startTime, endTime);
                    SYS_TINNHANBusiness.sendMessageMultipleUsers(new List<long> { entity.CREATED_BY.GetValueOrDefault() }, currentUser, title, content, url, string.Empty, false, entity.ID, 0);
                }
            }
            else
            {
                result.Status = false;
            }
            return Json(result);
        }
        /// <summary>
        /// @author: duynn
        /// @description: book phòng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionAudit]
        public PartialViewResult BookingRoom(long id)
        {
            AssignUserInfo();
            QUANLY_PHONGHOPBusiness = Get<QUANLY_PHONGHOPBusiness>();
            QL_PHONGHOPBusiness = Get<QL_PHONGHOPBusiness>();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            DetailVM viewModel = new DetailVM();
            viewModel.roomEntity = QUANLY_PHONGHOPBusiness.GetDetail(id);
            viewModel.groupOfRooms = QL_PHONGHOPBusiness.context.QL_PHONGHOP
                .Where(x => x.DEPID == currentUser.DeptParentID)
                .ToList().Select(x => new SelectListItem()
                {
                    Value = x.ID.ToString(),
                    Text = string.Format("{0} - {1}", x.TENPHONG, x.MAPHONG),
                }).ToList();
            return PartialView("_BookingRoom", viewModel);
        }

        /// <summary>
        /// @author: duynn
        /// @description: thông tin book
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionAudit]
        public PartialViewResult DetailBooking(long id)
        {
            QUANLY_PHONGHOPBusiness = Get<QUANLY_PHONGHOPBusiness>();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            DetailVM viewModel = new DetailVM();
            viewModel.roomEntity = QUANLY_PHONGHOPBusiness.GetDetail(id);
            return PartialView("_DetailBooking", viewModel);
        }

        /// <summary>
        /// @author: duynn
        /// @description: kiểm tra đặt phòng họp
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="roomId"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionAudit]
        public JsonResult CheckBookingRoom(long bookId, int roomId)
        {
            QUANLY_PHONGHOPBusiness = Get<QUANLY_PHONGHOPBusiness>();
            QUANLY_PHONGHOP bookEntity = QUANLY_PHONGHOPBusiness.Find(bookId);
            if (bookEntity != null && bookEntity.IS_DELETE != true)
            {
                List<QUANLY_PHONGHOP> roomBookings = QUANLY_PHONGHOPBusiness.GetBookingsOfRoomInDay(bookEntity.NGAYDAT.GetValueOrDefault(), roomId);
                roomBookings = roomBookings.Where(x => x.GIOBATDAU <= 12).ToList();
                if (roomBookings.Any())
                {
                    int totalBookDuplicate = 0; //tổng số lịch bị trùng
                    int totalNormalBookDuplicate = 0; //tổng số lịch thường bị trùng
                    int totalSpecialBookDuplicate = 0; //tổng số lịch đặt biệt bị trùng
                    List<int> duplicateIds = new List<int>();

                    //tính tổng số phút bắt đầu - kết thúc để tính sự trùng lặp
                    int totalStartMinutes = bookEntity.GIOBATDAU.GetValueOrDefault() * 60 + bookEntity.PHUTBATDAU.GetValueOrDefault();
                    int totalEndMinutes = bookEntity.GIOKETTHUC.GetValueOrDefault() * 60 + bookEntity.PHUTKETTHUC.GetValueOrDefault();

                    foreach (var roomBooking in roomBookings)
                    {
                        bool isDuplicate = false;
                        int totalStartMinutesDb = roomBooking.GIOBATDAU.GetValueOrDefault() * 60 + roomBooking.PHUTBATDAU.GetValueOrDefault();
                        int totalEndMinutesDb = roomBooking.GIOKETTHUC.GetValueOrDefault() * 60 + roomBooking.PHUTKETTHUC.GetValueOrDefault();

                        //kiểm tra khoảng thời gian trùng lặp
                        if (totalStartMinutes <= totalEndMinutesDb && totalEndMinutes >= totalStartMinutesDb)
                        {
                            isDuplicate = true;
                        }
                        if (isDuplicate)
                        {
                            totalBookDuplicate++;
                            if (roomBooking.USER_ID != 0)
                            {
                                totalSpecialBookDuplicate++;
                            }
                            else
                            {
                                totalNormalBookDuplicate++;
                                duplicateIds.Add(roomBooking.ID);
                            }
                        }
                    }

                    if (totalBookDuplicate == 0)
                    {
                        return Json(new { isDuplicate = false });
                    }
                    else
                    {
                        return Json(new
                        {
                            isDuplicate = true,
                            totalBookDuplicate = totalBookDuplicate,
                            totalNormalBookDuplicate = totalNormalBookDuplicate,
                            totalSpecialBookDuplicate = totalSpecialBookDuplicate,
                            duplicateIds = string.Join(",", duplicateIds.ToArray())
                        });
                    }
                }
                else
                {
                    return Json(new { isDuplicate = false });
                }
            }
            else
            {
                return Json(null);
            }
        }

        /// <summary>
        /// @author: duynn
        /// @description: lưu thông tin đặt phòng
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="roomId"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionAudit]
        public JsonResult SaveBooking(FormCollection col)
        {
            AssignUserInfo();

            JsonResultBO result = new JsonResultBO(true);
            QUANLY_PHONGHOPBusiness = Get<QUANLY_PHONGHOPBusiness>();
            QL_PHONGHOPBusiness = Get<QL_PHONGHOPBusiness>();
            SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
            List<long> notifyUsers = new List<long>();
            string title = string.Empty;
            string content = string.Empty;
            string url = string.Empty;
            string bookDate = string.Empty;
            string startTime = string.Empty;
            string endTime = string.Empty;
            string roomName = string.Empty;

            QUANLY_PHONGHOP bookEntity = QUANLY_PHONGHOPBusiness.Find(col["ID"].ToIntOrZero());
            if (bookEntity != null && bookEntity.IS_DELETE != true)
            {
                bookEntity.PHONG_ID = col["PHONG_ID"].ToIntOrZero();
                QUANLY_PHONGHOPBusiness.Save(bookEntity);
                //hủy lịch các phòng khác
                string duplicateIds = col["DUPLICATE"];
                if (!string.IsNullOrEmpty(duplicateIds))
                {
                    List<int> ids = duplicateIds.ToListInt(',');
                    foreach (var id in ids)
                    {
                        QUANLY_PHONGHOP cancelCalendar = QUANLY_PHONGHOPBusiness.Find(id);
                        if (cancelCalendar != null)
                        {
                            cancelCalendar.IS_DELETE = true;
                            QUANLY_PHONGHOPBusiness.Save(cancelCalendar);

                            //gửi thông báo hủy lịch đến người đặt lịch
                            bookDate = cancelCalendar.NGAYDAT != null ? cancelCalendar.NGAYDAT.Value.ToVietnameseDateFormat() : string.Empty;
                            startTime = string.Format("{0}h{1}", cancelCalendar.GIOBATDAU.Value.ToString("D2"), cancelCalendar.PHUTBATDAU.Value.ToString("D2"));
                            endTime = string.Format("{0}h{1}", cancelCalendar.GIOKETTHUC.Value.ToString("D2"), cancelCalendar.PHUTKETTHUC.Value.ToString("D2"));

                            title = "THÔNG BÁO HỦY LỊCH";
                            content = string.Format("Cuộc họp về \"{0}\" trong ngày {1} ({2} - {3}) đã bị hủy", cancelCalendar.MUCDICH, bookDate, startTime, endTime);
                            SYS_TINNHANBusiness.sendMessageMultipleUsers(new List<long> { cancelCalendar.CREATED_BY.GetValueOrDefault() }, currentUser, title, content, url, string.Empty, false, cancelCalendar.ID, 0);
                        }
                    }
                }
                result.Message = "Đặt phòng thành công";

                //lấy thông tin phòng, người đặt lịch và thời gian đặt lịch
                QL_PHONGHOP room = QL_PHONGHOPBusiness.Find(bookEntity.PHONG_ID) ?? new QL_PHONGHOP();
                roomName = room.TENPHONG;
                notifyUsers.Add(bookEntity.CREATED_BY.GetValueOrDefault());
                bookDate = bookEntity != null ? bookEntity.NGAYDAT.Value.ToVietnameseDateFormat() : string.Empty;
                startTime = string.Format("{0}h{1}", bookEntity.GIOBATDAU.Value.ToString("D2"), bookEntity.PHUTBATDAU.Value.ToString("D2"));
                endTime = string.Format("{0}h{1}", bookEntity.GIOKETTHUC.Value.ToString("D2"), bookEntity.PHUTKETTHUC.Value.ToString("D2"));
            }
            else
            {
                result.Status = false;
                result.Message = "Yêu cầu đặt lịch không tồn tại";
            }

            title = "THÔNG BÁO ĐẶT PHÒNG";
            content = string.Format("Cuộc họp về \"{0}\" trong ngày {1} ({2} - {3}) được đặt tại {4}", bookEntity.MUCDICH, bookDate, startTime, endTime, roomName);
            SYS_TINNHANBusiness.sendMessageMultipleUsers(notifyUsers, currentUser, title, content, url, string.Empty, false, bookEntity.ID, 0);
            return Json(result);
        }
    }
}