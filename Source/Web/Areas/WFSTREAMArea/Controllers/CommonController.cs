using Business.Business;
using Business.CommonBusiness;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.DMNguoiDung;
using Business.CommonModel.HSCVCONGVIEC;
using Business.CommonModel.HSCVVANBANDEN;
using Business.CommonModel.HSCVVANBANDI;
using CommonHelper;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Microsoft.Office.Interop.Word;
//using wSigner;
//using Web.Areas.CongViecArea.Models;
using Web.Common;
using Web.Common.Elastic;
using Web.Custom;
using Web.FwCore;
using Web.Models;
//using GramMultiThread.Controllers;

namespace Web.Controllers
{
    public class CommonController : BaseController
    {
        // GET: Common
        private CCTC_THANHPHANBusiness CCTC_THANHPHANBusiness;
        private DM_NGUOIDUNGBusiness DM_NGUOIDUNGBusiness;
        private string URLPath = WebConfigurationManager.AppSettings["FileUpload"];
        private TAILIEUDINHKEMBusiness TAILIEUDINHKEMBusiness;
        private SYS_TINNHANBusiness SYS_TINNHANBusiness;
        private HSCV_VANBANDIBusiness HSCV_VANBANDIBusiness;
        private HSCV_VANBANDENBusiness HSCV_VANBANDENBusiness;
        private QL_NGUOINHAN_VANBANBusiness QL_NGUOINHAN_VANBANBusiness;
        private int MaxPerpage = int.Parse(WebConfigurationManager.AppSettings["MaxPerpage"]);
        private string PROGRAMECMD = WebConfigurationManager.AppSettings["PROGRAMECMD"];
        private string CHUKYSO = WebConfigurationManager.AppSettings["CHUKYSO"];

        private HSCV_VANBANDI_DONVINHANBusiness HSCV_VANBANDI_DONVINHANBusiness;
        private HSCV_CONGVIECBusiness HSCV_CONGVIECBusiness;
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public PartialViewResult ListUserV2(int PHONGBAN_ID, string TEXT_ID, string VALUE_ID, string IS_MULTICHOICE,
            string IDS, string KEYWORD, string CALLBACK_FUNCTION, string INDEX, string SHOW_CHUC_VU_ID,
            string EXCLUDE_IDS, int? EXCEPT_DEPT, string ROLE = "")
        {
            string[] wIds = IDS.Split(',');
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            int DonViV2 = 0;
            if (SessionManager.GetValue("DonViV2") != null)
            {
                DonViV2 = SessionManager.GetValue("DonViV2").ToString().ToIntOrZero();
            }
            UserInfoBO user = (UserInfoBO)SessionManager.GetUserInfo();
            CCTCItemTreeBO TreeDonVi = (CCTCItemTreeBO)SessionManager.GetValue("TreeDonVi");
            if (PHONGBAN_ID == 0)
            {
                CCTC_THANHPHAN DonVi = CCTC_THANHPHANBusiness.GetFirstParent();
                PHONGBAN_ID = DonVi.ID;
            }
            if (TreeDonVi == null || DonViV2 != PHONGBAN_ID)
            {
                TreeDonVi = CCTC_THANHPHANBusiness.GetTree(PHONGBAN_ID);
            }
            List<CCTC_THANHPHAN> getChildPhongBan = (List<CCTC_THANHPHAN>)SessionManager.GetValue("ChildPhongBan");
            if (getChildPhongBan == null)
            {
                getChildPhongBan = CCTC_THANHPHANBusiness.GetDSChild(PHONGBAN_ID);
                CCTC_THANHPHAN DonVi = CCTC_THANHPHANBusiness.Find(PHONGBAN_ID);
                if (DonVi != null)
                {
                    getChildPhongBan.Add(DonVi);
                }
            }
            List<DM_NGUOIDUNG_BO> pageListNguoiDung = (List<DM_NGUOIDUNG_BO>)SessionManager.GetValue("ListUserV2");
            if (pageListNguoiDung == null)
            {
                pageListNguoiDung = DM_NGUOIDUNGBusiness.GetByPhongBan(getChildPhongBan.Select(x => x.ID).ToList(), user.ID, ROLE.ToListInt(','));
            }
            if (EXCEPT_DEPT.HasValue && EXCEPT_DEPT.Value > 0)
            {
                pageListNguoiDung = pageListNguoiDung.Where(x => x.DM_PHONGBAN_ID.HasValue && x.DM_PHONGBAN_ID.Value != EXCEPT_DEPT.Value).ToList();
            }
            ChonNguoiDungModel model = new ChonNguoiDungModel();
            model.TEXT_ID = TEXT_ID;
            model.VALUE_ID = VALUE_ID;
            model.IS_MULTICHOICE = IS_MULTICHOICE.ToIntOrZero();
            model.IDS = wIds;
            model.CALLBACK_FUNCTION = CALLBACK_FUNCTION;
            model.INDEX = INDEX.ToIntOrZero();
            model.SHOW_CHUC_VU_ID = SHOW_CHUC_VU_ID.Trim();
            model.TreeDonVi = TreeDonVi;
            model.LstNguoiDung = pageListNguoiDung;
            model.LstNguoiDungSearch = pageListNguoiDung;
            model.EXCEPT_DEPT = EXCEPT_DEPT;
            model.ROLE = ROLE;
            SessionManager.SetValue("ListUserV2", pageListNguoiDung);
            SessionManager.SetValue("DonViV2", PHONGBAN_ID);
            SessionManager.SetValue("TreeDonVi", TreeDonVi);
            SessionManager.SetValue("ChildPhongBan", getChildPhongBan);
            return PartialView("_ChonNguoiDungV2Result", model);
        }
        [HttpPost]
        public JsonResult GetDSByPhongBan(int id, int? EXCEPT_DEPT, string ROLE = "")
        {
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
            var getChildPhongBan = CCTC_THANHPHANBusiness.GetDSChild(id);
            var Ids = getChildPhongBan.Select(x => x.ID).ToList();
            Ids.Add(id);
            if (EXCEPT_DEPT.HasValue && EXCEPT_DEPT.Value > 0)
            {
                Ids = Ids.Where(x => x != EXCEPT_DEPT).ToList();
            }
            var modelResult = new PageListResultBO<DM_NGUOIDUNG_BO>();
            modelResult.ListItem = DM_NGUOIDUNGBusiness.GetByPhongBan(Ids, GetUserInfo().ID, ROLE.ToListInt(','));
            return Json(modelResult);
        }
        #region Các hàm thao tác với file
        public JsonResult CheckkingFile(long ID)
        {
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            TAILIEUDINHKEM taikieu = TAILIEUDINHKEMBusiness.Find(ID);
            UserInfoBO user = GetUserInfo();
            if (taikieu != null)
            {
                string path = URLPath + taikieu.DUONGDAN_FILE;
                if (System.IO.File.Exists(path))
                {
                    return Json("Co");
                }
                else
                {
                    return Json("Khong");
                }
            }
            return Json("Khong");
        }
        public FileResult DownloadFile(int id)
        {
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            if (id > 0 && id.GetType() == typeof(int))
            {
                TAILIEUDINHKEM taikieu = TAILIEUDINHKEMBusiness.Find(id);
                if (taikieu != null)
                {
                    AssignUserInfo();
                    var ITEM_ID = taikieu.ITEM_ID;
                    var LOAITAILIEUID = taikieu.LOAI_TAILIEU;
                    var checkAccess = false;
                    if (LOAITAILIEUID == LOAITAILIEU.VANBAN)
                    {
                        var WF_ITEM_USER_PROCESSBusiness = Get<WF_ITEM_USER_PROCESSBusiness>();
                        var HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
                        var DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
                        var WF_REVIEW_USERBusiness = Get<WF_REVIEW_USERBusiness>();
                        var VanBanTrinhKy = HSCV_VANBANDIBusiness.repository.Find(ITEM_ID);
                        if (VanBanTrinhKy != null)
                        {
                            // check quyền truy cập theo workflow
                            bool HasPermissionAccess = WF_ITEM_USER_PROCESSBusiness.CheckPermissionProcess(ITEM_ID.Value, MODULE_CONSTANT.VANBANTRINHKY, currentUser.ID);
                            // check quyền truy cập với quyền review
                            bool HasPermissionReview = WF_REVIEW_USERBusiness.CheckPermissionReview(ITEM_ID.Value, MODULE_CONSTANT.VANBANTRINHKY, currentUser.ID);
                            if (HasPermissionAccess || HasPermissionReview || currentUser.ID == VanBanTrinhKy.CREATED_BY)
                            {
                                checkAccess = true;
                            }
                        }

                    }
                    else if (LOAITAILIEUID == LOAITAILIEU.VANBANDEN)
                    {
                        var WF_ITEM_USER_PROCESSBusiness = Get<WF_ITEM_USER_PROCESSBusiness>();
                        var HSCV_VanBanDenBusiness = Get<HSCV_VANBANDENBusiness>();
                        var WF_REVIEW_USERBusiness = Get<WF_REVIEW_USERBusiness>();
                        var VanBanDen = HSCV_VanBanDenBusiness.repository.Find(ITEM_ID);
                        if (VanBanDen != null)
                        {
                            // check quyền truy cập theo workflow
                            bool HasPermissionAccess = WF_ITEM_USER_PROCESSBusiness.CheckPermissionProcess(ITEM_ID.Value, MODULE_CONSTANT.VANBANDEN, currentUser.ID);
                            // check quyền truy cập với quyền review
                            bool HasPermissionReview = WF_REVIEW_USERBusiness.CheckPermissionReview(ITEM_ID.Value, MODULE_CONSTANT.VANBANDEN, currentUser.ID);
                            if (HasPermissionAccess || HasPermissionReview || currentUser.ID == VanBanDen.NGUOITAO)
                            {
                                checkAccess = true;
                            }
                        }
                    }
                    else if (LOAITAILIEUID == LOAITAILIEU.CONGVIEC)
                    {
                        var HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
                        var HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness = Get<HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness>();
                        var CongViecObj = HSCV_CONGVIECBusiness.repository.Find(ITEM_ID);
                        if (CongViecObj != null)
                        {
                            List<long> ListParentId = new List<long>();
                            if (!string.IsNullOrEmpty(CongViecObj.CONGVIEC_LIENQUAN_ID))
                            {
                                //danh sách công việc cha
                                ListParentId = CongViecObj.CONGVIEC_LIENQUAN_ID.ToListLong(',');
                            }
                            ListParentId.Add(CongViecObj.ID);
                            //danh sách người tham gia xử lý của các công việc cha
                            var ListThamGia = HSCV_CONGVIEC_NGUOITHAMGIAXULYBusiness.GetData(ListParentId, currentUser.ID);
                            //danh sách người xử lý chính của các công việc cha
                            var ListXuLyChinh = HSCV_CONGVIECBusiness.GetData(ListParentId, currentUser.ID);
                            if (ListThamGia.Any() || ListXuLyChinh.Any())
                            {
                                checkAccess = true;
                            }
                        }

                    }
                    else if (LOAITAILIEUID == LOAITAILIEU.NOIDUNGTRAODOICONGVIEC)
                    {
                        checkAccess = true;

                    }
                    else if (LOAITAILIEUID == LOAITAILIEU.NOIDUNGTRAODOIVANBANDI)
                    {

                    }
                    if (taikieu.USER_ID == currentUser.ID || checkAccess)
                    {
                        string contentType = taikieu.DINHDANG_FILE;
                        string path = URLPath + taikieu.DUONGDAN_FILE;
                        var fileSave = Path.GetFileName(path);
                        return File(path, contentType, fileSave);
                    }
                }
            }
            return null;
        }
        public JsonResult DeleteFile(int id)
        {
            try
            {
                string URLPath = WebConfigurationManager.AppSettings["FileUpload"];
                TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
                if (id > 0 && id.GetType() == typeof(int))
                {
                    TAILIEUDINHKEM tailieu = TAILIEUDINHKEMBusiness.Find(id);
                    if (tailieu != null)
                    {
                        if (tailieu.LOAI_TAILIEU == LOAITAILIEU.VANBAN)
                        {
                            tailieu.IS_DELETE = true;
                        }
                        else
                        {
                            string contentType = tailieu.DINHDANG_FILE;
                            string path = URLPath + tailieu.DUONGDAN_FILE;
                            System.IO.File.Delete(path);
                            TAILIEUDINHKEMBusiness.repository.Delete(id);
                        }
                        TAILIEUDINHKEMBusiness.Save();
                    }

                }
                return Json("1");
            }
            catch
            {
                return Json("0");
            }
        }
        public PartialViewResult Preview(long id)
        {
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            TAILIEUDINHKEM TaiLieu = TAILIEUDINHKEMBusiness.Find(id);
            if (TaiLieu == null)
            {
                TaiLieu = new TAILIEUDINHKEM();
            }
            return PartialView("_Preview", TaiLieu);
        }
        #endregion

