using Business.Business;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.HSCVVANBANDI;
using CommonHelper;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Web.Areas.HSVanBanDiArea.Models;
using Web.Custom;
using Web.FwCore;

namespace Web.Areas.HSVanBanDiArea.Controllers
{
    public class VanBanDaXuLyController : BaseController
    {
        // GET: HSVanBanDiArea/VanBanDaXuLy
        private HSCV_VANBANDIBusiness HSCV_VANBANDIBusiness;
        private DM_NGUOIDUNGBusiness DM_NGUOIDUNGBusiness;
        private WF_PROCESSBusiness WF_PROCESSBusiness;
        private WF_ITEM_USER_PROCESSBusiness WF_ITEM_USER_PROCESSBusiness;
        private string VbTrinhKyExtension = WebConfigurationManager.AppSettings["VbDenExtension"];
        private string URL_FOLDER = WebConfigurationManager.AppSettings["FileUpload"];
        private int VbTrinhKySize = int.Parse(WebConfigurationManager.AppSettings["VbDenSize"]);
        private TAILIEUDINHKEMBusiness TAILIEUDINHKEMBusiness;
        private string ROLE_LANHDAODONVI = WebConfigurationManager.AppSettings["ROLE_LANHDAODONVI"];
        private string CODE_ROLE_THUHOIVANBAN = WebConfigurationManager.AppSettings["CODE_ROLE_THUHOIVANBAN"];
        private int MaxPerpage = int.Parse(WebConfigurationManager.AppSettings["MaxPerpage"]);
        private DM_DANHMUC_DATABusiness DM_DANHMUC_DATABusiness;
        private CCTC_THANHPHANBusiness CCTC_THANHPHANBusiness;
        public ActionResult Index()
        {
            AssignUserInfo();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            VanBanDiVM model = new VanBanDiVM();
            HSCV_VANBANDI_SEARCH searchModel = new HSCV_VANBANDI_SEARCH();
            model.LstDoKhan = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOQUANTRONG,0);
            model.LstDoUuTien = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOUUTIEN, 0);
            model.LstLoaiVanBan = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.LOAI_VANBAN, 0);
            model.LstLinhVucVanBan = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.LINHVUCVANBAN, 0);
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            searchModel.USER_ID = currentUser.ID;
            searchModel.ITEM_TYPE = MODULE_CONSTANT.VANBANTRINHKY;
            model.ListResult = HSCV_VANBANDIBusiness.GetListProcessed(searchModel, MaxPerpage);
            model.UserInfoBO = currentUser;
            model.HasRoleThuHoi = currentUser.ListThaoTac.Where(x => x.MA_THAOTAC == CODE_ROLE_THUHOIVANBAN).FirstOrDefault() != null;
            searchModel.pageSize = MaxPerpage;
            SessionManager.SetValue("VbDaXlSearch", searchModel);

            #region lấy thông tin wf
            model.STATEBEGIN = 0;
            var WF_MODULEBusiness = Get<WF_MODULEBusiness>();
            var WFMODULEObj = WF_MODULEBusiness.repository.All().Where(x => x.MODULE_CODE == MODULE_CONSTANT.VANBANTRINHKY).FirstOrDefault();
            if(WFMODULEObj != null)
            {
                var LstWFId = WFMODULEObj.WF_STREAM_ID.ToListInt(',');
                var WF_STREAMBusiness = Get<WF_STREAMBusiness>();
                var WF_STATEBusiness = Get<WF_STATEBusiness>();
                var CoCauBusiness = Get<CCTC_THANHPHANBusiness>();
                var DeptObj = CoCauBusiness.Find(currentUser.DM_PHONGBAN_ID);
                var StreamObj = WF_STREAMBusiness.repository.All()
                    .Where(x => LstWFId.Contains(x.ID) && x.LEVEL_ID == DeptObj.CATEGORY).FirstOrDefault();
                var StateBegin = WF_STATEBusiness.repository.All().Where(x => StreamObj.ID == x.WF_ID && true == x.IS_START).FirstOrDefault();
                if (StateBegin != null)
                {
                    model.STATEBEGIN = StateBegin.ID;
                    var WF_PROCESSBusiness = Get<WF_PROCESSBusiness>();
                    model.STARTSTATEBYUSER = WF_PROCESSBusiness.GetState(StateBegin.ID, currentUser);
                }
            }
            #endregion
            return View(model);
        }
        #region Các hàm private

        #endregion
        public ActionResult DetailVanBan(long ID)
        {
            AssignUserInfo();
            #region check quyền truy cập của người dùng đến văn bản hiện tại
            WF_ITEM_USER_PROCESSBusiness = Get<WF_ITEM_USER_PROCESSBusiness>();
            WF_PROCESSBusiness = Get<WF_PROCESSBusiness>();
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            var WF_REVIEW_USERBusiness = Get<WF_REVIEW_USERBusiness>();
            DM_DANHMUC_DATABusiness DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            // check quyền truy cập theo workflow
            bool HasPermissionAccess = WF_ITEM_USER_PROCESSBusiness.CheckPermissionProcess(ID, MODULE_CONSTANT.VANBANTRINHKY, currentUser.ID);
            // check quyền truy cập với quyền review
            bool HasPermissionReview = WF_PROCESSBusiness.CheckPermissionProcess(ID, MODULE_CONSTANT.VANBANTRINHKY, currentUser.ID);
            if (!HasPermissionAccess && !HasPermissionReview)
            {
                return Redirect("/Home/UnAuthor");
            }
            ThongTinVanBanDiVM myModel = new ThongTinVanBanDiVM();
            myModel.VanBanTrinhKy = HSCV_VANBANDIBusiness.Find(ID);
            if (myModel.VanBanTrinhKy == null)
            {
                return Redirect("/Home/UnAuthor");
            }
            #region lấy tên danh mục - do dữ liệu văn bản rất lớn cho nên không join với bảng danh mục mà thực hiện 3,4 câu sub-query, tốc độ nhanh hơn
            // chỉ áp dụng kiểu truy vấn này với màn hình chi tiết 1 văn bản
            // ở màn hình danh sách tuyệt đối không áp dụng query ntn
            var DanhMucDoKhan = DM_DANHMUC_DATABusiness.Find(myModel.VanBanTrinhKy.DOKHAN_ID);
            if (DanhMucDoKhan != null)
            {
                myModel.STR_DOKHAN = DanhMucDoKhan.TEXT;
            }
            var DanhMucDoUuTien = DM_DANHMUC_DATABusiness.Find(myModel.VanBanTrinhKy.DOUUTIEN_ID);
            if (DanhMucDoUuTien != null)
            {
                myModel.STR_DOUUTIEN = DanhMucDoUuTien.TEXT;
            }
            var DanhMucLinhVuc = DM_DANHMUC_DATABusiness.Find(myModel.VanBanTrinhKy.LINHVUCVANBAN_ID);
            if (DanhMucLinhVuc != null)
            {
                myModel.STR_LINHVUCVANBAN = DanhMucLinhVuc.TEXT;
            }
            var DanhMucLoaiVanBan = DM_DANHMUC_DATABusiness.Find(myModel.VanBanTrinhKy.LOAIVANBAN_ID);
            if (DanhMucLoaiVanBan != null)
            {
                myModel.STR_LOAIVANBAN = DanhMucLoaiVanBan.TEXT;
            }
            var NguoiKyVanBan = DM_NGUOIDUNGBusiness.Find(myModel.VanBanTrinhKy.NGUOIKY_ID);
            if (NguoiKyVanBan != null)
            {
                myModel.STR_NGUOIKY = NguoiKyVanBan.HOTEN;
            }
            //Danh sách tài liệu đính kèm của văn bản
            TAILIEUDINHKEMBusiness = Get<TAILIEUDINHKEMBusiness>();
            myModel.ListTaiLieu = TAILIEUDINHKEMBusiness.GetDataByItemID(ID, LOAITAILIEU.VANBAN);
            if (!string.IsNullOrEmpty(myModel.VanBanTrinhKy.DONVINHAN_INTERNAL_ID))
            {
                CCTC_THANHPHANBusiness = Get<CCTC_THANHPHANBusiness>();
                myModel.ListDonVi = CCTC_THANHPHANBusiness.GetDataByIds(myModel.VanBanTrinhKy.DONVINHAN_INTERNAL_ID.ToListInt(','));
            }
            else
            {
                myModel.ListDonVi = new List<CCTC_THANHPHAN>();
            }
            #endregion
            #endregion
            #region danh sách comment
            var HscvVanBanDiTraoDoiBusiness = Get<HSCV_VANBANDI_TRAODOIBusiness>();
            myModel.LstNoiDungTraoDoi = HscvVanBanDiTraoDoiBusiness.GetListCommentByVanBanDiId(ID);
            myModel.LstRootComment = myModel.LstNoiDungTraoDoi.Where(x => x.REPLY_ID == null).OrderByDescending(x => x.NGAYTAO).ToList();
            List<long> LstRootCommentIds = myModel.LstRootComment.Select(x => x.ID).ToList();
            var LstTaiLieuComment = TAILIEUDINHKEMBusiness.GetDataForTaskByListItemId(LstRootCommentIds, LOAITAILIEU.NOIDUNGTRAODOIVANBANDI);
            myModel.LstTaiLieuComment = LstTaiLieuComment;
            #endregion
            return View(myModel);
        }
        [HttpPost]
        public JsonResult ThuHoiVanBan(long id)
        {
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            var WF_PROCESSBusiness = Get<WF_PROCESSBusiness>();
            var WF_STATEBusiness = Get<WF_STATEBusiness>();
            var ProcessObj = WF_PROCESSBusiness.repository.All().Where(x => x.ITEM_TYPE == MODULE_CONSTANT.VANBANTRINHKY && x.ITEM_ID == id).FirstOrDefault();
            if(ProcessObj != null)
            {
                AssignUserInfo();
                var VanBanObj = HSCV_VANBANDIBusiness.repository.Find(id);
                if(VanBanObj != null)
                {
                    var DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
                    var UserBoInfo = DM_NGUOIDUNGBusiness.GetUserInfo(VanBanObj.CREATED_BY.Value);
                    int STARTSTATEBYUSERId = WF_PROCESSBusiness.GetState(ProcessObj.WF_ID.Value, UserBoInfo);
                    var STARTSTATEBYUSER = WF_STATEBusiness.repository.Find(STARTSTATEBYUSERId);

                    ProcessObj.CURRENT_STATE = STARTSTATEBYUSER.ID;
                    ProcessObj.CURRENT_STATE_NAME = STARTSTATEBYUSER.STATE_NAME;
                    ProcessObj.USER_ID = VanBanObj.CREATED_BY;
                    WF_PROCESSBusiness.Save(ProcessObj);

                    var log = new WF_LOG();
                    log.ITEM_ID = ProcessObj.ITEM_ID;
                    log.ITEM_TYPE = ProcessObj.ITEM_TYPE;
                    log.NGUOIXULY_ID = ProcessObj.USER_ID;
                    log.WF_ID = ProcessObj.WF_ID;
                    log.MESSAGE = "<div class='label label-danger'>Thu hồi</div>";
                    log.STEP_ID = null;
                    log.create_at = DateTime.Now;
                    
                    log.create_by = currentUser.ID;
                    var LogBusiness = Get<WF_LOGBusiness>();
                    LogBusiness.Save(log);
                }
            }
            return Json(new { Type = "SUCCESS", Message = "thu hồi bản trình ký thành công" });
        }
        [HttpPost]
        public JsonResult getData(int indexPage, string sortQuery, int pageSize)
        {
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            var searchModel = SessionManager.GetValue("VbDaXlSearch") as HSCV_VANBANDI_SEARCH;
            if (!string.IsNullOrEmpty(sortQuery))
            {
                if (searchModel == null)
                {
                    searchModel = new HSCV_VANBANDI_SEARCH();
                }
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue("VbChuaXlSearch", searchModel);
            }
            var data = HSCV_VANBANDIBusiness.GetListProcessed(searchModel, pageSize, indexPage);
            return Json(data);
        }
        public JsonResult searchData(FormCollection form)
        {
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            var searchModel = SessionManager.GetValue("VbDaXlSearch") as HSCV_VANBANDI_SEARCH;
            searchModel.SOHIEU = form["SOHIEU"];
            searchModel.TRICHYEU = form["TRICHYEU"];
            searchModel.DOKHAN_ID = form["DOKHAN_ID"].ToIntOrNULL();
            searchModel.DOUUTIEN_ID = form["DOMAT_ID"].ToIntOrNULL();
            searchModel.LINHVUCVANBAN_ID = form["LINHVUCVANBAN_ID"].ToIntOrNULL();
            searchModel.LOAIVANBAN_ID = form["LOAIVANBAN_ID"].ToIntOrNULL();
            //searchModel. = form["NGUOIKY"];
            SessionManager.SetValue("VanBanDenSearch", searchModel);
            var data = HSCV_VANBANDIBusiness.GetListProcessed(searchModel, searchModel.pageSize, 1);
            return Json(data);
        }
    }
}