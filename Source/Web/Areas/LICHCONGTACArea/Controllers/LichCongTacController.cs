using Business.Business;
using Business.CommonModel.LICH_CONGTAC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Areas.LICHCONGTACArea.Models;
using Web.Custom;
using CommonHelper.DateExtend;
using Web.FwCore;
using Model.Entities;
using CommonHelper;
using Business.CommonBusiness;
using Business.CommonModel.DMNguoiDung;
using Business.CommonModel.CONSTANT;
using System.Reflection;
using System.IO;
using System.Configuration;
using Web.Common;
using Web.Filter;

namespace Web.Areas.LICHCONGTACArea.Controllers
{
    public class LichCongTacController : BaseController
    {
        // GET: LICHCONGTACArea/LichCongTac
        private DM_DANHMUC_DATABusiness dmDanhMucDataBusiness;
        private HSCV_VANBANDENBusiness hscvVanBanDenBusiness;
        private LICHCONGTACBusiness lichCongTacBusiness;
        private CCTC_THANHPHANBusiness cctcThanhPhanBusiness;
        private DM_NGUOIDUNGBusiness dmNguoiDungBusiness;
        private WF_LOGBusiness wfLogBusiness;
        private QL_DANGKY_XEBusiness qlDangKyXeBusiness;
        private DM_NHOMDANHMUCBusiness dmNhomDanhMucBusiness;

        private string[] arrProperties = new string[] { "TIEUDE", "NGAY_CONGTAC", "GIO_CONGTAC", "PHUT_CONGTAC", "LANHDAO_ID", "DIADIEM", "GHICHU" };

        [ActionAudit]
        public ActionResult Index()
        {
            AssignUserInfo();
            var demo = currentUser;
            dmNguoiDungBusiness = Get<DM_NGUOIDUNGBusiness>();
            LichCongTacViewModel viewModel = new LichCongTacViewModel();
            viewModel.calendarType = LICH_CONSTANT.NGAY;
            viewModel.groupOfLanhDaos = dmNguoiDungBusiness.GetDropDownByDeptParentId(currentUser.DeptParentID.GetValueOrDefault(), 0);
            viewModel.isLeader = dmNguoiDungBusiness.CheckIsLeader(currentUser.ID);
            //kiểm tra là lãnh đạo
            if (viewModel.isLeader)
            {
                viewModel.groupOfLanhDaos = new List<SelectListItem>() {
                    new SelectListItem() {
                        Value = currentUser.ID.ToString(),
                        Text = currentUser.HOTEN
                    }
                };
            }

            DM_THAOTAC userFunction = currentUser.ListThaoTac.Where(o => o.MA_THAOTAC.ToUpper() == "TAO_LICHCONGTAC").FirstOrDefault();
            viewModel.canCreate = (userFunction != null && userFunction.DM_THAOTAC_ID > 0);
            return View(viewModel);
        }

        /// <summary>
        /// @description: tạo lịch công tác
        /// @author: duynn
        /// @since: 14/08/2018
        /// </summary>
        /// <returns></returns>
        [ActionAudit]
        public ActionResult CreateLichCongTac(long idVanBanDen = 0)
        {
            AssignUserInfo();
            hscvVanBanDenBusiness = Get<HSCV_VANBANDENBusiness>();
            cctcThanhPhanBusiness = Get<CCTC_THANHPHANBusiness>();
            dmNguoiDungBusiness = Get<DM_NGUOIDUNGBusiness>();
            dmDanhMucDataBusiness = Get<DM_DANHMUC_DATABusiness>();
            wfLogBusiness = Get<WF_LOGBusiness>();

            EditLichCongTacViewModel model = new EditLichCongTacViewModel();
            long userId = 0;
            if (idVanBanDen > 0)
            {
                HSCV_VANBANDEN entityVanBanDen = hscvVanBanDenBusiness.Find(idVanBanDen);
                if (entityVanBanDen != null && entityVanBanDen.NGUOITAO == currentUser.ID && hscvVanBanDenBusiness.CheckIsFinish(entityVanBanDen.ID))
                {
                    LICHCONGTAC calendar = new LICHCONGTAC();
                    calendar.TIEUDE = entityVanBanDen.TRICHYEU;
                    calendar.NGAY_CONGTAC = entityVanBanDen.NGAYCONGTAC ?? DateTime.Now;
                    calendar.GIO_CONGTAC = entityVanBanDen.GIO_CONGTAC.GetValueOrDefault();
                    calendar.PHUT_CONGTAC = entityVanBanDen.PHUT_CONGTAC.GetValueOrDefault();
                    model = new EditLichCongTacViewModel(calendar);
                    model.entityLichCongTac = calendar;

                    //lấy người xử lý cuối làm người chủ trì công tác
                    DM_NGUOIDUNG_BO lastProcessor = wfLogBusiness.GetFinalProcessor(entityVanBanDen.ID, MODULE_CONSTANT.VANBANDEN);
                    userId = lastProcessor.ID;
                }
                else
                {
                    return Redirect("/Home/UnAuthor");
                }
            }
            else if (SessionManager.HasValue("CreateCalendarOfTheDay"))
            {
                LICHCONGTAC entityCalendar = (LICHCONGTAC)SessionManager.GetValue("CreateCalendarOfTheDay");
                model = new EditLichCongTacViewModel(entityCalendar);

                SessionManager.Remove("CreateCalendarOfTheDay");
            }
            model.groupOfLanhDaos = dmNguoiDungBusiness.GetDropDownByDeptParentId(currentUser.DeptParentID.GetValueOrDefault(), userId);
            model.groupOfDestinations = dmDanhMucDataBusiness.GetGroupTextByCode(DMLOAI_CONSTANT.DIEM_DEN);
            return View("EditLichCongTac", model);
        }

