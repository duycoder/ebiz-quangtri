using Business.Business;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.QLDANGKYXE;
using CommonHelper;
using CommonHelper.DateExtend;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Web.Areas.QL_DANGKY_XEArea.Models;
using Web.Custom;
using Web.Filter;
using Web.FwCore;

namespace Web.Areas.QL_DANGKY_XEArea.Controllers
{
    public class QL_DANGKY_XEController : BaseController
    {
        private static string DEFAULT_START_POINT = WebConfigurationManager.AppSettings["DEFAULT_START_POINT"];

        private QL_DANGKY_XEBusiness qlDangKyXeBusiness;
        private QL_XEBusiness qlXeBusiness;
        private QL_LAIXEBusiness qlLaiXeBusiness;
        private QL_DANGKYXE_LAIXEBusiness qlChuyenBusiness;
        private DM_NGUOIDUNGBusiness dmNguoiDungBusiness;
        private LICHCONGTACBusiness lichCongTacBusiness;
        private DM_DANHMUC_DATABusiness dmDanhMucDataBusiness;
        private DM_NHOMDANHMUCBusiness dmNhomDanhMucBusiness;
        private SYS_TINNHANBusiness sysTinNhanBusiness;
        private const string CONFIRM_CAR_REGISTRATION = "XACNHAN_DANGKYXE";
        // GET: QL_DANGKY_XEArea/QL_DANGKY_XE

        [ActionAudit]
        public ActionResult Index()
        {
            AssignUserInfo();
            bool canReceiveRegistration = currentUser.ListThaoTac
                    .Where(x => x.MA_THAOTAC == CONFIRM_CAR_REGISTRATION).FirstOrDefault() != null;
            qlDangKyXeBusiness = Get<QL_DANGKY_XEBusiness>();
            DangKyXeIndexViewModel viewModel = new DangKyXeIndexViewModel();
            DangKyXeSearchBO searchModel = new DangKyXeSearchBO();
            searchModel.CCTC_THANHPHAN_ID = currentUser.DeptParentID.GetValueOrDefault();
            if (canReceiveRegistration)
            {
                searchModel.TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.MOITAO_ID;
                searchModel.USER_ID = currentUser.ID;
                searchModel.HAS_ROLE_CONFIRM = true;
                viewModel.canReceiveRegistration = canReceiveRegistration;
            }
            viewModel.currentUserId = currentUser.ID;
            viewModel.listDangKyXeBenhViens = qlDangKyXeBusiness.GetDataByPage(searchModel);
            SessionManager.SetValue("SearchDangKyXeBenhVien", searchModel);
            return View(viewModel);
        }

        [ActionAudit]
        public JsonResult GetData(int pageIndex, string sortQuery, int pageSize)
        {
            AssignUserInfo();
            bool canReceiveRegistration = currentUser.ListThaoTac
                    .Where(x => x.MA_THAOTAC == CONFIRM_CAR_REGISTRATION).FirstOrDefault() != null;
            qlDangKyXeBusiness = Get<QL_DANGKY_XEBusiness>();
            DangKyXeSearchBO searchModel = (DangKyXeSearchBO)SessionManager.GetValue("SearchDangKyXeBenhVien");
            if (searchModel == null)
            {
                searchModel = new DangKyXeSearchBO();
            }
            if (canReceiveRegistration)
            {
                searchModel.TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.MOITAO_ID;
                searchModel.USER_ID = currentUser.ID;
                searchModel.HAS_ROLE_CONFIRM = true;
            }
            searchModel.sortQuery = sortQuery;
            searchModel.CCTC_THANHPHAN_ID = currentUser.DeptParentID.GetValueOrDefault();
            PageListResultBO<DangKyXeBO> data = qlDangKyXeBusiness.GetDataByPage(searchModel);
            return Json(data);
        }