        public PartialViewResult ChartVanBan()
        {
            //AssignUserInfo();
            //VanBanChartVM model = new VanBanChartVM();
            //var currentDate = DateTime.Now;
            //var lastWeek = currentDate.Subtract(new TimeSpan(7, 0, 0, 0)).ToStartDay();
            //var WF_PROCESSBusiness = Get<WF_PROCESSBusiness>();
            //model.LstData = WF_PROCESSBusiness.getStaticVanBanChart(currentUser.ID, lastWeek, currentDate);
            //model.StartDate = lastWeek.Date;
            //model.EndDate = currentDate.Date;
            //return PartialView("_ChartVanBan", model);
            return PartialView("_ChartVanBan");
        }
        /// <summary>
        /// Báo cáo thống kê tiến độ và chất lượng công việc
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        //public PartialViewResult ChartTienDoCongViec(string from, string to, string type)
        //{
        //    TienDoCongViecModel model = new TienDoCongViecModel();
        //    HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
        //    AssignUserInfo();
        //    List<HSCV_CONGVIEC> ListCongViec = HSCV_CONGVIECBusiness.GetDataByUserId(currentUser.ID,
        //        from.ToDateTime(), to.ToDateTime());
        //    #region Xếp loại công việc
        //    var ListXepLoai = ListCongViec.Where(x => x.XEPLOAICONGVIEC.HasValue).ToList();
        //    int total = ListXepLoai.Count;
        //    int dat = ListXepLoai.Where(x => XepLoaiCongViecConstant.DAT == x.XEPLOAICONGVIEC).Count();
        //    int tot = ListXepLoai.Where(x => XepLoaiCongViecConstant.TOT == x.XEPLOAICONGVIEC).Count();
        //    int kha = ListXepLoai.Where(x => XepLoaiCongViecConstant.KHA == x.XEPLOAICONGVIEC).Count();
        //    int khongdat = ListXepLoai.Where(x => XepLoaiCongViecConstant.KHONGDAT == x.XEPLOAICONGVIEC).Count();
        //    if (total > 0)
        //    {
        //        model.DAT = Math.Round(((float)dat / total) * 100);
        //        model.TOT = Math.Round(((float)tot / total) * 100);
        //        model.KHA = Math.Round(((float)kha / total) * 100);
        //        model.KHONGDAT = Math.Round(((float)khongdat / total) * 100);
        //    }
        //    else
        //    {
        //        model.DAT = 0;
        //        model.TOT = 0;
        //        model.KHA = 0;
        //        model.KHONGDAT = 0;
        //    }
        //    #endregion
        //    total = ListCongViec.Count;
        //    DateTime now = DateTime.Now;
        //    var dunghan = ListCongViec.Where(x => 100 == x.PHANTRAMHOANTHANH ?
        //    ((true == x.IS_HASPLAN && (x.NGAYKETTHUC_KEHOACH.HasValue ? x.NGAYKETTHUC_THUCTE <= x.NGAYKETTHUC_KEHOACH : x.NGAYKETTHUC_THUCTE <= x.NGAYHOANTHANH_THEOMONGMUON))
        //    || ((false == x.IS_HASPLAN || !x.IS_HASPLAN.HasValue) && x.NGAYKETTHUC_THUCTE <= x.NGAYHOANTHANH_THEOMONGMUON)) :