        /// <summary>
        /// @author: duynn
        /// @description: tạo nhanh lịch công tác
        /// </summary>
        /// <returns></returns>
        [ActionAudit]
        public PartialViewResult EditFastLichCongTac(int year, int month, int day, int hour = 0, int minute = 0, long id = 0)
        {
            AssignUserInfo();
            lichCongTacBusiness = Get<LICHCONGTACBusiness>();
            dmNguoiDungBusiness = Get<DM_NGUOIDUNGBusiness>();
            dmDanhMucDataBusiness = Get<DM_DANHMUC_DATABusiness>();

            EditLichCongTacViewModel model = new EditLichCongTacViewModel();
            List<long> groupLanhDaoIds = new List<long>();
            long userId = 0;
            if (id > 0)
            {
                LICHCONGTAC entity = lichCongTacBusiness.Find(id);
                if (entity != null && entity.IS_DELETE != true && entity.NGAY_CONGTAC.IsOldWeek() == false)
                {
                    model = new EditLichCongTacViewModel(entity);
                    userId = entity.LANHDAO_ID.GetValueOrDefault();
                }
            }
            else
            {
                LICHCONGTAC entity = new LICHCONGTAC();
                entity.NGAY_CONGTAC = new DateTime(year, month, day);
                entity.GIO_CONGTAC = hour;
                entity.PHUT_CONGTAC = minute;
                model = new EditLichCongTacViewModel(entity);
            }
            model.groupOfLanhDaos = dmNguoiDungBusiness.GetDropDownByDeptParentId(currentUser.DeptParentID.GetValueOrDefault(), userId);
            model.groupOfDestinations = dmDanhMucDataBusiness.GetGroupTextByCode(DMLOAI_CONSTANT.DIEM_DEN);
            model.isPopUp = true;
            return PartialView("_EditFastLichCongTac", model);
        }

        /// <summary>
        /// @description: cập nhật lịch công tác
        /// @author: duynn
        /// @since: 14/08/2018
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionAudit]
        public ActionResult UpdateLichCongTac(long id)
        {
            AssignUserInfo();
            JsonResultBO editResult = new JsonResultBO(true);
            cctcThanhPhanBusiness = Get<CCTC_THANHPHANBusiness>();
            dmNguoiDungBusiness = Get<DM_NGUOIDUNGBusiness>();
            lichCongTacBusiness = Get<LICHCONGTACBusiness>();
            dmDanhMucDataBusiness = Get<DM_DANHMUC_DATABusiness>();

            LICHCONGTAC entity = lichCongTacBusiness.Find(id);
            if (entity != null && entity.IS_DELETE != true && entity.NGAY_CONGTAC.IsOldWeek() == false)
            {
                EditLichCongTacViewModel model = new EditLichCongTacViewModel(entity);
                model.groupOfLanhDaos = dmNguoiDungBusiness.GetDropDownByDeptParentId(currentUser.DeptParentID.GetValueOrDefault(), entity.LANHDAO_ID.GetValueOrDefault());
                model.groupOfDestinations = dmDanhMucDataBusiness.GetGroupTextByCode(DMLOAI_CONSTANT.DIEM_DEN);
                return View("EditLichCongTac", model);
            }
            return Redirect("/Home/UnAuthor");
        }