        [ActionAudit]
        [HttpPost]
        public JsonResult SearchData(FormCollection fc)
        {
            AssignUserInfo();
            bool canReceiveRegistration = currentUser.ListThaoTac
                    .Where(x => x.MA_THAOTAC == CONFIRM_CAR_REGISTRATION).FirstOrDefault() != null;

            qlDangKyXeBusiness = Get<QL_DANGKY_XEBusiness>();
            var searchModel = (DangKyXeSearchBO)SessionManager.GetValue("SearchDangKyXeBenhVien");
            if (canReceiveRegistration)
            {
                searchModel.TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.MOITAO_ID;
                searchModel.USER_ID = currentUser.ID;
                searchModel.HAS_ROLE_CONFIRM = true;
            }
            searchModel.queryThoiGianXuatPhatStart = fc["queryThoiGianXuatPhatStart"].ToDateTime();
            searchModel.queryThoiGianXuatPhatEnd = fc["queryThoiGianXuatPhatEnd"].ToDateTime();
            searchModel.querySoNguoiStart = fc["querySoNguoiStart"].ToIntOrNULL();
            searchModel.querySoNguoiEnd = fc["querySoNguoiEnd"].ToIntOrNULL();
            searchModel.MUCDICH = fc["MUCDICH"].Trim();
            searchModel.CCTC_THANHPHAN_ID = currentUser.DeptParentID.GetValueOrDefault();
            SessionManager.SetValue("SearchDangKyXeBenhVien", searchModel);
            var result = qlDangKyXeBusiness.GetDataByPage(searchModel);
            return Json(result);
        }

        // GET: QL_DANGKY_XEArea/QL_DANGKY_XE/Details/5
        [ActionAudit]
        public ActionResult Details(long id)
        {
            AssignUserInfo();
            qlDangKyXeBusiness = Get<QL_DANGKY_XEBusiness>();
            lichCongTacBusiness = Get<LICHCONGTACBusiness>();
            DangKyXeBO entity = qlDangKyXeBusiness.GetDetail(id);
            if (entity != null && entity.CCTC_THANHPHAN_ID == currentUser.DeptParentID.GetValueOrDefault())
            {
                DangKyXeDetailViewModel viewModel = new DangKyXeDetailViewModel();

                viewModel.canSendRegistration = entity.TRANGTHAI == TRANGTHAI_DANGKY_XE_CONSTANT.MOITAO_ID
                    && entity.NGUOITAO == currentUser.ID;
                viewModel.canRecieveRegistratiion = currentUser.ListThaoTac
                    .Where(x => x.MA_THAOTAC == CONFIRM_CAR_REGISTRATION).FirstOrDefault() != null
                        && entity.TRANGTHAI == TRANGTHAI_DANGKY_XE_CONSTANT.DAGUI_ID;
                viewModel.currentUserId = currentUser.ID;
                viewModel.entity = entity;
                viewModel.entityCalendar = lichCongTacBusiness.repository.All()
                    .Where(x => x.IS_DELETE != true && x.IS_LATTEST == true && x.ID == entity.LICHCONGTAC_ID)
                    .FirstOrDefault();
                return View(viewModel);
            }
            return Redirect("/Home/UnAuthor");
        }

