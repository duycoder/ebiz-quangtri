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
    public class VanBanCanReviewController : BaseController
    {
        // GET: /HSVanBanDiArea/VanBanCanReview
        private HSCV_VANBANDIBusiness HSCV_VANBANDIBusiness;
        private DM_NGUOIDUNGBusiness DM_NGUOIDUNGBusiness;
        private WF_ITEM_USER_PROCESSBusiness WF_ITEM_USER_PROCESSBusiness;
        private string VbTrinhKyExtension = WebConfigurationManager.AppSettings["VbDenExtension"];
        private string URL_FOLDER = WebConfigurationManager.AppSettings["FileUpload"];
        private int VbTrinhKySize = int.Parse(WebConfigurationManager.AppSettings["VbDenSize"]);
        private TAILIEUDINHKEMBusiness TAILIEUDINHKEMBusiness;
        private string ROLE_LANHDAODONVI = WebConfigurationManager.AppSettings["ROLE_LANHDAODONVI"];
        private int MaxPerpage = int.Parse(WebConfigurationManager.AppSettings["MaxPerpage"]);
        private DM_DANHMUC_DATABusiness DM_DANHMUC_DATABusiness;
        private WF_REVIEW_USERBusiness WF_REVIEW_USERBusiness;
        private CCTC_THANHPHANBusiness CCTC_THANHPHANBusiness;

        #region Các hàm actionresult 
        public ActionResult Index()
        {
            AssignUserInfo();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            VanBanDiVM model = new VanBanDiVM();
            HSCV_VANBANDI_SEARCH searchModel = new HSCV_VANBANDI_SEARCH();
            model.LstDoKhan = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOQUANTRONG, 0);
            model.LstDoUuTien = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOUUTIEN, 0);
            model.LstLoaiVanBan = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.LOAI_VANBAN, 0);
            model.LstLinhVucVanBan = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.LINHVUCVANBAN, 0);
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            searchModel.USER_ID = currentUser.ID;
            searchModel.ITEM_TYPE = MODULE_CONSTANT.VANBANTRINHKY;
            searchModel.IS_APPROVE = null;
            model.ListResult = HSCV_VANBANDIBusiness.GetListReview(searchModel, MaxPerpage);
            model.UserInfoBO = currentUser;
            searchModel.pageSize = MaxPerpage;
            SessionManager.SetValue("VbCanReview", searchModel);
            return View(model);
        }
        public ActionResult DetailVanBan(long ID)
        {
            AssignUserInfo();
            #region check quyền truy cập của người dùng đến văn bản hiện tại
            WF_ITEM_USER_PROCESSBusiness = Get<WF_ITEM_USER_PROCESSBusiness>();
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            DM_NGUOIDUNGBusiness = Get<DM_NGUOIDUNGBusiness>();
            var WF_REVIEW_USERBusiness = Get<WF_REVIEW_USERBusiness>();
            DM_DANHMUC_DATABusiness DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            // check quyền truy cập theo workflow
            bool HasPermissionAccess = WF_ITEM_USER_PROCESSBusiness.CheckPermissionProcess(ID, MODULE_CONSTANT.VANBANTRINHKY, currentUser.ID);
            // check quyền truy cập với quyền review
            bool HasPermissionReview = WF_REVIEW_USERBusiness.CheckPermissionReview(ID, MODULE_CONSTANT.VANBANTRINHKY, currentUser.ID);
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
            #region danh sách comment
            var HscvVanBanDiTraoDoiBusiness = Get<HSCV_VANBANDI_TRAODOIBusiness>();
            myModel.LstNoiDungTraoDoi = HscvVanBanDiTraoDoiBusiness.GetListCommentByVanBanDiId(ID);
            myModel.LstRootComment = myModel.LstNoiDungTraoDoi.Where(x => x.REPLY_ID == null).OrderByDescending(x => x.NGAYTAO).ToList();
            List<long> LstRootCommentIds = myModel.LstRootComment.Select(x => x.ID).ToList();
            var LstTaiLieuComment = TAILIEUDINHKEMBusiness.GetDataForTaskByListItemId(LstRootCommentIds, LOAITAILIEU.NOIDUNGTRAODOIVANBANDI);
            myModel.LstTaiLieuComment = LstTaiLieuComment;
            #endregion
            #endregion
            return View(myModel);
        }
        #endregion
        #region Các hàm private
        #endregion
        #region Các hàm partialview
        #endregion
        #region Các hàm jsonresult
        [HttpPost]
        public JsonResult getData(int indexPage, string sortQuery, int pageSize)
        {
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            var searchModel = SessionManager.GetValue("VbCanReview") as HSCV_VANBANDI_SEARCH;
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
                SessionManager.SetValue("VbCanReview", searchModel);
            }
            var data = HSCV_VANBANDIBusiness.GetListReview(searchModel, pageSize, indexPage);
            return Json(data);
        }
        #endregion
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
            SessionManager.SetValue("VbCanReview", searchModel);
            var data = HSCV_VANBANDIBusiness.GetListReview(searchModel, searchModel.pageSize, 1);
            return Json(data);
        }
    }
}