        //    ((true == x.IS_HASPLAN && (x.NGAYKETTHUC_KEHOACH.HasValue ? now <= x.NGAYKETTHUC_KEHOACH : now <= x.NGAYHOANTHANH_THEOMONGMUON))
        //    || ((false == x.IS_HASPLAN || !x.IS_HASPLAN.HasValue) && now <= x.NGAYHOANTHANH_THEOMONGMUON))
        //    ).Count();

        //    int trehan = ListCongViec.Where(x => 100 == x.PHANTRAMHOANTHANH ?
        //    ((true == x.IS_HASPLAN && (x.NGAYKETTHUC_KEHOACH.HasValue ? x.NGAYKETTHUC_THUCTE >= x.NGAYKETTHUC_KEHOACH : x.NGAYKETTHUC_THUCTE >= x.NGAYHOANTHANH_THEOMONGMUON))
        //    || ((false == x.IS_HASPLAN || !x.IS_HASPLAN.HasValue) && x.NGAYKETTHUC_THUCTE >= x.NGAYHOANTHANH_THEOMONGMUON)) :

        //    ((true == x.IS_HASPLAN && (x.NGAYKETTHUC_KEHOACH.HasValue ? now >= x.NGAYKETTHUC_KEHOACH : now >= x.NGAYHOANTHANH_THEOMONGMUON))
        //    || ((false == x.IS_HASPLAN || !x.IS_HASPLAN.HasValue) && now >= x.NGAYHOANTHANH_THEOMONGMUON))
        //    ).Count();
        //    model.DUNGHAN = Math.Round(((float)dunghan / total) * 100, 2);
        //    //model.TREHAN = Math.Round(((float)trehan / total) * 100);
        //    model.TREHAN = 100 - model.DUNGHAN;
        //    if (!string.IsNullOrEmpty(type))
        //    {
        //        return PartialView("_TienDoCV", model);
        //    }
        //    return PartialView("_ChartTienDoCongViec", model);
        //}
        public PartialViewResult NewNotification()
        {
            SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
            List<SYS_TINNHAN> ListNotif = new List<SYS_TINNHAN>();
            AssignUserInfo();
            ListNotif = SYS_TINNHANBusiness.GetDaTaByPage(currentUser.ID, true, 5, 1).ListItem;
            return PartialView("_Notification", ListNotif);
        }
        /// <summary>
        /// @description: danh sách văn bản đi + đến
        /// @author: duynn
        /// @since: 08/08/2018
        /// </summary>
        /// <returns></returns>
        public PartialViewResult RecentVanBan()
        {
            AssignUserInfo();
            HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();

            HSCV_VANBANDEN_SEARCH searchModelOfVanBanDen = new HSCV_VANBANDEN_SEARCH();
            HSCV_VANBANDI_SEARCH searchModelOfVanBanDi = new HSCV_VANBANDI_SEARCH();

            searchModelOfVanBanDen.USER_ID = currentUser.ID;
            searchModelOfVanBanDen.ITEM_TYPE = MODULE_CONSTANT.VANBANDEN;

            searchModelOfVanBanDi.USER_ID = currentUser.ID;
            searchModelOfVanBanDi.ITEM_TYPE = MODULE_CONSTANT.VANBANTRINHKY;

            VanBanModel model = new VanBanModel();
            model.ListVbDen = HSCV_VANBANDENBusiness.GetListInProcess(searchModelOfVanBanDen, 5);
            model.ListVbDi = HSCV_VANBANDIBusiness.GetListProcessing(searchModelOfVanBanDi, 5);

            searchModelOfVanBanDen.ITEM_TYPE = MODULE_CONSTANT.VANBANDENNOIBO;
            searchModelOfVanBanDen.isInternal = true;
            model.ListVbDenNoiBo = HSCV_VANBANDENBusiness.GetListInProcess(searchModelOfVanBanDen, 5);
            return PartialView("_RecentVanBan", model);
        }
        public PartialViewResult ListNotif()
        {
            SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
            AssignUserInfo();
            PageListResultBO<SYS_TINNHAN> ListNotif = SYS_TINNHANBusiness.GetDaTaByPage(currentUser.ID, true, 10, 1);
            return PartialView("_ListNotification", ListNotif);
        }
        public PartialViewResult Statistical()
        {
            AssignUserInfo();
            //HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            //PageListResultBO<CongViecBO> PageCongViec = new PageListResultBO<CongViecBO>();
            HSCV_VANBANDI_SEARCH searchModel = new HSCV_VANBANDI_SEARCH();
            searchModel.USER_ID = currentUser.ID;
            searchModel.ITEM_TYPE = MODULE_CONSTANT.VANBANTRINHKY;
            PageListResultBO<HSCV_VANBANDI_BO> PageVanBan = HSCV_VANBANDIBusiness.GetListProcessing(searchModel, 10, 1);
            Statistical model = new Statistical();
            model.ListResultVanBan = PageVanBan;
            //PageCongViec = HSCV_CONGVIECBusiness.GetDataByUserId(currentUser.ID, 10, 1);
            //model.ListResultCongViec = PageCongViec;
            HSCV_VANBANDEN_SEARCH searchModelVBDen = new HSCV_VANBANDEN_SEARCH();
            searchModelVBDen.USER_ID = currentUser.ID;
            searchModelVBDen.ITEM_TYPE = MODULE_CONSTANT.VANBANDEN;
            model.ListResultVanBanDen = HSCV_VANBANDENBusiness.GetListInProcess(searchModelVBDen, 10, 1);
            return PartialView("_Statistical", model);
        }
        public JsonResult UpdateNotifStatus(long id)
        {
            SYS_TINNHANBusiness = Get<SYS_TINNHANBusiness>();
            SYS_TINNHAN Notif = SYS_TINNHANBusiness.Find(id);
            if (Notif != null)
            {
                Notif.IS_READ = true;
                SYS_TINNHANBusiness.Save(Notif);
            }
            return Json(new { Type = "SUCCESS" });
        }