        // GET: QL_DANGKY_XEArea/QL_DANGKY_XE/Create
        [ActionAudit]
        public ActionResult Create(long calendarId = 0)
        {
            AssignUserInfo();
            dmNguoiDungBusiness = Get<DM_NGUOIDUNGBusiness>();
            lichCongTacBusiness = Get<LICHCONGTACBusiness>();
            qlDangKyXeBusiness = Get<QL_DANGKY_XEBusiness>();
            dmDanhMucDataBusiness = Get<DM_DANHMUC_DATABusiness>();

            DangKyXeEditViewModel model = new DangKyXeEditViewModel();
            model.dangKyXeEntity.DIEM_XUATPHAT = DEFAULT_START_POINT;
            model.groupOfLanhDaos = dmNguoiDungBusiness.GetListUserByDeptParentId(currentUser.DeptParentID.GetValueOrDefault(),0);
            if (calendarId != 0)
            {
                LICHCONGTAC calendar = lichCongTacBusiness.Find(calendarId);
                //kiểm tra lịch công tác chưa bị xóa - mới nhất - chưa được đăng ký
                if (calendar != null && calendar.IS_DELETE != true && calendar.IS_LATTEST == true)
                {
                    QL_DANGKY_XE registration = qlDangKyXeBusiness.GetAvailableRegistrationByCalendarId(calendarId);
                    if (registration == null)
                    {
                        registration = new QL_DANGKY_XE();
                        registration.LICHCONGTAC_ID = calendar.ID;
                        registration.CANBO_ID = calendar.LANHDAO_ID;
                        registration.MUCDICH = calendar.TIEUDE;
                        registration.NGAY_XUATPHAT = calendar.NGAY_CONGTAC;
                        registration.GIO_XUATPHAT = calendar.GIO_CONGTAC;
                        registration.PHUT_XUATPHAT = calendar.PHUT_CONGTAC;
                        registration.DIEM_KETTHUC = calendar.DIADIEM;
                        registration.GHICHU = calendar.GHICHU;
                        model = new DangKyXeEditViewModel(registration);
                        model.groupOfLanhDaos = dmNguoiDungBusiness
                            .GetListUserByDeptParentId(currentUser.DeptParentID.GetValueOrDefault(), calendar.LANHDAO_ID.GetValueOrDefault());
                    }
                }
            }
            model.groupOfStartPoints = dmDanhMucDataBusiness.GetGroupTextByCode(DMLOAI_CONSTANT.DIEM_XUATPHAT);
            model.groupOfDestinations = dmDanhMucDataBusiness.GetGroupTextByCode(DMLOAI_CONSTANT.DIEM_DEN);
            return View("EditDangKyXe", model);
        }