        /// <summary>
        /// @description: lưu lịch công tác
        /// @author: duynn
        /// @since: 14/08/2018
        /// </summary>
        /// <returns></returns>
        [ActionAudit]
        [HttpPost]
        public JsonResult SaveLichCongTac(LICHCONGTAC entity, FormCollection fc)
        {
            AssignUserInfo();
            qlDangKyXeBusiness = Get<QL_DANGKY_XEBusiness>();
            lichCongTacBusiness = Get<LICHCONGTACBusiness>();
            dmNhomDanhMucBusiness = Get<DM_NHOMDANHMUCBusiness>();
            dmDanhMucDataBusiness = Get<DM_DANHMUC_DATABusiness>();

            JsonResultBO editResult = new JsonResultBO(true);
            entity.NGAY_CONGTAC = fc["NGAY_CONGTAC"].ToDateTimeNotNull();

            bool isObsolete = entity.NGAY_CONGTAC.IsOldWeek();
            if (isObsolete)
            {
                editResult.Status = false;
                editResult.Message = "Thời gian công tác đã quá hạn để thêm mới";
                return Json(editResult);
            }

            entity.TIEUDE = fc["TIEUDE"].Trim();
            entity.GIO_CONGTAC = fc["GIO_CONGTAC"].ToIntOrZero();
            entity.PHUT_CONGTAC = fc["PHUT_CONGTAC"].ToIntOrZero();
            //entity.NGUOICHUTRI_ID = fc["NGUOICHUTRI_ID"].ToIntOrNULL();
            entity.LANHDAO_ID = fc["LANHDAO_ID"].ToIntOrNULL();
            entity.DIADIEM = fc["DIADIEM"].Trim();
            entity.GHICHU = fc["GHICHU"].Trim();
            entity.NGUOITAO = currentUser.ID;
            entity.NGUOISUA = currentUser.ID;
            entity.NGAYTAO = DateTime.Now;
            entity.NGAYSUA = DateTime.Now;
            entity.IS_LATTEST = true;
            entity.IS_DELETE = false;
            string destination = fc["DIADIEM"].Trim();

            //kiểm tra trùng lịch của cán bộ trong ngày
            bool existed = lichCongTacBusiness.CheckIsDuplicate(entity.ID, entity.LANHDAO_ID.GetValueOrDefault(), entity.NGAY_CONGTAC, entity.GIO_CONGTAC, entity.PHUT_CONGTAC);
            if(existed == true)
            {
                editResult.Status = false;
                editResult.Message = "Lịch công tác của cán bộ đã tồn tại";
                return Json(editResult);
            }

            DM_DANHMUC_DATA dataItem = dmDanhMucDataBusiness.GetItemByCodeAndText(DMLOAI_CONSTANT.DIEM_DEN, destination);
            if (dataItem == null)
            {
                DM_NHOMDANHMUC groupCategory = dmNhomDanhMucBusiness.GetByCode(DMLOAI_CONSTANT.DIEM_DEN) ?? new DM_NHOMDANHMUC();
                DM_DANHMUC_DATA destinationEntity = new DM_DANHMUC_DATA();
                destinationEntity.DM_NHOM_ID = groupCategory.ID;
                destinationEntity.TEXT = destination;
                dmDanhMucDataBusiness.Save(destinationEntity);
            }

            if (entity.ID <= 0)
            {
                lichCongTacBusiness.Save(entity);
                editResult.Message = "Thêm mới lịch công tác thành công";
            }
            else
            {
                LICHCONGTAC dbEntity = lichCongTacBusiness.Find(entity.ID);
                if (dbEntity != null && dbEntity.IS_DELETE != true)
                {
                    if (dbEntity.NGAY_CONGTAC.IsOldWeek())
                    {
                        editResult.Status = false;
                        editResult.Message = "Thời gian công tác đã quá hạn cập nhật";
                        return Json(editResult);
                    }

                    //so sánh 2 phiên bản nếu khác => tạo phiên bản mới có ROOT_LICH = 0;
                    bool hasChanged = CheckHasChanged(arrProperties, dbEntity, entity);
                    if (hasChanged)
                    {
                        entity.ID = 0;
                        entity.LICH_GOC_ID = dbEntity.ID;
                        dbEntity.IS_LATTEST = false; //cập nhật phiên bản trước đã cũ
                        lichCongTacBusiness.Save(entity);

                        //kiểm tra xem lịch công tác này đã được đăng ký xe hay chưa
                        //nếu có thì cập nhật đăng ký xe
                        QL_DANGKY_XE register = qlDangKyXeBusiness.GetAvailableRegistrationByCalendarId(dbEntity.ID);
                        if (register != null)
                        {
                            register.LICHCONGTAC_ID = entity.ID;
                            qlDangKyXeBusiness.Save(register);
                        }
                    }
                    lichCongTacBusiness.Save(dbEntity);
                    editResult.Message = "Cập nhật lịch công tác thành công";
                }
                else
                {
                    editResult.Status = false;
                    editResult.Message = "Lịch công tác không tồn tại";
                    return Json(editResult);
                }
            }

            return Json(editResult);
        }