        public PartialViewResult ThongKeCongViec()
        {
            DanhSachCongViec model = new DanhSachCongViec();
            AssignUserInfo();
            HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            var PageCongViec = HSCV_CONGVIECBusiness.GetDataByUserId(currentUser.ID, 10, 1);
            model.Type = 1;
            model.ListResult = PageCongViec;
            return PartialView("_ListCongViec", model);
        }
        public PartialViewResult SmartSearch(string search)
        {
            AssignUserInfo();
            var result = ElasticSearch.SmartSearch(search, currentUser.ID);
            SmartSearch model = new SmartSearch();
            model.search = search;
            model.ListSearchResult = result;
            return PartialView("_ResultSmartSearch", model);
        }
        #region Chat
        [ValidateInput(false)]
        public long InsertChatContent(string msg, string from, string to, long group, int coso)
        {
            var ChatNoidungBusiness = Get<CHAT_NOIDUNGBusiness>();
            var NguoiDungBs = Get<DM_NGUOIDUNGBusiness>();
            var from_user = NguoiDungBs.repository.All().Where(o => o.TENDANGNHAP == from).FirstOrDefault();
            var to_user = NguoiDungBs.repository.All().Where(o => o.TENDANGNHAP == to).FirstOrDefault();
            if (from_user != null && to_user != null)
            {
                CHAT_NOIDUNG noidung = new CHAT_NOIDUNG();
                noidung.NGUOIGUI_ID = from_user.ID;
                noidung.NGUOINHAN_ID = to_user.ID;
                noidung.FROMUSER = from_user.TENDANGNHAP;
                noidung.TOUSER = to_user.TENDANGNHAP;
                noidung.FROMFULLNAME = from_user.HOTEN;
                noidung.TOFULLNAME = to_user.HOTEN;
                if (group > 0)
                {
                    noidung.GROUPCHAT_ID = group;
                }
                noidung.COSO_ID = coso;
                noidung.NOIDUNG = msg;
                noidung.NGAYGUI = DateTime.Now;
                noidung.IS_DELETE = false;
                ChatNoidungBusiness.Save(noidung);
                return noidung.ID;
            }
            return 0;
        }
        public JsonResult DeleteChatContent(long id)
        {
            AssignUserInfo();
            var ChatNoidungBusiness = Get<CHAT_NOIDUNGBusiness>();
            var noidung = ChatNoidungBusiness.repository.Find(id);
            if (noidung.NGUOIGUI_ID == currentUser.ID)
            {
                ChatNoidungBusiness.repository.Delete(id);
                ChatNoidungBusiness.Save();
                return Json(new { Type = "SUCCESS", Mess = "Xóa thành công" });
            }
            else
            {
                return Json(new { Type = "ERROR", Mess = "Bạn không có quyền xóa nội dung này" });
            }
        }
        [ValidateInput(false)]
        public long InsertChatGroupContent(string msg, string from, long group, int coso)
        {
            var ChatNoidungBusiness = Get<CHAT_NOIDUNGBusiness>();
            var NguoiDungBs = Get<DM_NGUOIDUNGBusiness>();
            if (group > 0)
            {
                var from_user = NguoiDungBs.repository.All().Where(o => o.TENDANGNHAP == from).FirstOrDefault();
                if (from_user != null)
                {
                    CHAT_NOIDUNG noidung = new CHAT_NOIDUNG();
                    noidung.NGUOIGUI_ID = from_user.ID;
                    noidung.FROMUSER = from_user.TENDANGNHAP;
                    noidung.FROMFULLNAME = from_user.HOTEN;
                    noidung.GROUPCHAT_ID = group;
                    noidung.COSO_ID = coso;
                    noidung.NOIDUNG = msg;
                    noidung.NGAYGUI = DateTime.Now;
                    noidung.IS_DELETE = false;
                    ChatNoidungBusiness.Save(noidung);
                    return noidung.ID;
                }
            }
            return 0;
        }
        [ValidateInput(false)]
        public void UpdateGroupName(string groupname, string msg, string from, long group, int coso)
        {
            var ChatNoidungBusiness = Get<CHAT_NOIDUNGBusiness>();
            var NguoiDungBs = Get<DM_NGUOIDUNGBusiness>();
            var ChatGroupBusiness = Get<CHAT_GROUPBusiness>();
            if (group > 0)
            {
                var GroupChat = ChatGroupBusiness.Find(group);
                GroupChat.TENNHOM = groupname;
                GroupChat.NGAYSUA = DateTime.Now;
                GroupChat.NGUOISUA = from;
                ChatGroupBusiness.Save(GroupChat);
                var from_user = NguoiDungBs.repository.All().Where(o => o.TENDANGNHAP == from).FirstOrDefault();
                if (from_user != null)
                {
                    CHAT_NOIDUNG noidung = new CHAT_NOIDUNG();
                    noidung.NGUOIGUI_ID = from_user.ID;
                    noidung.FROMUSER = from_user.TENDANGNHAP;
                    noidung.FROMFULLNAME = from_user.HOTEN;
                    noidung.GROUPCHAT_ID = group;
                    noidung.COSO_ID = coso;
                    noidung.NOIDUNG = msg;
                    noidung.NGAYGUI = DateTime.Now;
                    noidung.IS_DELETE = false;
                    ChatNoidungBusiness.Save(noidung);
                }
            }
        }
        //LeftGroupChat
        [ValidateInput(false)]
        public void LeftGroupChat(int coso, long group_id, string username)
        {
            var ChatNoidungBusiness = Get<CHAT_NOIDUNGBusiness>();
            var NguoiDungBs = Get<DM_NGUOIDUNGBusiness>();
            var ChatGroupUserBusiness = Get<CHAT_GROUP_USERBusiness>();
            if (group_id > 0)
            {
                var user_left = NguoiDungBs.repository.All().Where(o => o.TENDANGNHAP == username).FirstOrDefault();
                var user_group_left = ChatGroupUserBusiness.repository.All().Where(o => o.GROUP_ID == group_id && o.USER_ID == user_left.ID).FirstOrDefault();
                ChatGroupUserBusiness.repository.Delete(user_group_left.ID);
                ChatGroupUserBusiness.Save();
                if (user_left != null)
                {
                    CHAT_NOIDUNG noidung = new CHAT_NOIDUNG();
                    noidung.NGUOIGUI_ID = user_left.ID;
                    noidung.FROMUSER = user_left.TENDANGNHAP;
                    noidung.FROMFULLNAME = user_left.HOTEN;
                    noidung.GROUPCHAT_ID = group_id;
                    noidung.COSO_ID = coso;
                    noidung.NOIDUNG = "Đã rời khỏi nhóm chat";
                    noidung.NGAYGUI = DateTime.Now;
                    noidung.IS_DELETE = false;
                    ChatNoidungBusiness.Save(noidung);
                }
            }
        }
        [ValidateInput(false)]
        public PartialViewResult Chat(int? cosoId, string fromUser, string toUser, string fromFullName, string toFullName,
           int? soCuaSoChat, int? reload, string chat_id, int? index)
        {
            var ChatNoidungBusiness = Get<CHAT_NOIDUNGBusiness>();
            UserInfoBO user = (UserInfoBO)SessionManager.GetUserInfo();
            ChatViewModel model = new ChatViewModel();
            model.cosoId = user.DeptParentID.Value;
            model.fromUser = fromUser;
            model.toUser = toUser;
            model.fromFullName = fromFullName;
            model.toFullName = toFullName;
            model.soCuaSoChat = soCuaSoChat.Value;
            model.currentUserName = user.TENDANGNHAP;
            //chat_id = chat_id.Replace("@", "\\@");
            //chat_id = chat_id.Replace(".", "\\.");
            model.chat_id = chat_id;
            model.reload = reload.Value;
            model.index = index.Value;
            model.listChat = ChatNoidungBusiness.GetListChat(fromUser, toUser, DateTime.Now, 0, 30, 0);
            return PartialView("_Chat", model);
        }
        public JsonResult UploadFileAttachmentChat(IEnumerable<HttpPostedFileBase> attachment, FormCollection col)
        {
            UploadFileTool upload = new UploadFileTool();
            string[] filename = new string[attachment.Count()];
            List<string> OutFilePath;
            List<string> OutFileName;
            List<string> OutFileExt;
            List<long> OutFileID;
            upload.UploadCustomFileAndOutPath(attachment, true, string.Empty, URLPath, 0, null, filename, 0, out OutFilePath, out OutFileName, out OutFileExt, out OutFileID, LOAITAILIEU.CHAT, "Chat-Trao đổi");
            return Json(new { OutFilePath = OutFilePath, OutFileName = OutFileName, OutFileExt = OutFileExt, OutFileID = OutFileID });
        }
        [HttpPost]
        public JsonResult InsertGroupChat(int cosoId, string created_user, string listUserName)
        {
            //cosoId = 0;
            UserInfoBO user = (UserInfoBO)SessionManager.GetUserInfo();
            cosoId = user.DeptParentID.Value;
            var ChatGroupBusiness = Get<CHAT_GROUPBusiness>();
            var ChatGroupUserBusiness = Get<CHAT_GROUP_USERBusiness>();
            var NguoiDungBs = Get<DM_NGUOIDUNGBusiness>();
            var list_UserName = listUserName.Split(',');
            CHAT_GROUP group = new CHAT_GROUP();
            if (list_UserName != null && list_UserName.Count() > 0)
            {
                group.COSO_ID = cosoId;
                group.USERCREATE_ID = (long)user.ID;
                group.NGUOITAO = user.TENDANGNHAP;
                group.NGAYTAO = DateTime.Now;
                ChatGroupBusiness.Save(group);
                foreach (var username in list_UserName)
                {
                    var _username = username.Trim();
                    var userInfor = NguoiDungBs.repository.All().Where(o => o.TENDANGNHAP.ToLower() == _username).FirstOrDefault();
                    CHAT_GROUP_USER grp_user = new CHAT_GROUP_USER();
                    grp_user.GROUP_ID = group.ID;
                    grp_user.USER_ID = userInfor.ID;
                    grp_user.USERNAME = userInfor.TENDANGNHAP;
                    grp_user.FULLNAME = userInfor.HOTEN;
                    ChatGroupUserBusiness.Save(grp_user);
                }
            }
            var groupChat_id = string.Format("g_{0}_{1}", cosoId, group.ID);
            return Json(new { groupId = group.ID, groupChat_id = groupChat_id });
        }
        [ValidateInput(false)]
        public PartialViewResult ChatGroup(int? cosoId, string createdUser, long? groupId, string currentUserName,
           int? soCuaSoChat, int? reload, string groupChat_id)
        {
            var ChatNoidungBusiness = Get<CHAT_NOIDUNGBusiness>();
            var ChatGroupBusiness = Get<CHAT_GROUPBusiness>();
            var ChatGroupUserBusiness = Get<CHAT_GROUP_USERBusiness>();
            UserInfoBO user = (UserInfoBO)SessionManager.GetUserInfo();
            var groupChat = ChatGroupBusiness.Find(groupId.Value);
            ChatViewModel model = new ChatViewModel();
            model.listFullName = ChatGroupUserBusiness.GetListUserName(groupId.Value);
            model.groupName = string.IsNullOrEmpty(groupChat.TENNHOM) ? model.listFullName : groupChat.TENNHOM;
            model.cosoId = cosoId.Value;
            model.soCuaSoChat = soCuaSoChat.Value;
            model.currentUserName = currentUserName;
            model.groupChat_id = groupChat_id;
            model.group_id = groupId.Value;
            model.reload = reload.Value;
            //model.index = index.Value;
            model.listChat = ChatNoidungBusiness.GetListChat(string.Empty, string.Empty, DateTime.Now, groupId.Value, 15, 0);
            return PartialView("_ChatGroup", model);
        }
        //UpdateGroupChat
        [HttpPost]
        public JsonResult UpdateGroupChat(int cosoId, long? group_id, string listUserName)
        {
            cosoId = 0;
            UserInfoBO user = (UserInfoBO)SessionManager.GetUserInfo();
            var ChatGroupBusiness = Get<CHAT_GROUPBusiness>();
            var ChatGroupUserBusiness = Get<CHAT_GROUP_USERBusiness>();
            var NguoiDungBs = Get<DM_NGUOIDUNGBusiness>();
            var list_UserName = listUserName.Split(',');
            CHAT_GROUP group = ChatGroupBusiness.Find(group_id.Value);
            string user_added = "";
            string fullname_add = "";
            if (list_UserName != null && list_UserName.Count() > 0)
            {
                group.COSO_ID = cosoId;
                group.NGUOISUA = user.TENDANGNHAP;
                group.NGAYSUA = DateTime.Now;
                ChatGroupBusiness.Save(group);
                foreach (var username in list_UserName)
                {
                    var _username = username.Trim();
                    var userInfor = NguoiDungBs.repository.All().Where(o => o.TENDANGNHAP.ToLower() == _username).FirstOrDefault();
                    var user_in_group = ChatGroupUserBusiness.repository.All().Where(o => o.GROUP_ID == group_id.Value && o.USER_ID == userInfor.ID).FirstOrDefault();
                    if (user_in_group == null)
                    {
                        CHAT_GROUP_USER grp_user = new CHAT_GROUP_USER();
                        grp_user.GROUP_ID = group.ID;
                        grp_user.USER_ID = userInfor.ID;
                        grp_user.USERNAME = userInfor.TENDANGNHAP;
                        grp_user.FULLNAME = userInfor.HOTEN;
                        ChatGroupUserBusiness.Save(grp_user);
                        user_added += _username + ",";
                        fullname_add += userInfor.HOTEN + ",";
                    }
                }
            }
            var groupChat_id = string.Format("g_{0}_{1}", cosoId, group.ID);
            return Json(new { groupId = group.ID, groupChat_id = groupChat_id, user_added = user_added, fullname_added = fullname_add });
        }
        public JsonResult UploadFileAttachmentGroupChat(IEnumerable<HttpPostedFileBase> attachment, FormCollection col)
        {
            UploadFileTool upload = new UploadFileTool();
            string[] filename = new string[attachment.Count()];
            List<string> OutFilePath;
            List<string> OutFileName;
            List<string> OutFileExt;
            List<long> OutFileID;
            upload.UploadCustomFileAndOutPath(attachment, true, string.Empty, URLPath, 0, null, filename, 0, out OutFilePath, out OutFileName, out OutFileExt, out OutFileID, LOAITAILIEU.CHAT, "ChatGroup-Trao đổi");
            return Json(new { OutFilePath = OutFilePath, OutFileName = OutFileName, OutFileExt = OutFileExt, OutFileID = OutFileID });
        }
        //[HttpPost]
        //public void InsertMessage(FormCollection col)
        //{
        //    //Lấy danh sách user nhận tin nhắn
        //    //dạng string chuyển thành List<string>: List<UserName>
        //    //các user nhận tin nhắn, ví dụ: namdv, cuc_truong, tulq
        //    var usernames = col["USERNAMES"];
        //    if (!string.IsNullOrEmpty(usernames))
        //    {
        //        SYS_TINNHANBusiness SysTinnhanBusiness = Get<SYS_TINNHANBusiness>();
        //        List<SYS_TINNHAN> list_msg = new List<SYS_TINNHAN>();
        //        var list_username = usernames.Split(',');
        //        if (list_username != null && list_username.Count() > 0)
        //        {
        //            SYS_TINNHAN msg = new SYS_TINNHAN();
        //            //Thông tin người gửi tin nhắn
        //            msg.FROM_USER_ID = col["FROM_USER_ID"].ToIntOrNULL();
        //            msg.FROM_USERNAME = col["FROM_USERNAME"];