        // POST: QL_DANGKY_XEArea/QL_DANGKY_XE/Create
        [HttpPost]
        [ActionAudit]
        public JsonResult SaveDangKyXe(FormCollection collection)
        {
            qlDangKyXeBusiness = Get<QL_DANGKY_XEBusiness>();
            dmNhomDanhMucBusiness = Get<DM_NHOMDANHMUCBusiness>();
            dmDanhMucDataBusiness = Get<DM_DANHMUC_DATABusiness>();

            JsonResultBO result = new JsonResultBO(true);
            try
            {
                AssignUserInfo();
                long id = collection["ID"].ToLongOrZero();
                QL_DANGKY_XE entity = new QL_DANGKY_XE();
                entity.NGAY_XUATPHAT = collection["NGAY_XUATPHAT"].ToDateTime();
                //kiểm tra ngày xuất phát <= ngày hiện tại
                if(entity.ID == 0 && entity.NGAY_XUATPHAT.HasValue && entity.NGAY_XUATPHAT.Value < DateTime.Now.ToVietnameseDateFormat().ToStartDay())
                {
                    result.Status = false;
                    result.Message = "Thời gian xuất phát không thể nhỏ hơn ngày hiện tại";
                    return Json(result);
                }

                entity.PHONGBAN_ID = currentUser.DM_PHONGBAN_ID;
                entity.SONGUOI = collection["SONGUOI"].ToIntOrZero();
                entity.MUCDICH = collection["MUCDICH"].Trim();
                entity.NOIDUNG = collection["NOIDUNG"].Trim();
                entity.GIO_XUATPHAT = collection["GIO_XUATPHAT"].ToIntOrZero();
                entity.PHUT_XUATPHAT = collection["PHUT_XUATPHAT"].ToIntOrZero();
                entity.DIEM_XUATPHAT = collection["DIEM_XUATPHAT"].Trim();
                entity.DIEM_KETTHUC = collection["DIEM_KETTHUC"].Trim();
                entity.GHICHU = collection["GHICHU"].Trim();
                entity.TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.MOITAO_ID;
                entity.LICHCONGTAC_ID = collection["LICHCONGTAC_ID"].ToIntOrNULL();
                entity.CCTC_THANHPHAN_ID = currentUser.DeptParentID.GetValueOrDefault();
                //if (!string.IsNullOrEmpty(collection["TEN_BENHNHAN"]))
                //{
                //    entity.TEN_BENHNHAN = collection["TEN_BENHNHAN"].Trim();
                //}
                if (!string.IsNullOrEmpty(collection["CANBO_ID"]))
                {
                    entity.CANBO_ID = collection["CANBO_ID"].ToIntOrNULL();
                }
                //if (!string.IsNullOrEmpty(collection["IS_BHYT"]))
                //{
                //    entity.IS_BHYT = collection["IS_BHYT"].ToIntOrNULL() > 0;
                //}

                //if (!string.IsNullOrEmpty(collection["LOAI_CHUYEN_ID"]))
                //{
                //    entity.LOAI_CHUYEN_ID = collection["LOAI_CHUYEN_ID"].ToIntOrZero();
                //}

                entity.DIEM_XUATPHAT = collection["DIEM_XUATPHAT"].Trim();
                entity.DIEM_KETTHUC = collection["DIEM_KETTHUC"].Trim();

                DM_DANHMUC_DATA dataStartPointItem = dmDanhMucDataBusiness.GetItemByCodeAndText(DMLOAI_CONSTANT.DIEM_XUATPHAT, entity.DIEM_XUATPHAT);
                if (dataStartPointItem == null)
                {
                    DM_NHOMDANHMUC groupCategory = dmNhomDanhMucBusiness.GetByCode(DMLOAI_CONSTANT.DIEM_XUATPHAT) ?? new DM_NHOMDANHMUC();
                    DM_DANHMUC_DATA startPointEntity = new DM_DANHMUC_DATA();
                    startPointEntity.DM_NHOM_ID = groupCategory.ID;
                    startPointEntity.TEXT = entity.DIEM_XUATPHAT;
                    dmDanhMucDataBusiness.Save(startPointEntity);
                }

                DM_DANHMUC_DATA dataDestinationItem = dmDanhMucDataBusiness.GetItemByCodeAndText(DMLOAI_CONSTANT.DIEM_DEN, entity.DIEM_KETTHUC);
                if (dataDestinationItem == null)
                {
                    DM_NHOMDANHMUC groupCategory = dmNhomDanhMucBusiness.GetByCode(DMLOAI_CONSTANT.DIEM_DEN) ?? new DM_NHOMDANHMUC();
                    DM_DANHMUC_DATA destinationEntity = new DM_DANHMUC_DATA();
                    destinationEntity.DM_NHOM_ID = groupCategory.ID;
                    destinationEntity.TEXT = entity.DIEM_KETTHUC;
                    dmDanhMucDataBusiness.Save(destinationEntity);
                }
                entity.NGAYSUA = DateTime.Now;
                entity.NGAYTAO = DateTime.Now;
                entity.NGUOITAO = currentUser.ID;
                entity.NGUOISUA = currentUser.ID;
                if (id > 0)
                {
                    QL_DANGKY_XE dbEntity = qlDangKyXeBusiness.Find(id);
                    if (dbEntity != null)
                    {
                        dbEntity.PHONGBAN_ID = entity.PHONGBAN_ID;
                        dbEntity.SONGUOI = entity.SONGUOI;
                        dbEntity.MUCDICH = entity.MUCDICH;
                        dbEntity.NOIDUNG = entity.NOIDUNG;
                        dbEntity.NGAY_XUATPHAT = entity.NGAY_XUATPHAT;
                        dbEntity.GIO_XUATPHAT = entity.GIO_XUATPHAT;
                        dbEntity.PHUT_XUATPHAT = entity.PHUT_XUATPHAT;
                        dbEntity.DIEM_XUATPHAT = entity.DIEM_XUATPHAT;
                        dbEntity.DIEM_KETTHUC = entity.DIEM_KETTHUC;
                        dbEntity.CCTC_THANHPHAN_ID = entity.CCTC_THANHPHAN_ID;
                        dbEntity.GHICHU = entity.GHICHU;
                        dbEntity.NGAYSUA = DateTime.Now;
                        dbEntity.NGUOISUA = currentUser.ID;
                        qlDangKyXeBusiness.Save(dbEntity);
                        result.Message = "Cập nhật đăng ký xe thành công";
                    }
                }
                else
                {
                    qlDangKyXeBusiness.Save(entity);
                    result.Message = "Thêm mới đăng ký xe thành công";
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = ex.ToString();
            }
            return Json(result);
        }

        // GET: QL_DANGKY_XEArea/QL_DANGKY_XE/Edit/5
        [ActionAudit]
        public ActionResult Edit(long id)
        {
            AssignUserInfo();

            dmNguoiDungBusiness = Get<DM_NGUOIDUNGBusiness>();
            qlDangKyXeBusiness = Get<QL_DANGKY_XEBusiness>();
            QL_DANGKY_XE register = qlDangKyXeBusiness.Find(id) ?? new QL_DANGKY_XE();
            DangKyXeEditViewModel model = new DangKyXeEditViewModel(register);
            model.groupOfLanhDaos = dmNguoiDungBusiness.GetDropDownByDeptParentId(currentUser.DeptParentID.GetValueOrDefault(), register.CANBO_ID.GetValueOrDefault());
            return View("EditDangKyXe", model);
        }

        // POST: QL_DANGKY_XEArea/QL_DANGKY_XE/Delete/5
        [HttpPost]
        [ActionAudit]
        public ActionResult Delete(long id)
        {
            qlDangKyXeBusiness = Get<QL_DANGKY_XEBusiness>();
            JsonResultBO result = new JsonResultBO(true);
            QL_DANGKY_XE dbEntity = qlDangKyXeBusiness.Find(id);
            if (dbEntity != null)
            {
                dbEntity.IS_DELETE = true;
                qlDangKyXeBusiness.Save(dbEntity);
            }
            else
            {
                result.Status = false;
                result.Message = "Không tìm thấy đăng ký xe";
            }
            return Json(result);
        }

        /// <summary>
        /// @author: duynn
        /// @description: màn hình hủy tiếp nhận yêu cầu đăng ký xe
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionAudit]
        public PartialViewResult ConfirmRejectCarRegistration(long registrationId)
        {
            qlDangKyXeBusiness = Get<QL_DANGKY_XEBusiness>();
            QL_DANGKY_XE registration = qlDangKyXeBusiness.Find(registrationId);
            return PartialView("_ConfirmRejectCarRegistration", registration);
        }