        /// <summary>
        /// @description: xóa lịch công tác
        /// @author: duynn
        /// @since: 14/08/2018
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionAudit]
        [HttpPost]
        public JsonResult DeleteLichCongTac(long id)
        {
            AssignUserInfo();
            JsonResultBO result = new JsonResultBO(true);
            lichCongTacBusiness = Get<LICHCONGTACBusiness>();
            qlDangKyXeBusiness = Get<QL_DANGKY_XEBusiness>();

            LICHCONGTAC calendar = lichCongTacBusiness.Find(id);
            if (calendar != null && calendar.IS_DELETE != true && calendar.NGUOITAO == currentUser.ID)
            {
                calendar.IS_DELETE = true;
                lichCongTacBusiness.Save(calendar);

                //kiểm tra lịch công tác có đăng ký xe hay không?
                //nếu có thì hủy lịch đăng ký xe
                QL_DANGKY_XE registration = qlDangKyXeBusiness.GetAvailableRegistrationByCalendarId(id);
                if (registration != null)
                {
                    registration.IS_DELETE = true;
                    qlDangKyXeBusiness.Save(registration);
                }
            }
            else
            {
                result.Status = false;
                result.Message = "Không tìm thấy lịch công tác";
            }
            return Json(result);
        }

        /// <summary>
        /// @author: duynn
        /// @description: kiểm tra 2 phiên bản có khác nhau để cập nhật phiên bản
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="oldEntity"></param>
        /// <param name="newEntity"></param>
        /// <returns></returns>
        [ActionAudit]
        public bool CheckHasChanged(string[] properties, LICHCONGTAC oldEntity, LICHCONGTAC newEntity)
        {
            bool isChange = false;
            Type entityType = typeof(LICHCONGTAC);
            foreach (string propertyName in properties)
            {
                PropertyInfo propertyInfo = entityType.GetProperty(propertyName);
                var oldValue = propertyInfo.GetValue(oldEntity);
                var newValue = propertyInfo.GetValue(newEntity);

                if (oldValue != newValue)
                {
                    isChange = true;
                    break;
                }
            }
            return isChange;
        }

        /// <summary>
        /// @author: duynn
        /// @description: thông tin chi tiết lịch công tác
        /// @since: 15/08/2018
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionAudit]
        public PartialViewResult DetailLichCongTac(long id, bool isShowCarRegistration = false)
        {
            qlDangKyXeBusiness = Get<QL_DANGKY_XEBusiness>();
            lichCongTacBusiness = Get<LICHCONGTACBusiness>();
            LichCongTacDetailViewModel viewModel = new LichCongTacDetailViewModel();
            viewModel.calendarEntity = lichCongTacBusiness.GetDetailLichCongTac(id);
            viewModel.isShowCarRegistration = isShowCarRegistration;
            if (isShowCarRegistration)
            {
                viewModel.carRegistrationEntity = qlDangKyXeBusiness.GetAvailableRegistrationByCalendarId(id);
            }
            return PartialView("_DetailLichCongTac", viewModel);
        }

        /// <summary>
        /// @description: tìm kiếm lịch công tác
        /// @author: duynn
        /// @since: 14/08/2018
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [ActionAudit]
        public PartialViewResult SearchLichCongTac(LICHCONGTAC_SEARCH searchModel)
        {
            AssignUserInfo();
            lichCongTacBusiness = Get<LICHCONGTACBusiness>();
            DM_THAOTAC userFunction = currentUser.ListThaoTac.Where(o => o.MA_THAOTAC.ToUpper() == "TAO_LICHCONGTAC").FirstOrDefault();
            searchModel.queryDeptId = currentUser.DeptParentID;
            if (searchModel.calendarDay == null)
            {
                searchModel.calendarDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            }
            else
            {
                var calendarDayValue = searchModel.calendarDay.Value;
                searchModel.calendarDay = new DateTime(calendarDayValue.Year, calendarDayValue.Month, calendarDayValue.Day);
            }
            var currentDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var day = searchModel.calendarDay.Value;
            var groupOfYears = lichCongTacBusiness.GetListYears(day);
            var groupOfWeeks = Enumerable.Range(1, 52).Select(w => new SelectListItem()
            {
                Value = w.ToString(),
                Text = "Tuần " + w,
                Selected = (day.GetWeekNumber() == w)
            }).ToList();
            var groupOfMonths = Enumerable.Range(1, 12).Select(m => new SelectListItem()
            {
                Value = m.ToString(),
                Text = "Tháng " + m,
                Selected = (day.Month == m)
            }).ToList();

            if (searchModel.calendarType == LICH_CONSTANT.NGAY)
            {
                //nếu người dùng tìm kiếm không nhập ngày bắt đầu và kết thúc mặc định lấy của tuần này
                if (searchModel.startDate == null && searchModel.endDate == null)
                {
                    searchModel.startDate = day.GetStartDayOfWeek();
                    searchModel.endDate = day.GetEndDayOfWeek();
                }
                LichCongTacByDayViewModel viewModel = new LichCongTacByDayViewModel();
                viewModel.currentUserId = currentUser.ID;
                viewModel.startDate = searchModel.startDate;
                viewModel.endDate = searchModel.endDate;
                viewModel.groupOfYears = groupOfYears;
                viewModel.title = "Từ " + string.Format("{0:dd/MM/yyyy}", searchModel.startDate) + " đến " + string.Format("{0:dd/MM/yyyy}", searchModel.endDate);
                viewModel.groupDateEntities = lichCongTacBusiness.GetListLichCongTacs(searchModel)
                    .GroupBy(x => x.NGAY_CONGTAC).OrderBy(x => x.Key)
                    .Select(x => new LichCongTacDateEntity()
                    {
                        title = x.Key.GetNameOfDay() + " (" + x.Key.ToVietnameseDateFormat() + ")",
                        isToday = x.Key.Equals(currentDay),
                        groupOfCalendars = x.OrderBy(y => y.GIO_CONGTAC).ThenBy(y => y.PHUT_CONGTAC).ToList()
                    }).ToList();
                return PartialView("_ListLichCongTacByDayOfWeek", viewModel);
            }
            else if (searchModel.calendarType == LICH_CONSTANT.TUAN)
            {
                LichCongTacByWeekViewModel viewModel = new LichCongTacByWeekViewModel();
                var startOfWeek = day.GetStartDayOfWeek();
                var endOfWeek = day.GetEndDayOfWeek();
                viewModel.currentUserId = currentUser.ID;
                viewModel.canCreate = (userFunction != null && userFunction.DM_THAOTAC_ID > 0);
                viewModel.title = day.GetNameOfWeek() + " (" + startOfWeek.GetNameOfDay() + ", " + startOfWeek.ToVietnameseDateFormat() + " - " + endOfWeek.GetNameOfDay() + ", " + endOfWeek.ToVietnameseDateFormat() + ")";
                viewModel.groupOfYears = groupOfYears;
                viewModel.groupOfWeeks = groupOfWeeks;

                searchModel.startDate = startOfWeek;
                searchModel.endDate = endOfWeek;
                List<LICHCONGTAC_BO> calendars = lichCongTacBusiness.GetListLichCongTacs(searchModel);

                //lấy các ngày trong tuần
                viewModel.groupOfDates = new List<DateTime>();
                for (var d = startOfWeek; d <= endOfWeek; d = d.AddDays(1))
                {
                    viewModel.groupOfDates.Add(d);
                }

                //lấy lịch theo từng khung giờ theo từng ngày
                viewModel.groupWeekHourEntities = new List<LichCongTacByWeekHourEntity>();
                for (int h = 0; h <= 23; h++)
                {
                    LichCongTacByWeekHourEntity hourEntity = new LichCongTacByWeekHourEntity();
                    hourEntity.title = h.ToString("D2") + "h";
                    hourEntity.hour = h;
                    hourEntity.groupDateEntities = new List<LichCongTacDateEntity>();
                    for (var d = startOfWeek; d <= endOfWeek; d = d.AddDays(1))
                    {
                        LichCongTacDateEntity hourDateEntity = new LichCongTacDateEntity();
                        hourDateEntity.date = d;
                        hourDateEntity.groupOfCalendars = new List<LICHCONGTAC_BO>();
                        hourDateEntity.groupOfCalendars.AddRange(calendars
                            .Where(x => x.NGAY_CONGTAC.Day == d.Day && x.GIO_CONGTAC == h)
                            .OrderBy(x => x.GIO_CONGTAC).ThenBy(x => x.PHUT_CONGTAC).ToList());
                        hourDateEntity.isToday = (d.Equals(currentDay));
                        hourEntity.groupDateEntities.Add(hourDateEntity);
                    }
                    viewModel.groupWeekHourEntities.Add(hourEntity);
                }
                return PartialView("_ListLichCongTacByWeek", viewModel);
            }
            else
            {
                LichCongTacByMonthViewModel viewModel = new LichCongTacByMonthViewModel();
                viewModel.currentUserId = currentUser.ID;
                viewModel.canCreate = (userFunction != null && userFunction.DM_THAOTAC_ID > 0);
                viewModel.year = day.Year;
                viewModel.month = day.Month;
                viewModel.title = "Tháng " + day.Month + ", năm " + day.Year;
                viewModel.groupOfYears = groupOfYears;
                viewModel.groupOfMonths = groupOfMonths;

                //lấy ngày đầu tiên và cuối cùng trong tháng
                int lastDayInMonth = DateTime.DaysInMonth(day.Year, day.Month);
                searchModel.startDate = (new DateTime(day.Year, day.Month, 1));
                searchModel.endDate = (new DateTime(day.Year, day.Month, lastDayInMonth));
                List<LICHCONGTAC_BO> calendars = lichCongTacBusiness.GetListLichCongTacs(searchModel);

                List<DateTime> datesOfMonth = Enumerable.Range(1, lastDayInMonth)
                    .Select(x => new DateTime(day.Year, day.Month, x)).ToList();
                DateTime defaultDate = new DateTime(1, 1, 1);

                viewModel.groupWeekEntities = new List<LichCongTacByMonthWeekEntity>();
                for (int w = 1; w <= 6; w++)
                {
                    LichCongTacByMonthWeekEntity weekEntity = new LichCongTacByMonthWeekEntity();
                    List<DateTime> daysOfWeek = datesOfMonth.Where(x => x.GetWeekOfMonth() == w).ToList();
                    if (daysOfWeek.Any())
                    {
                        weekEntity.groupDateEntities = new List<LichCongTacDateEntity>();
                        for (int d = (int)DayOfWeek.Sunday; d <= (int)DayOfWeek.Saturday; d++)
                        {
                            DateTime weekDay = daysOfWeek.Where(x => (int)x.DayOfWeek == d).FirstOrDefault();
                            LichCongTacDateEntity dateEntity = new LichCongTacDateEntity();
                            if (weekDay != defaultDate)
                            {
                                dateEntity.isToday = weekDay.Equals(currentDay);
                                dateEntity.date = weekDay;
                                dateEntity.title = weekDay.Day.ToString();
                                dateEntity.groupOfCalendars = calendars.Where(x => x.NGAY_CONGTAC.Day == weekDay.Day)
                                    .OrderBy(x => x.GIO_CONGTAC)
                                    .ThenBy(x => x.PHUT_CONGTAC)
                                    .ToList();
                            }
                            weekEntity.groupDateEntities.Add(dateEntity);
                        }
                        viewModel.groupWeekEntities.Add(weekEntity);
                    }
                }
                return PartialView("_ListLichCongTacByMonth", viewModel);
            }
        }

        /// <summary>
        /// @author: duynn
        /// @description: lịch công tác của cán bộ trong ngày
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [ActionAudit]
        [HttpPost]
        public PartialViewResult LichCongTacCanBoTrongNgay(long userId, DateTime date)
        {
            AssignUserInfo();
            lichCongTacBusiness = Get<LICHCONGTACBusiness>();
            LichCongTacByDayViewModel viewModel = new LichCongTacByDayViewModel();
            viewModel.startDate = new DateTime(date.Year, date.Month, date.Day);
            viewModel.currentUserId = currentUser.ID;
            viewModel.title = "LỊCH CÔNG TÁC CỦA CÁN BỘ TRONG TUẦN";
            viewModel.groupDateEntities = lichCongTacBusiness.GetLichCongTacOfUserByWeek(userId, date)
                .GroupBy(x => x.NGAY_CONGTAC).OrderBy(x => x.Key)
                    .Select(x => new LichCongTacDateEntity()
                    {
                        date = x.Key,
                        title = x.Key.GetNameOfDay() + " (" + x.Key.ToVietnameseDateFormat() + ")",
                        groupOfCalendars = x.OrderBy(y => y.GIO_CONGTAC).ThenBy(y => y.PHUT_CONGTAC).ToList()
                    }).ToList();
            return PartialView("_LichCongTacOfUserByDay", viewModel);
        }

        /// <summary>
        /// @author: duynn
        /// @description: hàm generate nội dung email lấy từ trong 
        /// </summary>
        /// <param name="calendars"></param>
        /// <returns></returns>
        private string GenerateLichCongTacEmailContent(List<LICHCONGTAC_BO> calendars)
        {
            string result = string.Empty;
            lichCongTacBusiness = Get<LICHCONGTACBusiness>();

            var groupDateEntities = calendars
                    .GroupBy(x => x.NGAY_CONGTAC).OrderBy(x => x.Key)
                    .Select(x => new LichCongTacDateEntity()
                    {
                        date = x.Key,
                        groupOfCalendars = x.OrderBy(y => y.GIO_CONGTAC).ThenBy(y => y.PHUT_CONGTAC).ToList()
                    }).ToList();
            foreach (var entity in groupDateEntities)
            {
                var morningCalendars = entity.groupOfCalendars.Where(x => x.GIO_CONGTAC <= 12).ToList();
                var afternoonCalendars = entity.groupOfCalendars.Where(x => x.GIO_CONGTAC > 12).ToList();
                result += "<tr>";

                result += "<td style='text-align:center; font-weight:bold; color:#016897;width:10%;vertical-align:middle;border:1px solid #ddd'>";
                result += "<p>" + entity.date.GetNameOfDay() + "</p>";
                result += "<p>" + string.Format("{0:dd/MM}", entity.date) + "</p>";
                result += "</td>";

                result += "<td style='border:1px solid #ddd;vertical-align: top;color:#003399;width:45%;'>";
                foreach (var morningCal in morningCalendars)
                {
                    List<string> changedProperties = GetListPropertiesChangedOfCalendar(morningCal, lichCongTacBusiness);

                    if (morningCalendars.IndexOf(morningCal) != morningCalendars.Count() - 1)
                    {
                        result += "<div style ='margin-bottom:10px; border-bottom:1px solid #ddd;text-align:left;padding:10px;'>";
                    }
                    else
                    {
                        result += "<div style ='margin-bottom:10px;text-align:left;padding:0 10px;'>";
                    }
                    result += "<p>";

                    //kiểm tra cập nhật thời gian lịch công tác
                    if (changedProperties.Contains("GIO_CONGTAC") || changedProperties.Contains("PHUT_CONGTAC"))
                    {
                        result += "<strong style='color:#f7b512'>" + (morningCal.GIO_CONGTAC.ToString("D2") + "h" + morningCal.PHUT_CONGTAC.ToString("D2")) + ":&nbsp;</strong>";
                    }
                    else
                    {
                        result += "<strong>" + (morningCal.GIO_CONGTAC.ToString("D2") + "h" + morningCal.PHUT_CONGTAC.ToString("D2")) + ":&nbsp;</strong>";
                    }

                    //kiểm tra thay đổi tiêu đề lịch công tác
                    if (changedProperties.Contains("TIEUDE"))
                    {
                        result += "<span style='color:#f7b512'>" + (morningCal.TIEUDE) + "</span>";
                    }
                    else
                    {
                        result += "<span>" + (morningCal.TIEUDE) + "</span>";
                    }
                    result += "</p>";

                    result += "<p>";
                    result += "<strong>Địa điểm:&nbsp;</strong>";

                    //kiểm tra thay đổi địa điểm lịch công tác
                    if (changedProperties.Contains("DIADIEM"))
                    {
                        result += "<span style='color:#f7b512'>" + (morningCal.DIADIEM) + "</span>";
                    }
                    else
                    {
                        result += "<span>" + (morningCal.DIADIEM) + "</span>";
                    }

                    result += "</p>";

                    result += "<p>";
                    result += "<strong>Ghi chú:&nbsp;</strong>";
                    //kiểm tra thay đổi tài nguyên sử dụng
                    if (changedProperties.Contains("GHICHU"))
                    {
                        result += "<span style='color:#f7b512'>" + (morningCal.GHICHU) + "</span>";
                    }
                    else
                    {
                        result += "<span>" + (morningCal.GHICHU) + "</span>";
                    }
                    result += "</p>";
                    result += "</div>";
                }
                result += "</td>";

                result += "<td style='border:1px solid #ddd;vertical-align: top;color:#003399;width:45%;'>";
                foreach (var afternoonCal in afternoonCalendars)
                {
                    List<string> changedProperties = GetListPropertiesChangedOfCalendar(afternoonCal, lichCongTacBusiness);

                    if (afternoonCalendars.IndexOf(afternoonCal) != afternoonCalendars.Count() - 1)
                    {
                        result += "<div style ='margin-bottom:10px; border-bottom:1px solid #ddd;text-align:left;padding:10px;'>";
                    }
                    else
                    {
                        result += "<div style ='margin-bottom:10px;text-align:left;padding:0 10px;'>";
                    }

                    //kiểm tra cập nhật thời gian lịch công tác
                    if (changedProperties.Contains("GIO_CONGTAC") || changedProperties.Contains("PHUT_CONGTAC"))
                    {
                        result += "<strong style='color:#f7b512'>" + (afternoonCal.GIO_CONGTAC.ToString("D2") + "h" + afternoonCal.PHUT_CONGTAC.ToString("D2")) + ":&nbsp;</strong>";
                    }
                    else
                    {
                        result += "<strong>" + (afternoonCal.GIO_CONGTAC.ToString("D2") + "h" + afternoonCal.PHUT_CONGTAC.ToString("D2")) + ":&nbsp;</strong>";
                    }

                    //kiểm tra thay đổi tiêu đề lịch công tác
                    if (changedProperties.Contains("TIEUDE"))
                    {
                        result += "<span style='color:#f7b512'>" + (afternoonCal.TIEUDE) + "</span>";
                    }
                    else
                    {
                        result += "<span>" + (afternoonCal.TIEUDE) + "</span>";
                    }
                    result += "</p>";

                    result += "<p>";
                    result += "<strong>Địa điểm:&nbsp;</strong>";

                    //kiểm tra thay đổi địa điểm lịch công tác
                    if (changedProperties.Contains("DIADIEM"))
                    {
                        result += "<span style='color:#f7b512'>" + (afternoonCal.DIADIEM) + "</span>";
                    }
                    else
                    {
                        result += "<span>" + (afternoonCal.DIADIEM) + "</span>";
                    }

                    result += "</p>";

                    result += "<p>";
                    result += "<strong>Ghi chú:&nbsp;</strong>";
                    //kiểm tra thay đổi tài nguyên sử dụng
                    if (changedProperties.Contains("GHICHU"))
                    {
                        result += "<span style='color:#f7b512'>" + (afternoonCal.GHICHU) + "</span>";
                    }
                    else
                    {
                        result += "<span>" + (afternoonCal.GHICHU) + "</span>";
                    }
                    result += "</p>";
                    result += "</div>";
                }
                result += "</td>";
                result += "</tr>";
            }
            return result;
        }

        private string GenerateLichCongTacDeleteEmailContent(List<LICHCONGTAC_BO> calendars)
        {
            string result = string.Empty;
            lichCongTacBusiness = Get<LICHCONGTACBusiness>();

            var groupDateEntities = calendars
                    .GroupBy(x => x.NGAY_CONGTAC).OrderBy(x => x.Key)
                    .Select(x => new LichCongTacDateEntity()
                    {
                        date = x.Key,
                        groupOfCalendars = x.OrderBy(y => y.GIO_CONGTAC).ThenBy(y => y.PHUT_CONGTAC).ToList()
                    }).ToList();
            if (groupDateEntities.Any())
            {
                result += "<table style = 'width:100%;font-size:13px;border-color:#ddd;border-collapse: collapse' border = '1'>";
                result += "<thead>";
                result += "<tr style ='font-weight:bold;'>";
                result += "<th style ='width:10%;border: 1px solid #ddd'>Ngày</th>";
                result += "<th style ='border:1px solid #ddd'> Sáng </th>";
                result += "<th style ='border:1px solid #ddd'> Chiều </th>";
                result += "</tr>";
                result += "</thead>";
                result += "<tbody>";
                foreach (var entity in groupDateEntities)
                {
                    var morningCalendars = entity.groupOfCalendars.Where(x => x.GIO_CONGTAC <= 12).ToList();
                    var afternoonCalendars = entity.groupOfCalendars.Where(x => x.GIO_CONGTAC > 12).ToList();
                    result += "<tr>";

                    result += "<td style='text-align:center; font-weight:bold; color:#FE1A1A;width:10%;vertical-align:middle;border:1px solid #ddd'>";
                    result += "<p>" + entity.date.GetNameOfDay() + "</p>";
                    result += "<p>" + string.Format("{0:dd/MM}", entity.date) + "</p>";
                    result += "</td>";

                    result += "<td style='border:1px solid #ddd;vertical-align: top;color:#FE1A1A;width:45%;'>";
                    foreach (var morningCal in morningCalendars)
                    {
                        List<string> changedProperties = GetListPropertiesChangedOfCalendar(morningCal, lichCongTacBusiness);

                        if (morningCalendars.IndexOf(morningCal) != morningCalendars.Count() - 1)
                        {
                            result += "<div style ='margin-bottom:10px; border-bottom:1px solid #ddd;text-align:left;padding:10px;'>";
                        }
                        else
                        {
                            result += "<div style ='margin-bottom:10px;text-align:left;padding:0 10px;'>";
                        }
                        result += "<p>";

                        result += "<strong>" + (morningCal.GIO_CONGTAC.ToString("D2") + "h" + morningCal.PHUT_CONGTAC.ToString("D2")) + ":&nbsp;</strong>";
                        result += "<span>" + (morningCal.TIEUDE) + "</span>";
                        result += "</p>";

                        result += "<p>";
                        result += "<strong>Địa điểm:&nbsp;</strong>";
                        result += "<span>" + (morningCal.DIADIEM) + "</span>";
                        result += "</p>";

                        result += "<p>";
                        result += "<strong>Ghi chú:&nbsp;</strong>";
                        result += "<span>" + (morningCal.GHICHU) + "</span>";
                        result += "</p>";
                        result += "</div>";
                    }
                    result += "</td>";

                    result += "<td style='border:1px solid #ddd;vertical-align: top;color:#FE1A1A;width:45%;'>";
                    foreach (var afternoonCal in afternoonCalendars)
                    {
                        List<string> changedProperties = GetListPropertiesChangedOfCalendar(afternoonCal, lichCongTacBusiness);

                        if (afternoonCalendars.IndexOf(afternoonCal) != afternoonCalendars.Count() - 1)
                        {
                            result += "<div style ='margin-bottom:10px; border-bottom:1px solid #ddd;text-align:left;padding:10px;'>";
                        }
                        else
                        {
                            result += "<div style ='margin-bottom:10px;text-align:left;padding:0 10px;'>";
                        }

                        result += "<strong>" + (afternoonCal.GIO_CONGTAC.ToString("D2") + "h" + afternoonCal.PHUT_CONGTAC.ToString("D2")) + ":&nbsp;</strong>";
                        result += "<span>" + (afternoonCal.TIEUDE) + "</span>";
                        result += "</p>";

                        result += "<p>";
                        result += "<strong>Địa điểm:&nbsp;</strong>";

                        result += "<span>" + (afternoonCal.DIADIEM) + "</span>";
                        result += "</p>";

                        result += "<p>";
                        result += "<strong>Tài nguyên sử dụng:&nbsp;</strong>";
                        //result += "<span>" + (afternoonCal.TAINGUYEN_SUDUNG) + "</span>";
                        result += "</p>";
                        result += "</div>";
                    }
                    result += "</td>";
                    result += "</tr>";
                }
                result += "</tbody>";
                result += "</table>";
            }
            return result;
        }

        public List<string> GetListPropertiesChangedOfCalendar(LICHCONGTAC_BO calendar, LICHCONGTACBusiness lichCongTacBusiness)
        {
            List<string> result = new List<string>();
            Type entityType = typeof(LICHCONGTAC);
            if (calendar.LICH_GOC_ID != null)
            {
                LICHCONGTAC oldEntity = lichCongTacBusiness.Find(calendar.LICH_GOC_ID);
                LICHCONGTAC newEntity = lichCongTacBusiness.Find(calendar.ID);
                if (oldEntity != null && newEntity != null)
                {
                    foreach (string propertyName in arrProperties)
                    {
                        PropertyInfo propertyInfo = entityType.GetProperty(propertyName);
                        var oldValue = propertyInfo.GetValue(oldEntity);
                        var newValue = propertyInfo.GetValue(newEntity);
                        if (oldValue != newValue)
                        {
                            result.Add(propertyName);
                        }
                    }
                }
            }
            return result;

        }

        /// <summary>
        /// @author: duynn
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionAudit]
        public PartialViewResult SendEmail(long userId = 0)
        {
            AssignUserInfo();
            dmNguoiDungBusiness = Get<DM_NGUOIDUNGBusiness>();
            LichCongTacEmailViewModel viewModel = new LichCongTacEmailViewModel();
            viewModel.groupOfUsers = dmNguoiDungBusiness.GetDropDownByDeptParentId(currentUser.DeptParentID.GetValueOrDefault(), 0);
            return PartialView("_SendEmailLichCongTac", viewModel);
        }

        /// <summary>
        /// @author: duynn
        /// @description: gửi email
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionAudit]
        public JsonResult SendEmail(FormCollection fc)
        {
            JsonResultBO result = new JsonResultBO(false);
            lichCongTacBusiness = Get<LICHCONGTACBusiness>();
            dmNguoiDungBusiness = Get<DM_NGUOIDUNGBusiness>();

            DateTime currentDate = DateTime.Now;
            var startOfWeek = currentDate.GetStartDayOfWeek();
            var endOfWeek = currentDate.GetEndDayOfWeek();
            LICHCONGTAC_SEARCH searchModel = new LICHCONGTAC_SEARCH();
            searchModel.startDate = startOfWeek;
            searchModel.endDate = endOfWeek;
            List<long> userIds = fc["LANHDAO_ID"].ToListLong(',');
            string subject = fc["TIEUDE"].Trim();

            string emailTemplatePath = Path.Combine(ConfigurationManager.AppSettings["FileUpload"], ConfigurationManager.AppSettings["LichCongTacEmailTemplate"]);
            string emailContent = System.IO.File.ReadAllText(emailTemplatePath);

            foreach (var item in userIds)
            {
                DM_NGUOIDUNG user = dmNguoiDungBusiness.Find(item);
                if (user != null && string.IsNullOrEmpty(user.EMAIL) == false)
                {
                    searchModel.leaderId = user.ID;
                    List<LICHCONGTAC_BO> calendars = lichCongTacBusiness.GetListLichCongTacs(searchModel);
                    List<LICHCONGTAC_BO> deletedCalendars = lichCongTacBusiness.GetListDeletedLichCongTacs(searchModel);
                    emailContent = emailContent.Replace("[[recipient]]", user.HOTEN);
                    emailContent = emailContent.Replace("[[week_header_title]]", currentDate.GetWeekNumber() + " (từ ngày " + startOfWeek.ToVietnameseDateFormat() + " đến ngày " + endOfWeek.ToVietnameseDateFormat() + ")");
                    emailContent = emailContent.Replace("[[week_title]]", currentDate.GetNameOfWeek() + " năm " + currentDate.Year);
                    emailContent = emailContent.Replace("[[week_start_end_date_title]]", currentDate.GetNameOfWeek() +
                        " (" + startOfWeek.GetNameOfDay() + ", "
                        + startOfWeek.ToVietnameseDateFormat() + " - "
                        + endOfWeek.GetNameOfDay() + ", "
                        + endOfWeek.ToVietnameseDateFormat() + ")");
                    emailContent = emailContent.Replace("[[email_main_content]]", this.GenerateLichCongTacEmailContent(calendars));
                    emailContent = emailContent.Replace("[[email_error_content]]", this.GenerateLichCongTacDeleteEmailContent(deletedCalendars));
                    List<string> address = new List<string>() { user.EMAIL };
                    EmailProvider.sendEmail(emailContent, subject, address);
                }
            }
            result.Status = true;
            return Json(result);
        }


        /// <summary>
        /// @author: duynn
        /// @since: 31/08/2018
        /// @description: lấy thông tin lịch công tác để đăng ký xe
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionAudit]
        public PartialViewResult GetCalendarToRegisterCar(FormCollection collection)
        {
            lichCongTacBusiness = Get<LICHCONGTACBusiness>();
            LICHCONGTAC_SEARCH searchModel = new LICHCONGTAC_SEARCH();
            searchModel.leaderId = collection["CAL_LANHDAO_ID"].ToIntOrNULL();
            searchModel.startDate = collection["CAL_NGAYBATDAU"].ToDateTime();
            searchModel.endDate = collection["CAL_NGAYKETTHUC"].ToDateTime();
            LichCongTacByDayViewModel viewModel = new LichCongTacByDayViewModel();
            viewModel.groupDateEntities = lichCongTacBusiness.GetListLichCongTacs(searchModel)
                .GroupBy(x => x.NGAY_CONGTAC).OrderBy(x => x.Key)
                .Select(x => new LichCongTacDateEntity()
                {
                    title = x.Key.GetNameOfDay() + " (" + x.Key.ToVietnameseDateFormat() + ")",
                    groupOfCalendars = x.OrderBy(y => y.GIO_CONGTAC).ThenBy(y => y.PHUT_CONGTAC).ToList()
                }).ToList();
            return PartialView("_ListLichCongTacRegisterCar", viewModel);
        }

        /// <summary>
        /// @author: duynn
        /// @description: lấy thông tin lịch công tác để tạo đăng ký xe
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionAudit]
        public JsonResult GetInfoLichCongTac(long id)
        {
            lichCongTacBusiness = Get<LICHCONGTACBusiness>();
            LICHCONGTAC calendar = lichCongTacBusiness.Find(id);
            if (calendar != null && calendar.IS_DELETE != true)
            {
                return Json(calendar);
            }
            return Json(new LICHCONGTAC());
        }

        /// <summary>
        /// @author: duynn
        /// @description: tạo lịch công tác trong ngày
        /// @since: 04/09/2018
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionAudit]
        public ActionResult CreateCalendarOfTheDay(int year, int month, int day, int hour = 0, int minute = 0)
        {
            LICHCONGTAC entityCalendar = new LICHCONGTAC();
            entityCalendar.NGAY_CONGTAC = new DateTime(year, month, day);
            entityCalendar.GIO_CONGTAC = hour;
            entityCalendar.PHUT_CONGTAC = minute;
            SessionManager.SetValue("CreateCalendarOfTheDay", entityCalendar);
            return RedirectToAction("CreateLichCongTac");
        }
    }
}