        //            //Url chi tiết đi kèm với tin nhắn: ví dụ url đến chi tiết văn bản, chi tiết đơn nâng lương ...
        //            msg.URL = col["URL"];
        //            //Tiêu đề của tin nhắn
        //            msg.TIEUDE = col["TIEUDE"];
        //            //Nội dung tin nhắn
        //            msg.NOIDUNG = col["NOIDUNG"];
        //            msg.NGAYTAO = DateTime.Now;
        //            msg.FROM_USERNAME = msg.FROM_USERNAME;
        //            //Danh sách người nhận
        //            foreach (var to_user in list_username)
        //            {
        //                if (!string.IsNullOrEmpty(usernames))
        //                {
        //                    msg.TO_USERNAME = to_user;
        //                    list_msg.Add(msg);
        //                }
        //            }
        //            SysTinnhanBusiness.SaveListMsg(list_msg);
        //            //foreach (var item in list_msg)
        //            //{
        //            //    hn.pushNotification(item);
        //            //}
        //        }
        //    }
        //}

        //public PartialViewResult ShowTinNhan()
        //{
        //    UserInfoBO user = (UserInfoBO)SessionManager.GetUserInfo();
        //    SYS_TINNHANBusiness SysTinnhanBusiness = Get<SYS_TINNHANBusiness>();
        //    var listTinNhan = SysTinnhanBusiness.GetListTinNhan(user.UserID, 15, 1);
        //    return PartialView("_ShowTinNhan", listTinNhan);
        //}
        #endregion