        /// <summary>
        /// @author: duynn
        /// @description: màn hình chấp nhận đăng ký xe
        /// </summary>
        /// <param name="regitrationId"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionAudit]
        public PartialViewResult ConfirmCarRegistration(long registrationId)
        {
            qlDangKyXeBusiness = Get<QL_DANGKY_XEBusiness>();
            qlXeBusiness = Get<QL_XEBusiness>();
            qlLaiXeBusiness = Get<QL_LAIXEBusiness>();

            QL_DANGKY_XE carRegister = qlDangKyXeBusiness.Find(registrationId) ?? new QL_DANGKY_XE();
            DangKyXeEditViewModel viewModel = new DangKyXeEditViewModel(carRegister);
            viewModel.groupOfCars = qlXeBusiness.GetDropDownAvailableCarsForTrip(carRegister.ID,0);
            viewModel.groupOfDrivers = qlLaiXeBusiness.GetDropDownAvailableDriversForTrip(carRegister.ID);
            return PartialView("_ConfirmCarRegistration", viewModel);
        }

        /// <summary>
        /// @author: duynn
        /// @description: tiếp nhận yêu cầu đăng ký xe
        /// @since: 28/08/2018
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionAudit]
        public JsonResult AcceptCarRegistration(FormCollection fc)
        {
            AssignUserInfo();
            JsonResultBO result = new JsonResultBO(false);
            qlChuyenBusiness = Get<QL_DANGKYXE_LAIXEBusiness>();
            qlDangKyXeBusiness = Get<QL_DANGKY_XEBusiness>();
            qlLaiXeBusiness = Get<QL_LAIXEBusiness>();
            sysTinNhanBusiness = Get<SYS_TINNHANBusiness>();
            dmNguoiDungBusiness = Get<DM_NGUOIDUNGBusiness>();
            try
            {
                long registrationId = fc["DANGKY_XE_ID"].ToIntOrZero();
                QL_DANGKY_XE registration = qlDangKyXeBusiness.Find(registrationId);

                if (registration != null)
                {
                    List<int> cars = fc["XE_ID"].ToListInt(',');
                    List<int> drivers = fc["LAIXE_ID"].ToListInt(',');

                    for (int i = 0; i < cars.Count; i++)
                    {
                        QL_LAIXE driver = qlLaiXeBusiness.Find(drivers[i]) ?? new QL_LAIXE();
                        QL_DANGKYXE_LAIXE item = new QL_DANGKYXE_LAIXE();
                        item.CCTC_THANHPHAN_ID = currentUser.DeptParentID.GetValueOrDefault();
                        item.TEN_CHUYEN = "Chuyến " + registrationId + "-" + driver.HOTEN;
                        item.QL_DANGKY_XE_ID = registrationId;
                        item.XE_ID = cars[i];
                        item.LAIXE_ID = drivers[i];
                        item.TRANGTHAI = TRANGTHAI_CHUYEN_CONSTANT.MOITAO_ID;
                        item.GHICHU = fc["GHICHU"].Trim();
                        item.NGAYTAO = DateTime.Now;
                        item.NGAYSUA = DateTime.Now;
                        item.NGUOISUA = currentUser.ID;
                        item.NGUOITAO = currentUser.ID;
                        qlChuyenBusiness.Save(item);
                    }

                    registration.TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DA_TIEPNHAN_ID;
                    qlDangKyXeBusiness.Save(registration);

                    result.Status = true;
                    result.Message = "Tiếp nhận yêu cầu thành công";

                    //gửi tin nhắn cho người tạo ra yêu cầu
                    //gửi tin nhắn cho người tạo yêu cầu
                    List<long> notifyUsers = new List<long>() { registration.NGUOITAO.GetValueOrDefault() };
                    string title = "TIẾP NHẬN YÊU CẦU SỬ DỤNG XE";
                    string content = string.Format("{0} đã tiếp nhận một yêu cầu sử dụng xe", currentUser.HOTEN);

                    sysTinNhanBusiness.sendMessageMultipleUsers(notifyUsers, currentUser, title, content, Url.Action("Details", new { id = registrationId }), string.Empty, false, registrationId, 0);
                }
                else
                {
                    result.Message = "Không tìm thấy yêu cầu đăng ký xe";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result);
        }

        /// <summary>
        /// @author: duynn
        /// @description: không tiếp nhận yêu cầu đăng ký đăng ký xe
        /// @since: 28/08/2018
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionAudit]
        public JsonResult RejectCarRegistration(FormCollection fc)
        {
            AssignUserInfo();
            JsonResultBO result = new JsonResultBO(false);
            qlDangKyXeBusiness = Get<QL_DANGKY_XEBusiness>();
            dmNguoiDungBusiness = Get<DM_NGUOIDUNGBusiness>();
            sysTinNhanBusiness = Get<SYS_TINNHANBusiness>();

            long id = fc["ID"].ToLongOrZero();
            QL_DANGKY_XE registration = qlDangKyXeBusiness.Find(id);
            if (registration != null && registration.IS_DELETE != true)
            {
                registration.TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.MOITAO_ID;
                registration.LYDO_TUCHOI = fc["LYDO_TUCHOI"].Trim();
                qlDangKyXeBusiness.Save(registration);
                result.Status = true;
                result.Message = "Đã hủy tiếp nhận yêu cầu đăng ký xe";

                //gửi tin nhắn cho người tạo yêu cầu
                List<long> notifyUsers = new List<long>() { registration.NGUOITAO.GetValueOrDefault() };
                string title = "KHÔNG TIẾP NHẬN YÊU CẦU SỬ DỤNG XE";
                string content = string.Format("{0} đã từ chối tiếp nhận một yêu cầu sử dụng xe", currentUser.HOTEN);

                sysTinNhanBusiness.sendMessageMultipleUsers(notifyUsers, currentUser, title, content, Url.Action("Details", new { id = id }), string.Empty, false, id, 0);
            }
            return Json(result);
        }

