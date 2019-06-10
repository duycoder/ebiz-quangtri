using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Business.Business;
using Business.CommonModel.CONSTANT;
using Business.CommonModel.HSCVVANBANDI;
using CommonHelper;
using Web.Areas.HSVanBanDiArea.Models;
using Web.FwCore;
using Web.Custom;

namespace Web.Areas.HSVanBanDiArea.Controllers
{
    public class VanBanDiDaBanHanhController : BaseController
    {
        // GET: HSVanBanDiArea/VanBanDiDaBanHanh
        private CCTC_THANHPHANBusiness CCTC_THANHPHANBusiness;
        private DM_VAITROBusiness DM_VAITROBusiness;
        private HSCV_VANBANDIBusiness HSCV_VANBANDIBusiness;
        private DM_NGUOIDUNGBusiness DM_NGUOIDUNGBusiness;
        private WF_PROCESSBusiness WF_PROCESSBusiness;
        private WF_ITEM_USER_PROCESSBusiness WF_ITEM_USER_PROCESSBusiness;
        private string VbTrinhKyExtension = WebConfigurationManager.AppSettings["VbDenExtension"];
        private string URL_FOLDER = WebConfigurationManager.AppSettings["FileUpload"];
        private int VbTrinhKySize = int.Parse(WebConfigurationManager.AppSettings["VbDenSize"]);
        private TAILIEUDINHKEMBusiness TAILIEUDINHKEMBusiness;
        private string ROLE_LANHDAODONVI = WebConfigurationManager.AppSettings["ROLE_LANHDAODONVI"];
        private string CODE_ROLE_LANHDAODONVI = WebConfigurationManager.AppSettings["CODE_ROLE_LANHDAODONVI"];
        private string CODE_ROLE_BANTONGGIAMDOC = WebConfigurationManager.AppSettings["CODE_ROLE_BANTONGGIAMDOC"];
        private int MaxPerpage = int.Parse(WebConfigurationManager.AppSettings["MaxPerpage"]);
        private DM_DANHMUC_DATABusiness DM_DANHMUC_DATABusiness;
        private THUMUC_LUUTRUBusiness THUMUC_LUUTRUBusiness;
        public ActionResult Index()
        {
            AssignUserInfo();
            DM_DANHMUC_DATABusiness = Get<DM_DANHMUC_DATABusiness>();
            HSCV_VANBANDI_SEARCH searchModel = new HSCV_VANBANDI_SEARCH();
            VanBanDiVM model = new VanBanDiVM();
            model.LstDoKhan = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOQUANTRONG, 0);
            model.LstDoUuTien = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.DOUUTIEN, 0);
            model.LstLoaiVanBan = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.LOAI_VANBAN, 0);
            model.LstLinhVucVanBan = DM_DANHMUC_DATABusiness.DsByMaNhom(DMLOAI_CONSTANT.LINHVUCVANBAN, 0);
            model.LstSoVanBanDi = DM_DANHMUC_DATABusiness.DsByMaNhom(VanBanConstant.SOVANBANDI, 0);
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            searchModel.USER_ID = currentUser.ID;
            searchModel.ITEM_TYPE = MODULE_CONSTANT.VANBANTRINHKY;
            searchModel.pageSize = MaxPerpage;
            model.ListResult = HSCV_VANBANDIBusiness.GetVanBanDaBanHanh(searchModel, currentUser.DeptParentID.Value, MaxPerpage);
            model.UserInfoBO = currentUser;
            SessionManager.SetValue("VanBanDiBanHanhSearch", searchModel);
            return View(model);
        }
        [HttpPost]
        public JsonResult getData(int indexPage, string sortQuery, int pageSize)
        {
            AssignUserInfo();
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            var searchModel = SessionManager.GetValue("VanBanDiBanHanhSearch") as HSCV_VANBANDI_SEARCH;
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
                SessionManager.SetValue("VanBanDiBanHanhSearch", searchModel);
            }
            var data = HSCV_VANBANDIBusiness.GetVanBanDaBanHanh(searchModel, currentUser.DeptParentID.Value, pageSize, indexPage);
            return Json(data);
        }
        public JsonResult searchData(FormCollection form)
        {
            AssignUserInfo();
            HSCV_VANBANDIBusiness = Get<HSCV_VANBANDIBusiness>();
            var searchModel = SessionManager.GetValue("VanBanDiBanHanhSearch") as HSCV_VANBANDI_SEARCH;
            searchModel.SOHIEU = form["SOHIEU"];
            searchModel.TRICHYEU = form["TRICHYEU"];
            searchModel.DOKHAN_ID = form["DOKHAN_ID"].ToIntOrNULL();
            searchModel.DOUUTIEN_ID = form["DOMAT_ID"].ToIntOrNULL();
            searchModel.LINHVUCVANBAN_ID = form["LINHVUCVANBAN_ID"].ToIntOrNULL();
            searchModel.LOAIVANBAN_ID = form["LOAIVANBAN_ID"].ToIntOrNULL();
            searchModel.SOVANBAN_ID = form["SOVANBANDI_ID"].ToIntOrZero();
            SessionManager.SetValue("VbReview", searchModel);
            var data = HSCV_VANBANDIBusiness.GetVanBanDaBanHanh(searchModel, currentUser.DeptParentID.Value, searchModel.pageSize, 1);
            return Json(data);
        }
    }
}