        #region Change password
        public PartialViewResult ChangePassword()
        {
            AssignUserInfo();
            return PartialView("_ChangePassword", currentUser);
        }
        //public JsonResult SaveChangePassword(FormCollection col, HttpPostedFileBase AVATAR)
        public ActionResult SaveChangePassword(FormCollection col, HttpPostedFileBase AVATAR)
        {
            string password = col["Password"];
            string NewPassword = col["NewPassword"];
            string RePassword = col["RePassword"];
            string Email = col["Email"];
            AssignUserInfo();
            var pass = MaHoaMatKhau.Encode_Data(password + currentUser.MAHOA_MK);
            List<CommonError> ListError = new List<CommonError>();
            CommonError error = new CommonError();
            if (!string.IsNullOrEmpty(password))
            {
                if (string.IsNullOrEmpty(password))
                {
                    error = new CommonError();
                    error.Field = "Password";
                    error.Message = "Bạn chưa nhập mật khẩu cũ";
                    ListError.Add(error);
                }
                else if (pass != currentUser.MATKHAU)
                {
                    error = new CommonError();
                    error.Field = "Password";
                    error.Message = "Mật khẩu cũ không đúng";
                    ListError.Add(error);
                }
                if (string.IsNullOrEmpty(NewPassword))
                {
                    error = new CommonError();
                    error.Field = "NewPassword";
                    error.Message = "Bạn chưa nhập mật khẩu mới";
                    ListError.Add(error);
                }
                else if (NewPassword.Trim().Length < 8)
                {
                    error = new CommonError();
                    error.Field = "NewPassword";
                    error.Message = "Mật khẩu mới phải chứa ít nhất 8 ký tự";
                    ListError.Add(error);
                }
                if (RePassword != NewPassword)
                {
                    error = new CommonError();
                    error.Field = "RePassword";
                    error.Message = "Mật khẩu không giống nhau";
                    ListError.Add(error);
                }
            }
            if (!string.IsNullOrEmpty(Email))
            {
                try
                {
                    MailAddress m = new MailAddress(Email);
                }
                catch (FormatException)
                {
                    error = new CommonError();
                    error.Field = "Email";
                    error.Message = "Email không đúng định dạng";
                    ListError.Add(error);
                }
            }
            if (ListError.Any())
            {
                return Json(new { Type = "ERROR", Message = "Không thể thay đổi mật khẩu", Error = ListError });
            }
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            var ngdung = DM_NGUOIDUNGBusiness.Find(currentUser.ID);
            if (!string.IsNullOrEmpty(NewPassword))
            {
                ngdung.MAHOA_MK = CommonHelper.MaHoaMatKhau.GenerateRandomString(5);
                ngdung.MATKHAU = CommonHelper.MaHoaMatKhau.Encode_Data(NewPassword + ngdung.MAHOA_MK);
            }
            if (!string.IsNullOrEmpty(Email))
            {
                ngdung.EMAIL = Email;
            }

            if (AVATAR != null)
            {
                string pathfolder = "Uploads\\User\\" + ngdung.ID + "\\";
                if (!System.IO.Directory.Exists(pathfolder))
                {
                    Directory.CreateDirectory(Path.Combine(Server.MapPath("~"), pathfolder));
                }

                if (!string.IsNullOrEmpty(ngdung.signpath))
                {
                    string pathFile = Path.Combine(Server.MapPath("~"), ngdung.signpath);
                    if (System.IO.File.Exists(pathFile))
                    {
                        System.IO.File.Delete(pathFile);
                    }
                }

                var arrFile = AVATAR.FileName.Split('.');
                string pathSaveLogo = pathfolder + "Avatar." + arrFile[arrFile.Count() - 1];
                string logoPath = Path.Combine(Server.MapPath("~"), pathSaveLogo);
                AVATAR.SaveAs(logoPath);
                ngdung.signpath = pathSaveLogo;
            }
            DM_NGUOIDUNGBusiness.Save(ngdung);
            currentUser.signpath = ngdung.signpath;
            SessionManager.SetValue(SessionManager.USER_INFO, currentUser);
            return Redirect("/");
            //return Json(new { Type = "SUCCESS", Message = "Thay đổi mật khẩu thành công" });
        }
        #endregion
        /// <summary>
        /// Màn hình danh sách công việc đã giao
        /// </summary>
        /// <returns></returns>
        public PartialViewResult MyJob()
        {
            AssignUserInfo();
            DanhSachCongViec model = new DanhSachCongViec();
            model.Type = 2;
            #region Check role
            //if (IsInActivities(currentUser.ListThaoTac, Permission.GIAOVIEC_DONVI)
            //    || IsInActivities(currentUser.ListThaoTac, Permission.GIAOVIEC_CANHAN)
            //    || IsInActivities(currentUser.ListThaoTac, Permission.GIAOVIEC_PHONGBAN))
            //{
            //    HSCV_CONGVIECBusiness = Get<HSCV_CONGVIECBusiness>();
            //    //Ban giám đốc
            //    HSCV_CONGVIEC_SEARCH searchModel = new HSCV_CONGVIEC_SEARCH();
            //    searchModel.LOAI_CONGVIEC = LOAI_CONGVIEC.CANHAN;
            //    searchModel.USER_ID = currentUser.ID;
            //    searchModel.pageSize = 10;
            //    var ListCongViec = HSCV_CONGVIECBusiness.GetDaTaByPage(searchModel, 10);
            //    model.ListResult = ListCongViec;

            //    return PartialView("_ListCongViec", model);
            //}
            #endregion
            model.ListResult = new PageListResultBO<CongViecBO>();
            return PartialView("_ListCongViec", model);
        }

        public PartialViewResult CheckFile(long id)
        {
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            TAILIEUDINHKEM TaiLieu = TAILIEUDINHKEMBusiness.Find(id);
            var fullPath = Server.MapPath("~/Uploads/" + TaiLieu.DUONGDAN_FILE);
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = Server.MapPath("~/Abbyy/ConsoleApplication1.exe");
            fullPath = "C:\\Users\\Administrator\\Desktop\\abc.pdf";
            process.StartInfo.Arguments =
                fullPath + " C:\\Users\\Administrator\\Desktop\\" + TaiLieu.TAILIEU_ID + ".docx";
            process.Start();
            process.Close();
            //System.Diagnostics.Process.Start("CMD.exe", "/C " + FullCMD);
            System.Threading.Thread.Sleep(10000);
            return PartialView("_CheckFileOCR", process.StartInfo.Arguments);
        }
        public PartialViewResult PreviewExistedFile(long ID)
        {
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            TAILIEUDINHKEM TaiLieu = TAILIEUDINHKEMBusiness.Find(ID);
            var fullPath = "/Uploads/" + TaiLieu.DUONGDAN_FILE;
            var CheckExtension = TaiLieu.DUONGDAN_FILE.Split('.');
            var Extension = CheckExtension[CheckExtension.Length - 1].ToLower();

            ViewTaiLieuVM model = new ViewTaiLieuVM();
            switch (Extension)
            {
                case "pdf":
                    break;
                case "jpg":
                case "png":
                    break;
                case "doc":
                case "docx":
                    var TmpFilePdf =
                        Server.MapPath("~/Uploads/Pdf/" + TaiLieu.ITEM_ID.ToString() + ".pdf");
                    if (!System.IO.File.Exists(TmpFilePdf))
                    {
                        Word2Pdf convert = new Word2Pdf();
                        convert.Convert(Server.MapPath("~/Uploads/" + TaiLieu.DUONGDAN_FILE), TmpFilePdf);
                    }
                    fullPath = "/Uploads/Pdf/" + TaiLieu.ITEM_ID.ToString() + ".pdf";
                    break;
                //case "xlsx":
                //case "xls":
                //    var TmpFileExcel2Pdf =
                //        Server.MapPath("~/Uploads/Pdf/" + TaiLieu.ITEM_ID.ToString() + ".pdf");
                //    var TmpFileExcel =
                //        Server.MapPath("~/Uploads/Pdf/Temp/" + TaiLieu.ITEM_ID.ToString() + "." + Extension);
                //    if (!System.IO.File.Exists(TmpFileExcel2Pdf))
                //    {
                //        string filePathSource = Server.MapPath("~/Uploads/" + TaiLieu.DUONGDAN_FILE);
                //        System.IO.File.Copy(filePathSource, TmpFileExcel, true);
                //        Excel2Pdf convertExcel = new Excel2Pdf();
                //        convertExcel.Convert(TmpFileExcel, TmpFileExcel2Pdf);
                //        System.IO.File.Delete(TmpFileExcel);
                //    }
                //    fullPath = "/Uploads/Pdf/" + TaiLieu.ITEM_ID.ToString() + ".pdf";
                //    break;
                default:
                    break;
            }

            model.Extension = Extension;
            model.fullPath = fullPath;
            return PartialView("_showDetailTaiLieu", model);
        }