        /// <summary>
        /// @author: duynn
        /// @description: gửi yêu cầu đăng ký xe
        /// @since: 28/08/2018
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionAudit]
        public JsonResult SendCarRegistration(long registrationId)
        {
            AssignUserInfo();
            JsonResultBO result = new JsonResultBO(false);
            qlDangKyXeBusiness = Get<QL_DANGKY_XEBusiness>();
            dmNguoiDungBusiness = Get<DM_NGUOIDUNGBusiness>();
            sysTinNhanBusiness = Get<SYS_TINNHANBusiness>();

            QL_DANGKY_XE registration = qlDangKyXeBusiness.Find(registrationId);
            if (registration != null && registration.IS_DELETE != true)
            {
                registration.TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DAGUI_ID;
                qlDangKyXeBusiness.Save(registration);
                result.Status = true;
                result.Message = "Đã gửi yêu cầu đăng ký sử dụng xe";

                //gửi tin nhắn cho người có quyền xác nhận yêu cầu sử dụng xe
                List<long> notifyUsers = dmNguoiDungBusiness.GetListUsersByFunctionCodeAndDeptId(CONFIRM_CAR_REGISTRATION, currentUser.DeptParentID.GetValueOrDefault());
                string title = "YÊU CẦU SỬ DỤNG XE";
                string content = string.Format("{0} đã gửi một yêu cầu sử dụng xe", currentUser.HOTEN);

                sysTinNhanBusiness.sendMessageMultipleUsers(notifyUsers, currentUser, title, content, Url.Action("Details", new { id = registrationId }), string.Empty, false, registrationId, 0);
            }
            else
            {
                result.Message = "Không tìm thấy đăng ký xe";
            }
            return Json(result);
        }

        /// <summary>
        /// @author: duynn
        /// @description: hủy đăng ký
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionAudit]
        public JsonResult CancelRegistration(long registrationId)
        {
            AssignUserInfo();
            JsonResultBO result = new JsonResultBO(true);
            qlDangKyXeBusiness = Get<QL_DANGKY_XEBusiness>();
            qlChuyenBusiness = Get<QL_DANGKYXE_LAIXEBusiness>();
            dmNguoiDungBusiness = Get<DM_NGUOIDUNGBusiness>();
            sysTinNhanBusiness = Get<SYS_TINNHANBusiness>();

            QL_DANGKY_XE registration = qlDangKyXeBusiness.Find(registrationId);
            if (registration != null && registration.IS_DELETE != true)
            {
                QL_DANGKYXE_LAIXE trip = qlChuyenBusiness.repository.All().Where(x => x.QL_DANGKY_XE_ID == registration.ID).FirstOrDefault();
                if (trip == null || trip.TRANGTHAI == TRANGTHAI_CHUYEN_CONSTANT.MOITAO_ID)
                {
                    if (trip != null)
                    {
                        qlChuyenBusiness.repository.Delete(trip);
                    }
                    registration.TRANGTHAI = TRANGTHAI_DANGKY_XE_CONSTANT.DA_HUY_ID;
                    qlDangKyXeBusiness.Save(registration);
                    result.Message = "Hủy yêu cầu thành công";

                    //gửi tin nhắn cho người có quyền xác nhận yêu cầu sử dụng xe
                    List<long> notifyUsers = dmNguoiDungBusiness.GetListUsersByFunctionCodeAndDeptId(CONFIRM_CAR_REGISTRATION, currentUser.DeptParentID.GetValueOrDefault());
                    string title = "HỦY YÊU CẦU SỬ DỤNG XE";
                    string content = string.Format("{0} đã hủy một yêu cầu sử dụng xe", currentUser.HOTEN);

                    sysTinNhanBusiness.sendMessageMultipleUsers(notifyUsers, currentUser, title, content, Url.Action("Details", new { id = registrationId }), string.Empty, false, registrationId, 0);

                }
                else
                {
                    result.Status = false;
                    if (trip.TRANGTHAI == TRANGTHAI_CHUYEN_CONSTANT.DANGCHAY_ID)
                    {
                        result.Message = "Yêu cầu đang được thực thi không thể hủy";
                    }
                    else
                    {
                        result.Message = "Hủy yêu cầu không thành công";
                    }
                }
            }
            return Json(result);
        }
    }
}