        //public PartialViewResult CheckFile(long id)
        //{
        //    TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
        //    TAILIEUDINHKEM TaiLieu = TAILIEUDINHKEMBusiness.Find(id);
        //    var fullPath = Server.MapPath("~/Uploads/" + TaiLieu.DUONGDAN_FILE);
        //    XElement HtmlString = DocxProvider.GetHTMLString(fullPath, TaiLieu.TENTAILIEU);
        //    XmlDocument doc = new XmlDocument();
        //    doc.LoadXml(HtmlString.ToString());
        //    XmlNodeList lstSpan = doc.GetElementsByTagName("span");
        //    CheckFileModel model = new CheckFileModel();
        //    model.HtmlString = HtmlString;
        //    var index = 0;
        //    Dictionary<int, string> dicParamgraph = new Dictionary<int, string>();
        //    Dictionary<int, ISearchResponse<ElasticBCAModel>> dicResponses = new Dictionary<int, ISearchResponse<ElasticBCAModel>>();
        //    ;
        //    foreach (XmlNode item in lstSpan)
        //    {
        //        var innerText = item.InnerText;
        //        if (!string.IsNullOrEmpty(innerText))
        //        {
        //            innerText = innerText.Trim();
        //            if (innerText.Length >= 5)
        //            {
        //                var charCount = 0;
        //                var maxLineLength = 500;
        //                var lines = innerText.Split('.');
        //                foreach (var line in lines)
        //                {
        //                    if (line.Length >= 5)
        //                    {
        //                        var sublines = line.Split(' ')
        //                            .GroupBy(w => (charCount += w.Length + 1) / maxLineLength)
        //                            .Select(g => string.Join(" ", g));
        //                        foreach (var subline in sublines)
        //                        {
        //                            dicParamgraph.Add(index, subline);
        //                            var lstTmpResult = ElasticSearch.SmartSearchFileContent(subline);
        //                            dicResponses.Add(index, lstTmpResult);
        //                            index = index + 1;
        //                        }
        //                    }

        //                }                  
        //            }

        //        }
        //    }

        //    model.totalindex = index;
        //    model.dicParamgraph = dicParamgraph;
        //    model.dicResponses = dicResponses;
        //    SessionManager.SetValue("dicResponses", dicResponses);
        //    return PartialView("_CheckFile", model);
        //}

        //public PartialViewResult showDetailParagraph(int index)
        //{
        //    var dicResponses = (Dictionary<int, ISearchResponse<ElasticBCAModel>>)SessionManager.GetValue("dicResponses");
        //    var result = dicResponses[index];
        //    return PartialView("_showDetailParagraph", result);
        //}

        //public PartialViewResult showDetailDetaiTrungLap(int detaiid, string tendetai)
        //{
        //    var dicResponses = (Dictionary<int, ISearchResponse<ElasticBCAModel>>)SessionManager.GetValue("dicResponses");
        //    List<string> lstFinalHighlights = new List<string>();
        //    foreach (var res in dicResponses)
        //    {
        //        var highlightsInEachHit = res.Value.Hits.Select(d => d.Highlights);
        //        foreach (var highlightField in highlightsInEachHit)
        //        {
        //            var lstHighlightField = highlightField.Values.Where(x => x.DocumentId == "BCA_" + detaiid.ToString()).ToList();
        //            foreach(var item in lstHighlightField)
        //            {
        //                lstFinalHighlights.AddRange(item.Highlights);
        //            }
        //        }
        //    }

        //    ShowTrungLapDeTaiModel model = new ShowTrungLapDeTaiModel();
        //    model.tendetai = tendetai;
        //    model.lsthighlights = lstFinalHighlights;
        //    return PartialView("_showDetailDetaiTrungLap", model);
        //}
        //public string GetDocXInnerText(string docxFilepath)
        //{
        //    string folder = Path.GetDirectoryName(docxFilepath);
        //    string extractionFolder = folder + "\\extraction";

        //    if (Directory.Exists(extractionFolder))
        //        Directory.Delete(extractionFolder, true);

        //    System.IO.Compression.ZipFile.ExtractToDirectory(docxFilepath, extractionFolder);
        //    string xmlFilepath = extractionFolder + "\\word\\document.xml";

        //    var xmldoc = new XmlDocument();
        //    xmldoc.Load(xmlFilepath);

        //    return xmldoc.DocumentElement.InnerText;
        //}

        /// <summary>
        /// @author: duynn
        /// @since: 13/08/2018
        /// @description: lấy số đi theo sổ của văn bản
        /// </summary>
        /// <param name="idSoVanBanDen"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetSoDiTheoSo(int idSoVanBan, bool isVanBanDi = false)
        {
            var dmDanhMucDataBusiness = Get<DM_DANHMUC_DATABusiness>();
            string docNumber = dmDanhMucDataBusiness.GetSoDiTheoSoVanBan(idSoVanBan);
            //nếu là văn bản đi lấy các số còn thiếu
            List<int> suggestDocNumbers = new List<int>();
            if (!string.IsNullOrEmpty(docNumber) && isVanBanDi)
            {
                suggestDocNumbers = dmDanhMucDataBusiness.GetSuggestSoVanBanDi(idSoVanBan);
            }
            return Json(new { numbSoDiTheoSo = docNumber, numbSoDiChuaCo = suggestDocNumbers });
        }

        /// <summary>
        /// @description: cập nhật tài liệu đính kèm của văn bản
        /// @author: duynn
        /// </summary>
        /// <param name="attachmentId"></param>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult EditDocAttachment(long attachmentId, long itemId, string itemType)
        {
            VanBanModel viewModel = new VanBanModel();
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            HSCV_VANBANDENBusiness = Get<HSCV_VANBANDENBusiness>();

            TAILIEUDINHKEM attachment = TAILIEUDINHKEMBusiness.Find(attachmentId);
            viewModel.Attachment = attachment;
            List<TAILIEUDINHKEM> attachments = new List<TAILIEUDINHKEM>();
            if (itemType == MODULE_CONSTANT.VANBANTRINHKY)
            {
                viewModel.EntityVanBanDi = HSCV_VANBANDIBusiness.Find(itemId);
                viewModel.ItemId = itemId;
                viewModel.ItemType = MODULE_CONSTANT.VANBANTRINHKY;
                viewModel.RootItemType = attachment.TAILIEU_GOC_ID.GetValueOrDefault();
                viewModel.GroupAttachments = TAILIEUDINHKEMBusiness.GetData(itemId, LOAITAILIEU.VANBAN)
                   .Where(x => x.TAILIEU_GOC_ID == attachment.TAILIEU_GOC_ID && x.TAILIEU_ID != attachmentId)
                   .OrderByDescending(x => x.TAILIEU_ID).ToList();
            }
            else if (itemType == MODULE_CONSTANT.VANBANDEN)
            {
                viewModel.EntityVanBanDen = HSCV_VANBANDENBusiness.Find(itemId);
                viewModel.ItemType = MODULE_CONSTANT.VANBANDEN;
                viewModel.RootItemType = attachment.TAILIEU_GOC_ID.GetValueOrDefault();
                viewModel.GroupAttachments = TAILIEUDINHKEMBusiness.GetData(itemId, LOAITAILIEU.VANBAN)
                   .Where(x => x.TAILIEU_GOC_ID == attachment.TAILIEU_GOC_ID && x.TAILIEU_ID != attachmentId)
                   .OrderByDescending(x => x.TAILIEU_ID).ToList();
            }
            else
            {

            }
            return PartialView("_EditDocAttachment", viewModel);
        }


        /// <summary>
        /// @description: cập nhật phiên bản mới của tài liệu
        /// @author: duynn
        /// </summary>
        /// <param name="files"></param>
        /// <param name="fc"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateDocAttachment(IEnumerable<HttpPostedFileBase> files, FormCollection fc, string[] filename)
        {
            AssignUserInfo();

            string itemType = fc["ITEM_TYPE"]; //loại tài liệu
            long itemId = fc["ITEM_ID"].ToLongOrZero(); //mã đối tượng
            long rootItemID = fc["ROOT_ITEM_ID"].ToLongOrZero(); //mã tài liệu gốc
            long rollBackId = fc["ROLL_BACK_ID"].ToLongOrZero();
            UploadFileTool tool = new UploadFileTool();
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();

            if (rollBackId > 0)
            {
                List<TAILIEUDINHKEM> savedAttachments = TAILIEUDINHKEMBusiness
                        .GetDataByItemID(itemId, LOAITAILIEU.VANBAN)
                        .Where(x => x.TAILIEU_GOC_ID == rootItemID)
                        .OrderByDescending(x => x.TAILIEU_ID).ToList();
                savedAttachments.ForEach(x =>
                {
                    //lưu chỉ hiển thị tài liệu mới nhất
                    if (x.TAILIEU_ID == rollBackId)
                    {
                        x.IS_ACTIVE = 1;
                    }
                    else
                    {
                        x.IS_ACTIVE = 0;
                    }
                    TAILIEUDINHKEMBusiness.Save(x);
                });
                TempData["RollBackSuccess"] = true;
            }
            else if (files != null && files.Count() > 0)
            {
                string fileExtensions = WebConfigurationManager.AppSettings["VbDenExtension"];
                int fileSize = int.Parse(WebConfigurationManager.AppSettings["VbDenSize"]);

                bool isSaved = tool.UploadFiles(files, fileExtensions.Split(',').ToList(), URLPath, filename, itemId, LOAITAILIEU.VANBAN, 0, currentUser);
                if (isSaved)
                {
                    List<TAILIEUDINHKEM> savedAttachments = TAILIEUDINHKEMBusiness
                        .GetDataByItemID(itemId, LOAITAILIEU.VANBAN)
                        .Where(x => x.TAILIEU_GOC_ID == rootItemID || x.TAILIEU_GOC_ID == null)
                        .OrderByDescending(x => x.TAILIEU_ID).ToList();
                    savedAttachments.ForEach(x =>
                    {
                        //lưu thông tin tài liệu gốc
                        x.TAILIEU_GOC_ID = rootItemID;

                        //lưu chỉ hiển thị tài liệu mới nhất
                        if (x == savedAttachments.ElementAt(0))
                        {
                            x.IS_ACTIVE = 1;
                        }
                        else
                        {
                            x.IS_ACTIVE = 0;
                        }
                        TAILIEUDINHKEMBusiness.Save(x);
                    });
                    TempData["UpdateFiles"] = true;
                }
            }

            if (itemType == MODULE_CONSTANT.VANBANTRINHKY)
            {

                return RedirectToAction("DetailVanBan", "HSVanBanDi", new { area = "HSVanBanDiArea", ID = itemId });
            }
            else if (itemType == MODULE_CONSTANT.VANBANDEN)
            {
                return RedirectToAction("DetailVanBanDen", "HSCV_VANBANDEN", new { area = "HSCV_VANBANDENArea", id = itemId });
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// @author:duynn
        /// @description: danh sách nhóm người dùng nhận văn bản
        /// @since:11/06/2019
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult GetRecipientOfDocument(long idVanBanDi = 0)
        {
            QL_NGUOINHAN_VANBANBusiness = Get<QL_NGUOINHAN_VANBANBusiness>();
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            RecipientDocViewModel viewModel = new RecipientDocViewModel()
            {
                GroupRecipients = QL_NGUOINHAN_VANBANBusiness.repository.All()
                .Where(x => x.IS_DEFAULT == true || x.DM_PHONGBAN_ID == currentUser.DM_PHONGBAN_ID)
                .OrderBy(x=>x.TEN_NHOM)
                .Select(x => new SelectListItem()
                {
                    Value = x.ID.ToString(),
                    Text = x.TEN_NHOM.ToString()
                })
            };


            if(idVanBanDi > 0)
            {
                viewModel.IsSendOthers = true;
                viewModel.EntityVanBanDi = HSCV_VANBANDIBusiness.Find(idVanBanDi);
            }

            return PartialView("_RecipientListOfDoc", viewModel);
        }

        /// <summary>
        /// @author:duynn
        /// @description: tìm kiếm người nhận văn bản
        /// @since: 11/06/2019
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult SearchRecipientOfDocument(FormCollection form)
        {
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            QL_NGUOINHAN_VANBANBusiness = Get<QL_NGUOINHAN_VANBANBusiness>();

            string queryName = form["RECIPIENT_HOTEN"]?.Trim();
            int queryGroup = form["RECIPIENT_GROUP"].ToIntOrZero();

            var users = DM_NGUOIDUNGBusiness.GetUsersByRecipient(queryGroup, queryName);
            return PartialView("_Recipient", users);
        }


        /// <summary>
        /// @author:duynn
        /// @description: thêm mới dữ liệu
        /// </summary>
        /// <param name="groupCode"></param>
        /// <param name="targetID"></param>
        /// <returns></returns>
        public PartialViewResult EditCategoryData(string groupCode, string targetID)
        {
            var DM_NHOMDANHMUCBusiness = Get<DM_NHOMDANHMUCBusiness>();
            DM_NHOMDANHMUC groupCategory = DM_NHOMDANHMUCBusiness.GetByCode(groupCode);
            ViewBag.GroupCategory = groupCategory;
            ViewBag.TargetID = targetID;
            return PartialView("_EditCategoryData");
        }

        /// <summary>
        /// @author:duynn
        /// @description: thêm dữ liệu
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public JsonResult SaveCategory(FormCollection collection)
        {
            var result = new JsonResultBO(true);
            try
            {
                var DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
                DM_DANHMUC_DATA entity = new DM_DANHMUC_DATA();
                entity.DM_NHOM_ID = collection["DM_NHOM_ID"].ToIntOrZero();
                entity.CODE = collection["CODE"].ToString();
                entity.DATA = collection["DATA"].ToIntOrZero();
                entity.TEXT = collection["TEXT"].ToString();
                entity.COLOR = collection["COLOR"].ToString();
                entity.GHICHU = collection["GHICHU"].ToString();

                var checkExistText = DM_DANHMUC_DATABusiness.CheckExistText(entity.TEXT, entity.DM_NHOM_ID.Value);
                var checkExistCode = DM_DANHMUC_DATABusiness.CheckExistCode(entity.CODE, entity.DM_NHOM_ID.Value);
                var checkExistData = DM_DANHMUC_DATABusiness.ExistValue(entity.DM_NHOM_ID.Value, entity.DATA.Value);
                if (checkExistText)
                {
                    result.Status = false;
                    result.Message = "Tên đã tồn tại";
                    return Json(result);
                }

                if (checkExistCode)
                {
                    result.Status = false;
                    result.Message = "Mã đã tồn tại";
                    return Json(result);
                }

                if (checkExistData.Status)
                {
                    result.Status = false;
                    result.Message = "Giá trị đã tồn tại";
                    return Json(result);
                }

                DM_DANHMUC_DATABusiness.Save(entity);
                return Json(entity);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return Json(result);
            }
        }
    }
}